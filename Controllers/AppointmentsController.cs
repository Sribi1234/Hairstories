using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hairstories.Data.Repositories;
using Hairstories.Models;

namespace Hairstories.Controllers
{
    /// <summary>
    /// Appointments management controller
    /// </summary>
    [Authorize(Roles = "Admin,Receptionist,Stylist")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentRepository appointmentRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            IServiceRepository serviceRepository,
            ILogger<AppointmentsController> logger)
        {
            _appointmentRepository = appointmentRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;
                var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(targetDate);
                
                ViewBag.SelectedDate = targetDate;
                ViewBag.Appointments = appointments;
                
                return View(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                TempData["ErrorMessage"] = "An error occurred while retrieving appointments.";
                return View(new List<Appointment>());
            }
        }

        public async Task<IActionResult> Calendar(int? month = null, int? year = null)
        {
            try
            {
                var today = DateTime.Now;
                var targetMonth = month ?? today.Month;
                var targetYear = year ?? today.Year;

                var startDate = new DateTime(targetYear, targetMonth, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var appointments = new List<Appointment>();
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayAppointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);
                    appointments.AddRange(dayAppointments);
                }

                ViewBag.CurrentMonth = targetMonth;
                ViewBag.CurrentYear = targetYear;
                ViewBag.Appointments = appointments;

                return View(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading calendar view");
                TempData["ErrorMessage"] = "An error occurred while loading the calendar.";
                return View(new List<Appointment>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction("Index");
                }

                return View(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointment {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the appointment.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var customers = await _customerRepository.GetActiveCustomersAsync();
                var stylists = await _staffRepository.GetStylistsAsync();
                var services = await _serviceRepository.GetActiveServicesAsync();

                ViewBag.Customers = customers;
                ViewBag.Stylists = stylists;
                ViewBag.Services = services;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create appointment form");
                TempData["ErrorMessage"] = "An error occurred while loading the form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate business hours
                    if (appointment.AppointmentDateTime < DateTime.Now)
                    {
                        ModelState.AddModelError("AppointmentDateTime", "Appointment date cannot be in the past.");
                    }

                    // Get service to check duration
                    var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
                    if (service == null)
                    {
                        ModelState.AddModelError("ServiceId", "Selected service not found.");
                    }

                    if (ModelState.IsValid && service != null)
                    {
                        // Check double booking
                        var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                            appointment.StaffId,
                            appointment.AppointmentDateTime,
                            service.DurationMinutes);

                        if (!isAvailable)
                        {
                            ModelState.AddModelError(string.Empty, "The selected stylist is not available at this time. Please choose another time slot.");
                            var customers = await _customerRepository.GetActiveCustomersAsync();
                            var stylists = await _staffRepository.GetStylistsAsync();
                            var services = await _serviceRepository.GetActiveServicesAsync();

                            ViewBag.Customers = customers;
                            ViewBag.Stylists = stylists;
                            ViewBag.Services = services;
                            
                            return View(appointment);
                        }

                        appointment.CreatedDate = DateTime.Now;
                        appointment.Status = AppointmentStatus.Scheduled;
                        appointment.PaymentStatus = PaymentStatus.Pending;

                        await _appointmentRepository.AddAsync(appointment);
                        await _appointmentRepository.SaveAsync();

                        TempData["SuccessMessage"] = "Appointment booked successfully.";
                        _logger.LogInformation($"Appointment created for customer {appointment.CustomerId}");
                        return RedirectToAction("Index");
                    }
                }

                var customersData = await _customerRepository.GetActiveCustomersAsync();
                var stylistsData = await _staffRepository.GetStylistsAsync();
                var servicesData = await _serviceRepository.GetActiveServicesAsync();

                ViewBag.Customers = customersData;
                ViewBag.Stylists = stylistsData;
                ViewBag.Services = servicesData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the appointment.");
            }

            return View(appointment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction("Index");
                }

                var customers = await _customerRepository.GetActiveCustomersAsync();
                var stylists = await _staffRepository.GetStylistsAsync();
                var services = await _serviceRepository.GetActiveServicesAsync();

                ViewBag.Customers = customers;
                ViewBag.Stylists = stylists;
                ViewBag.Services = services;

                return View(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointment {id} for edit");
                TempData["ErrorMessage"] = "An error occurred while retrieving the appointment.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
                    if (existingAppointment == null)
                    {
                        TempData["ErrorMessage"] = "Appointment not found.";
                        return RedirectToAction("Index");
                    }

                    var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
                    if (service == null)
                    {
                        ModelState.AddModelError("ServiceId", "Selected service not found.");
                    }

                    if (ModelState.IsValid && service != null)
                    {
                        // Check double booking if time or staff changed
                        if (existingAppointment.StaffId != appointment.StaffId || 
                            existingAppointment.AppointmentDateTime != appointment.AppointmentDateTime)
                        {
                            var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                                appointment.StaffId,
                                appointment.AppointmentDateTime,
                                service.DurationMinutes,
                                id);

                            if (!isAvailable)
                            {
                                ModelState.AddModelError(string.Empty, "The selected stylist is not available at this time.");
                                return View(appointment);
                            }
                        }

                        existingAppointment.CustomerId = appointment.CustomerId;
                        existingAppointment.ServiceId = appointment.ServiceId;
                        existingAppointment.StaffId = appointment.StaffId;
                        existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;
                        existingAppointment.Status = appointment.Status;
                        existingAppointment.PaymentStatus = appointment.PaymentStatus;
                        existingAppointment.Notes = appointment.Notes;
                        existingAppointment.ModifiedDate = DateTime.Now;

                        _appointmentRepository.Update(existingAppointment);
                        await _appointmentRepository.SaveAsync();

                        TempData["SuccessMessage"] = "Appointment updated successfully.";
                        _logger.LogInformation($"Appointment {id} updated");
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the appointment.");
            }

            var customers = await _customerRepository.GetActiveCustomersAsync();
            var stylists = await _staffRepository.GetStylistsAsync();
            var services = await _serviceRepository.GetActiveServicesAsync();

            ViewBag.Customers = customers;
            ViewBag.Stylists = stylists;
            ViewBag.Services = services;

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.ModifiedDate = DateTime.Now;

                _appointmentRepository.Update(appointment);
                await _appointmentRepository.SaveAsync();

                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
                _logger.LogInformation($"Appointment {id} cancelled");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling appointment {id}");
                TempData["ErrorMessage"] = "An error occurred while cancelling the appointment.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePayment(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                appointment.PaymentStatus = PaymentStatus.Paid;
                appointment.ModifiedDate = DateTime.Now;

                _appointmentRepository.Update(appointment);
                await _appointmentRepository.SaveAsync();

                TempData["SuccessMessage"] = "Payment marked as completed.";
                _logger.LogInformation($"Payment completed for appointment {id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating payment for appointment {id}");
                TempData["ErrorMessage"] = "An error occurred while updating the payment.";
                return RedirectToAction("Index");
            }
        }
    }
}

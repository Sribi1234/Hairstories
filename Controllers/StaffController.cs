using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hairstories.Data.Repositories;
using Hairstories.Models;

namespace Hairstories.Controllers
{
    /// <summary>
    /// Staff management controller
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            IStaffRepository staffRepository,
            IServiceRepository serviceRepository,
            ILogger<StaffController> logger)
        {
            _staffRepository = staffRepository;
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var staff = await _staffRepository.GetAllAsync();
                return View(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff list");
                TempData["ErrorMessage"] = "An error occurred while retrieving staff information.";
                return View(new List<Staff>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var staff = await _staffRepository.GetStaffWithServicesAsync(id);
                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                    return RedirectToAction("Index");
                }

                return View(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving staff details for id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving staff details.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new[] { StaffRole.Admin, StaffRole.Receptionist, StaffRole.Stylist };
            return await Task.FromResult(View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStaff = await _staffRepository.GetStaffByEmailAsync(staff.Email);
                    if (existingStaff != null)
                    {
                        ModelState.AddModelError("Email", "A staff member with this email already exists.");
                        ViewBag.Roles = new[] { StaffRole.Admin, StaffRole.Receptionist, StaffRole.Stylist };
                        return View(staff);
                    }

                    staff.CreatedDate = DateTime.Now;
                    await _staffRepository.AddAsync(staff);
                    await _staffRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Staff member added successfully.";
                    _logger.LogInformation($"Staff member {staff.FullName} created");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the staff member.");
            }

            ViewBag.Roles = new[] { StaffRole.Admin, StaffRole.Receptionist, StaffRole.Stylist };
            return View(staff);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                    return RedirectToAction("Index");
                }

                ViewBag.Roles = new[] { StaffRole.Admin, StaffRole.Receptionist, StaffRole.Stylist };
                return View(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving staff for edit with id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the staff member.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingStaff = await _staffRepository.GetByIdAsync(id);
                    if (existingStaff == null)
                    {
                        TempData["ErrorMessage"] = "Staff member not found.";
                        return RedirectToAction("Index");
                    }

                    existingStaff.FirstName = staff.FirstName;
                    existingStaff.LastName = staff.LastName;
                    existingStaff.Email = staff.Email;
                    existingStaff.Phone = staff.Phone;
                    existingStaff.Role = staff.Role;
                    existingStaff.WorkingHours = staff.WorkingHours;
                    existingStaff.IsActive = staff.IsActive;
                    existingStaff.ModifiedDate = DateTime.Now;

                    _staffRepository.Update(existingStaff);
                    await _staffRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Staff member updated successfully.";
                    _logger.LogInformation($"Staff member {existingStaff.FullName} updated");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating staff member {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the staff member.");
            }

            ViewBag.Roles = new[] { StaffRole.Admin, StaffRole.Receptionist, StaffRole.Stylist };
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    return NotFound();
                }

                _staffRepository.Delete(staff);
                await _staffRepository.SaveAsync();

                TempData["SuccessMessage"] = "Staff member deleted successfully.";
                _logger.LogInformation($"Staff member {staff.FullName} deleted");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting staff member {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the staff member.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> AssignServices(int id)
        {
            try
            {
                var staff = await _staffRepository.GetStaffWithServicesAsync(id);
                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                    return RedirectToAction("Index");
                }

                var allServices = await _serviceRepository.GetActiveServicesAsync();
                var assignedServiceIds = staff.StaffServices?.Select(ss => ss.ServiceId).ToList() ?? new List<int>();

                var viewModel = new StaffServicesViewModel
                {
                    StaffId = staff.Id,
                    StaffName = staff.FullName,
                    AssignedServices = allServices.Where(s => assignedServiceIds.Contains(s.Id)).ToList(),
                    AvailableServices = allServices.Where(s => !assignedServiceIds.Contains(s.Id)).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading service assignment for staff {id}");
                TempData["ErrorMessage"] = "An error occurred while loading services.";
                return RedirectToAction("Index");
            }
        }
    }

    public class StaffServicesViewModel
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public List<Service> AssignedServices { get; set; } = new();
        public List<Service> AvailableServices { get; set; } = new();
    }
}

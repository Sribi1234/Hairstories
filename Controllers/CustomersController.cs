using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hairstories.Data.Repositories;
using Hairstories.Models;

namespace Hairstories.Controllers
{
    /// <summary>
    /// Customer management controller
    /// </summary>
    [Authorize(Roles = "Admin,Receptionist")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerRepository customerRepository,
            IAppointmentRepository appointmentRepository,
            ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers list");
                TempData["ErrorMessage"] = "An error occurred while retrieving customers.";
                return View(new List<Customer>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerWithHistoryAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Customer not found.";
                    return RedirectToAction("Index");
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer details for id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving customer details.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                    if (existingCustomer != null)
                    {
                        ModelState.AddModelError("Email", "A customer with this email already exists.");
                        return View(customer);
                    }

                    customer.CreatedDate = DateTime.Now;
                    await _customerRepository.AddAsync(customer);
                    await _customerRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Customer added successfully.";
                    _logger.LogInformation($"Customer {customer.FullName} created");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the customer.");
            }

            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Customer not found.";
                    return RedirectToAction("Index");
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer for edit with id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the customer.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingCustomer = await _customerRepository.GetByIdAsync(id);
                    if (existingCustomer == null)
                    {
                        TempData["ErrorMessage"] = "Customer not found.";
                        return RedirectToAction("Index");
                    }

                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.Phone = customer.Phone;
                    existingCustomer.Notes = customer.Notes;
                    existingCustomer.IsActive = customer.IsActive;
                    existingCustomer.ModifiedDate = DateTime.Now;

                    _customerRepository.Update(existingCustomer);
                    await _customerRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Customer updated successfully.";
                    _logger.LogInformation($"Customer {existingCustomer.FullName} updated");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the customer.");
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _customerRepository.Delete(customer);
                await _customerRepository.SaveAsync();

                TempData["SuccessMessage"] = "Customer deleted successfully.";
                _logger.LogInformation($"Customer {customer.FullName} deleted");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the customer.";
                return RedirectToAction("Index");
            }
        }
    }
}

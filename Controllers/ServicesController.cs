using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hairstories.Data.Repositories;
using Hairstories.Models;

namespace Hairstories.Controllers
{
    /// <summary>
    /// Services management controller
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(
            IServiceRepository serviceRepository,
            ILogger<ServicesController> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var services = await _serviceRepository.GetAllAsync();
                return View(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services list");
                TempData["ErrorMessage"] = "An error occurred while retrieving services.";
                return View(new List<Service>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving service details for id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the service.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingService = await _serviceRepository.GetServiceByNameAsync(service.ServiceName);
                    if (existingService != null)
                    {
                        ModelState.AddModelError("ServiceName", "A service with this name already exists.");
                        return View(service);
                    }

                    service.CreatedDate = DateTime.Now;
                    await _serviceRepository.AddAsync(service);
                    await _serviceRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Service added successfully.";
                    _logger.LogInformation($"Service {service.ServiceName} created");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the service.");
            }

            return View(service);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving service for edit with id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the service.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingService = await _serviceRepository.GetByIdAsync(id);
                    if (existingService == null)
                    {
                        TempData["ErrorMessage"] = "Service not found.";
                        return RedirectToAction("Index");
                    }

                    existingService.ServiceName = service.ServiceName;
                    existingService.Description = service.Description;
                    existingService.DurationMinutes = service.DurationMinutes;
                    existingService.Price = service.Price;
                    existingService.IsActive = service.IsActive;
                    existingService.ModifiedDate = DateTime.Now;

                    _serviceRepository.Update(existingService);
                    await _serviceRepository.SaveAsync();

                    TempData["SuccessMessage"] = "Service updated successfully.";
                    _logger.LogInformation($"Service {existingService.ServiceName} updated");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating service {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the service.");
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return NotFound();
                }

                _serviceRepository.Delete(service);
                await _serviceRepository.SaveAsync();

                TempData["SuccessMessage"] = "Service deleted successfully.";
                _logger.LogInformation($"Service {service.ServiceName} deleted");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting service {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the service.";
                return RedirectToAction("Index");
            }
        }
    }
}

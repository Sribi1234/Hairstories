using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hairstories.Data.Repositories;
using Hairstories.Models;
using System.Linq;

namespace Hairstories.Controllers
{
    /// <summary>
    /// Home controller for dashboard and general pages
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IAppointmentRepository appointmentRepository,
            ICustomerRepository customerRepository,
            ILogger<HomeController> logger)
        {
            _appointmentRepository = appointmentRepository;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todayBookings = await _appointmentRepository.GetTodayBookingCountAsync();
                var todayRevenue = await _appointmentRepository.GetTodayRevenueAsync();
                var totalCustomers = (await _customerRepository.GetActiveCustomersAsync()).Count();
                var upcomingAppointments = await _appointmentRepository.GetTodayAppointmentsAsync();

                var dashboardData = new
                {
                    TodayBookings = todayBookings,
                    TodayRevenue = todayRevenue,
                    TotalCustomers = totalCustomers,
                    UpcomingCount = upcomingAppointments.Count(a => a.Status != AppointmentStatus.Cancelled)
                };

                ViewBag.TodayBookings = todayBookings;
                ViewBag.TodayRevenue = todayRevenue.ToString("C");
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.UpcomingAppointments = upcomingAppointments.OrderBy(a => a.AppointmentDateTime).Take(5);

                _logger.LogInformation("Dashboard accessed successfully");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View("Error");
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

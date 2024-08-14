using Microsoft.AspNetCore.Mvc;
using rare_crew_c__test.ApiService;
using rare_crew_c__test.DTO;
using rare_crew_c__test.Models;
using System.Diagnostics;

namespace rare_crew_c__test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Service _service;

        public HomeController(ILogger<HomeController> logger, Service service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await _service.GetEmployees();

            if (employees == null || employees.Count == 0)
            {
                ViewBag.Message = "No employees.";
            }
            List<EmployeeDTO> workingHours = await _service.GetEmployeesWithWorkingHours(employees);
            return View(workingHours);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

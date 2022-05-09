using FSMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using System.Data.Odbc;
using FSMS.Services;

namespace FSMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;

        public HomeController(ILogger<HomeController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            List<TestEmployee> employees = new List<TestEmployee>();
            employees = _customerService.GetEmployees();
            return View();
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
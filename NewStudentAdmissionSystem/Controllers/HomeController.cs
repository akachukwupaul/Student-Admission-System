using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewStudentAdmissionSystem.Models;
using System.Diagnostics;

namespace NewStudentAdmissionSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Apply()
        {
            // Redirect to application form
            return RedirectToAction("ApplicationForm", "Admission");
        }

        //[Authorize (Roles = "Admin")]
        //public IActionResult Admin()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "User")]

        //public IActionResult User()
        //{
        //    return View();
        //}


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

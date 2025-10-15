using Microsoft.AspNetCore.Mvc;

namespace NewStudentAdmissionSystem.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

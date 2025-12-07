//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;
using NewStudentAdmissionSystem.Services;

namespace NewStudentAdmissionSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _Context;
        private readonly IStudentService _StudentService;

        public StudentController(AppDbContext context, IStudentService studentService)
        {
            _Context = context;
            _StudentService = studentService;
        }

        //[Authorize(Roles = "User")]
        //public IActionResult Dashboard()
        //{
        //    return View();
        //}
        [HttpGet]
        //[Authorize(Roles = "User")]
        public IActionResult Register()
        {
            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View();
        }
        [HttpPost]
        //[Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentApplication model)
        {

            if (ModelState.IsValid)
            {
                var applicationNumber = await _StudentService.RegisterStudent(model);
                ViewBag.ApplicationNumber = applicationNumber;
                return View("RegistrationSuccess");
            }


            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View(model);
        }
        [HttpGet]

        public IActionResult CheckStatus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckStatus(string ApplicationNumber)
        {

            var (student, message) = await _StudentService.CheckApplicationStatus(ApplicationNumber);

            ViewBag.Message = message;
            ViewBag.Student = student;

            return View();
        }
        // This makes the data available for rendering in the view.}
    }

 }


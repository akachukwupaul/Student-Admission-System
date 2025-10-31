using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;

namespace NewStudentAdmissionSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _Context;      
        public StudentController(AppDbContext context)
        {
            _Context = context;
        }

        [Authorize(Roles = "User")]
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Register()
        {
            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentApplication model)
        {
            if (ModelState.IsValid)
            {
                var student = new StudentApplication
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    CourseId = model.CourseId,
                    PhoneNumber = model.PhoneNumber,
                    Status = AdmissionStatus.PENDING,
                    AcademicInfo = model.AcademicInfo,
                    DateOfBirth = model.DateOfBirth,
                    ApplicationNumber = model.ApplicationNumber ="STU" + $"{DateTime.Now.Year}{_Context.StudentApplications.Count() + 1:D4}",
                    DateOfApplication = DateTime.Now

                };               
                _Context.StudentApplications.Add(student);
                await _Context.SaveChangesAsync();
                ViewBag.ApplicationNumber = model.ApplicationNumber;

                return View("RegistrationSuccess");
            }
            // Log invalid fields for debugging
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Validation Error: " + error.ErrorMessage);
            }

            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View(model);       

        }
    }
}

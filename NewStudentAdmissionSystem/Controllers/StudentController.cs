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
            // Checks if the data submitted in the model object is valid.
            if (ModelState.IsValid)
            {
                // Create a new instance of the database entity model (StudentApplication).
                var student = new StudentApplication
                {
                    // Map data from the incoming form model to the database entity model.
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
            //foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            //{
            //    Console.WriteLine("Validation Error: " + error.ErrorMessage);
            //}

            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View(model);       
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckStatus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckStatus(string ApplicationNumber)
        {
            // Checks if the database query returned a student record.
            if (string.IsNullOrEmpty(ApplicationNumber))
            {
                ViewBag.Message = "Please enter your application number";
                return View();
            }
            
            var student = _Context.StudentApplications
                 .Include(s => s.Course)
                .FirstOrDefault (a => a.ApplicationNumber == ApplicationNumber);

            if (student == null)
            {
                ViewBag.Message = "invalid Application number";
                return View();
            }

            switch (student.Status)
            {
                case AdmissionStatus.ACCEPTED:
                    {
                        ViewBag.Message = "Congratulations! Your application has been accepted.";
                    }
                    break;
                case AdmissionStatus.PENDING:
                    {
                        ViewBag.Message = "Your application is under review. Please check back later.";
                    }
                    break;
                 case AdmissionStatus.REJECTED:
                    {
                        ViewBag.Message = "We regret to inform you that your application was not successful.";
                    }
                    break;
                    default: 
                    {
                        ViewBag.Message = "Unknown application status";
                    }
                    break ;
            }
            // This makes the data available for rendering in the view.
            ViewBag.Student = student;
            return View();

        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;
using NewStudentAdmissionSystem.Services;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NewStudentAdmissionSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _Context;
        private readonly IStudentService _StudentService;
        public AdminController(AppDbContext context, IStudentService studentService)
        {
            _Context = context;
            _StudentService = studentService;
        }

        [HttpGet]
        public IActionResult AdminHome()
        {
            return View();
        }

        private IQueryable<StudentApplication> BuildStudentQuery(string searchString)
        {
            
            var query = _Context.StudentApplications
                .Include(s => s.Course) 
                .AsQueryable();
           
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(s =>
                    (s.FirstName != null && s.FirstName.Contains(searchString)) ||
                    (s.LastName != null && s.LastName.Contains(searchString)) ||
                    (s.ApplicationNumber != null && s.ApplicationNumber.Contains(searchString)) ||
                    (s.Course != null && s.Course.Name.Contains(searchString))
                );
            }

            return query;
        }

        private async Task<IPagedList<StudentApplication>> GetPagedStudents(string searchString, int page)
        {
            const int PageSize = 7;
            var query = BuildStudentQuery(searchString);

            return await query
                .OrderByDescending(s => s.Id)
                .ToPagedListAsync(page, PageSize);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string searchString, int page = 1)
        {
           ViewBag.SearchString = searchString;
            var students = await GetPagedStudents(searchString, page);

            return View(students);
            
        }

        [HttpGet]
        public async Task<IActionResult> StudentRecords(string searchString, int page = 1)
        {
            ViewBag.SearchString = searchString;
            var students = await GetPagedStudents(searchString, page);

            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AdmissionStatus status)
        {
            var student = await _Context.StudentApplications.FindAsync(id);

            
            if (student == null)
            {
                return NotFound();
            }
            student.Status = status;
            _Context.Update(student);
            await _Context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Student status updated to {status}.";
            return RedirectToAction(nameof(Dashboard));
        }



        [HttpGet]
        public IActionResult AddStudent()
        {
            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(StudentApplication model)
        {
            if (ModelState.IsValid)
            {
                var applicationNumber = await _StudentService.RegisterStudent(model);
                TempData["SuccessMessage"] = $"Student registered successfully! Application Number: {applicationNumber}";
                return RedirectToAction("Dashboard");
            }

            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var student = await _Context.StudentApplications
                .Include(c => c.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpGet]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _Context.StudentApplications.
                 Include(c => c.Course)
                 .FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name", student.CourseId);
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, StudentApplication student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _Context.Update(student);
                    await _Context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Student information updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Unable to update student information. Please try again.";
                    return NotFound();
                }
                return RedirectToAction("Dashboard");
            }
            ViewBag.Courses = new SelectList(_Context.Courses, "Id", "Name", student.CourseId);
            return View(student);
        }



        [HttpGet]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _Context.StudentApplications
                .FirstOrDefaultAsync(s => s.Id == id);
            return View(student);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = await _Context.StudentApplications.FindAsync(id);
                if (student != null)
                {
                    _Context.StudentApplications.Remove(student);
                }
                await _Context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} deleted successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to delete student. Please try again.";
                return RedirectToAction("Dashboard");
            }
        }


        [HttpGet]
        public IActionResult CheckStudentStatus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckStudentStatus(string ApplicationNumber)
        {
            var (student, message) = await _StudentService.CheckApplicationStatus(ApplicationNumber);

            ViewBag.Message = message;
            ViewBag.Student = student;

            return View();  
        }
    }
}

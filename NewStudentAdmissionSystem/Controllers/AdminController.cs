using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NewStudentAdmissionSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _Context;
        public AdminController(AppDbContext context)
        {
            _Context = context;
        }

        [HttpGet]
        public IActionResult AdminHome()
        {
            return View();
        }

        private IQueryable<StudentApplication> BuildStudentQuery(string searchString)
        {
            // 1. Initialize Query
            var query = _Context.StudentApplications
                .Include(s => s.Course) // Eager loading
                .AsQueryable();

            // 2. Apply Search Filter
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

        [HttpGet]
        public async Task<IActionResult> Dashboard(string searchString, int page = 1)
        {
            int PageSize = 7;

            // CALL THE HELPER: Build the dynamic query.
            var query = BuildStudentQuery(searchString);

            // Continue with view setup and execution.
            ViewBag.SearchString = searchString;

            var paginatedData = await query
                .OrderByDescending(s => s.Id)
                .ToPagedListAsync(page, PageSize);

            return View(paginatedData);
            // return View(await application.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> StudentRecords(string searchString, int page = 1)
        {
            int PageSize = 7;

            // CALL THE HELPER: Use the identical logic here.
            var query = BuildStudentQuery(searchString);

            // Continue with view setup and execution.
            ViewBag.SearchString = searchString;

            var paginatedData = await query
                .OrderByDescending(s => s.Id)
                .ToPagedListAsync(page, PageSize);

            // Note: This will return the StudentRecords view.
            return View(paginatedData);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AdmissionStatus status)
        {
            var student = await _Context.StudentApplications.FindAsync(id);

            // Checks if the student application record was successfully found.
            if (student == null)
            {
                return NotFound();
            }
            student.Status = status;
            _Context.Update(student);
            await _Context.SaveChangesAsync();
           

            return RedirectToAction(nameof(Dashboard));
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
                }
                catch (DbUpdateConcurrencyException)
                {
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
            var student = await _Context.StudentApplications.FirstOrDefaultAsync();
            return View(student);

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _Context.StudentApplications.FindAsync(id);
            if (student != null)
            {
                _Context.StudentApplications.Remove(student);
            }
            await _Context.SaveChangesAsync();
            TempData["Success"] = "Student Deleted Successfully";
            return RedirectToAction("Dashboard");
        }

    }
}

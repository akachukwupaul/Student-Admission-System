using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Dashboard(string searchString, int page = 1)
        {
            int PageSize = 7;

            // Starts building an IQueryable expression from the StudentApplications DbSet.
            var query = _Context.StudentApplications
                              .Include(s => s.Course)
                              .AsQueryable();

            // Checks if the user provided a non-empty, non-whitespace search term.
            if (!string.IsNullOrWhiteSpace(searchString))
            {
               query = query.Where(s =>
               (s.FirstName != null && s.FirstName.Contains(searchString)) ||
               (s.LastName != null && s.LastName.Contains(searchString)) ||
               (s.ApplicationNumber != null && s.ApplicationNumber.Contains(searchString)) ||
               (s.Course != null && s.Course.Name.Contains(searchString))
               );

            }
            // Stores the search term in the ViewBag so the view can retain the search value in the form input
            // and correctly generate pagination links that include the search filter.
            ViewBag.SearchString = searchString;
            var paginatedData = await query
                .OrderByDescending(s => s.Id)
                .ToPagedListAsync(page, PageSize);

            return View(paginatedData);
            // return View(await application.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Checks if the 'id' passed in the URL is null
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
    }
}

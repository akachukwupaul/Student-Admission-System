using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;

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
        public async Task<IActionResult> Dashboard(string searchString)
        {
            var application = from a in _Context.StudentApplications
                              .Include(c => c.Course)
                              select a;
            if (!string.IsNullOrEmpty(searchString))
            {
                application = application.Where(s => s.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                                                     s.LastName.ToUpper().Contains(searchString.ToUpper()) ||
                                                     s.ApplicationNumber.Contains(searchString));
            }
            return View(await application.ToListAsync());
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

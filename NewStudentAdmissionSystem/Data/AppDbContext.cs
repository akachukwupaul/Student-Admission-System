using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Models;
//using System.Reflection.Emit;

namespace NewStudentAdmissionSystem.Data
{
    public class AppDbContext: IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Computer Engineering" },
                new Course { Id = 2, Name = "Electrical Engineering" },
                new Course { Id = 3, Name = "Microbiology" },
                new Course { Id = 4, Name = "Mass communication" },
                new Course { Id = 5, Name = "Medicine" },
                new Course { Id = 6, Name = "Law" },
                new Course { Id = 7, Name = "Accounting" }
                );
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<StudentApplication> StudentApplications { get; set; }
        public DbSet<Course> Courses { get; set; }
    }   
}

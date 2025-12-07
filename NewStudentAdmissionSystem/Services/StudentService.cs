using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;

namespace NewStudentAdmissionSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _Context;
        public StudentService(AppDbContext context)
        {
            _Context = context;
        }
        public async Task<string> RegisterStudent(StudentApplication model)
        {

            var lastId = await _Context.StudentApplications
                             .OrderByDescending(s => s.Id)
                             .Select(s => s.Id)
                             .FirstOrDefaultAsync();

            var newSerial = lastId + 1;

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
                ApplicationNumber = "STU" + $"{DateTime.Now.Year}{newSerial:D4}",
                DateOfApplication = DateTime.Now
            };

            _Context.StudentApplications.Add(student);
            await _Context.SaveChangesAsync();

            return student.ApplicationNumber;
        }

        public async Task<(StudentApplication student, string message)> CheckApplicationStatus(string applicationNumber)
        {
            if (string.IsNullOrEmpty(applicationNumber))
            {
                return (null, "Please enter your application number");
            }

            var student = await _Context.StudentApplications
                .Include(s => s.Course)
                .FirstOrDefaultAsync(a => a.ApplicationNumber == applicationNumber);

            if (student == null)
            {
                return (null, "Invalid Application number");
            }

           string message;

switch (student.Status)
{
    case AdmissionStatus.ACCEPTED:
        message = "Congratulations! Your application has been accepted.";
        break;

    case AdmissionStatus.PENDING:
        message = "Your application is under review. Please check back later.";
        break;

    case AdmissionStatus.REJECTED:
        message = "We regret to inform you that your application was not successful.";
        break;

    default:
        message = "Unknown application status";
        break;
}

            return (student, message);
        }
    }
}

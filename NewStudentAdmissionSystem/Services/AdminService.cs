using Microsoft.EntityFrameworkCore;
using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Data;
using NewStudentAdmissionSystem.Models;

namespace NewStudentAdmissionSystem.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _Context;
        public AdminService(AppDbContext context)
        {
            _Context = context;
        }
        public async Task<bool> StudentExists(int id)
        {
            return await _Context.StudentApplications.AnyAsync(s => s.Id == id);
        }
        public async Task<bool> UpdateStudentStatus(int id, AdmissionStatus status)
        {
            var student = await _Context.StudentApplications.FindAsync(id);


            if (student == null)
            {
                return false;
            }
            student.Status = status;
            _Context.Update(student);
            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<StudentApplication> GetStudentDetails(int id)
        {

            var student = await _Context.StudentApplications
               .Include(s => s.Course)
               .FirstOrDefaultAsync(s => s.Id == id);

            return(student);
        }

        public async Task<bool> EditStudentRecord(StudentApplication student)
        {
            try
            {
                _Context.Update(student);
                await _Context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StudentExists(student.Id))
                {
                    return false;
                }              
               throw;
            }
        }

        public async Task<(bool, String FirstName, String LastName)> DeleteStudentRecord(int id)
        {
            try
            {
                var student = await _Context.StudentApplications.FindAsync(id);
                if (student == null)
                {
                    return (false, null, null);
                }

                var firstName = student.FirstName;
                var lastName = student.LastName;

                _Context.StudentApplications.Remove(student);
                await _Context.SaveChangesAsync();
               
                return (true, firstName, lastName);
            }
            catch (Exception ex)
            {               
                return (false, null, null);
            }
        }
       
    }
}

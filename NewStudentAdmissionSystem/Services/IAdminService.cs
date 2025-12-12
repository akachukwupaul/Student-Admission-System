using NewStudentAdmissionSystem.Constants;
using NewStudentAdmissionSystem.Models;

namespace NewStudentAdmissionSystem.Services
{
    public interface IAdminService
    {
        Task<bool> StudentExists(int id);
        Task<bool> UpdateStudentStatus(int id, AdmissionStatus status);
        Task<StudentApplication> GetStudentDetails(int id);
        Task<bool> EditStudentRecord(StudentApplication student);
        Task<(bool, String FirstName, String LastName)> DeleteStudentRecord(int id);
    }
}

using NewStudentAdmissionSystem.Models;

namespace NewStudentAdmissionSystem.Services
{
    public interface IStudentService
    {
        Task<string> RegisterStudent(StudentApplication model);
        Task<(StudentApplication student, string message)> CheckApplicationStatus(string applicationNumber);
    }
}

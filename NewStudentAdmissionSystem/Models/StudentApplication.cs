using NewStudentAdmissionSystem.Constants;
using System.ComponentModel.DataAnnotations;

namespace NewStudentAdmissionSystem.Models
{
    public class StudentApplication
    {
        public int Id { get; set; }
        public string? ApplicationNumber { get; set; }
        [Required (ErrorMessage ="Enter your First name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your Last name")]
        public string? LastName { get; set; }

        [Required, EmailAddress]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Please select a course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please enter your academic information")]
        public string? AcademicInfo { get; set; }


        public Course? Course { get; set; }
        public string? PhoneNumber { get; set; }
        public AdmissionStatus Status { get; set; } = AdmissionStatus.PENDING;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public DateTime DateOfApplication { get; set; }

     
    }
}

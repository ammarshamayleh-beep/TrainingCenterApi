using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TrainingCenterApi.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Student
    {
        public int StudentId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; } = "";

        [MaxLength(50)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        // One-to-One: each student has one profile
        public StudentProfile? Profile { get; set; }

        // Many-to-Many: one student has many enrollments
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
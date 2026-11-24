using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.Models
{
    public class StudentProfile
    {
        // Primary key and foreign key at the same time
        public int StudentId { get; set; }

        [MaxLength(50)]
        public string City { get; set; } = "";

        [MaxLength(300)]
        public string? Bio { get; set; }

        [MaxLength(200)]
        public string? LinkedInUrl { get; set; }

        // Each profile belongs to one student
        public Student? Student { get; set; }
    }
}

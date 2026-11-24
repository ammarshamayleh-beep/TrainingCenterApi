using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingCenterApi.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        // Link to the student
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Link to the course
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        // Extra data about the enrollment
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercent { get; set; }
    }
}
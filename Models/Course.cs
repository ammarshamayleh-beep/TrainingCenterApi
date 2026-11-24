using System;
using System.Collections.Generic;
using System.Text;
using TrainingCenterApi.Models.TrainingCenterApi.Models;

namespace TrainingCenterApi.Models
{
    public class Course
    {
        public int CourseId {  get; set; }

        public string Title { get; set; } = " ";

        public string ? Description { get; set; } 

        public decimal Price { get; set; }

        // One-to-Many: each course has one instructor
        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        // Many-to-Many: one course has many enrollments
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}

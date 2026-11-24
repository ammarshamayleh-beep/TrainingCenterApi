using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingCenterApi.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace TrainingCenterApi.Models
    {
        public class Instructor
        {
            public int InstructorId { get; set; }

            [MaxLength(50)]
            public string FirstName { get; set; } = "";

            [MaxLength(50)]
            public string LastName { get; set; } = "";

            // Self-Referencing: the manager is also an instructor
            public int? ManagerId { get; set; }
            public Instructor? Manager { get; set; }
            public List<Instructor> Subordinates { get; set; } = new();

            // One-to-Many: one instructor teaches many courses
            public List<Course> Courses { get; set; } = new();
        }
    }
}

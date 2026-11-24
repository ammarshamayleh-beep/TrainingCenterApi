using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingCenterApi.Models;
using TrainingCenterApi.Models.TrainingCenterApi.Models;


namespace TrainingCenterApi.Data
{
    public class TrainingContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<StudentProfile> StudentProfiles { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<User> Users { get; set; }
        // The options (provider + connection string) come from Program.cs
        public TrainingContext(DbContextOptions<TrainingContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Title)
                      .HasMaxLength(100);

                entity.Property(c => c.Description)
                      .HasMaxLength(500);

                entity.Property(c => c.Price)
                      .HasPrecision(10, 2);

                entity.ToTable(t => t.HasCheckConstraint("CK_Courses_Price", "[Price] >= 0"));
            });

            // Self-Referencing: instructor and manager
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.Manager)
                .WithMany(i => i.Subordinates)
                .HasForeignKey(i => i.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: instructor and courses
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            // One-to-One: student and profile
            modelBuilder.Entity<StudentProfile>()
                .HasKey(p => p.StudentId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Profile)
                .WithOne(p => p.Student)
                .HasForeignKey<StudentProfile>(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: students and courses through enrollments
            modelBuilder.Entity<Enrollment>(entity =>
            {
                
                entity.HasIndex(e => new { e.StudentId, e.CourseId })
                      .IsUnique();

                entity.Property(e => e.ProgressPercent)
                      .HasPrecision(5, 2);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Enrollments_Progress",
                    "[ProgressPercent] BETWEEN 0 AND 100"));
            });
        }
    }
}

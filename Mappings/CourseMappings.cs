using System.Linq.Expressions;
using TrainingCenterApi.DTOs;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Mappings;

// All the conversions between Course and its DTOs, in one place
public static class CourseMappings
{
    // Entity -> Response DTO
    // An Expression, so EF Core can translate it to SQL (SELECT + LEFT JOIN)
    public static readonly Expression<Func<Course, CourseDto>> ToDto = c => new CourseDto
    {
        Id = c.CourseId,
        Title = c.Title,
        Description = c.Description,
        Price = c.Price,
        InstructorName = c.Instructor != null
            ? c.Instructor.FirstName + " " + c.Instructor.LastName
            : null
    };

    // Request DTO -> a NEW entity (for POST)
    public static Course ToEntity(this CreateCourseDto dto)
    {
        return new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            InstructorId = dto.InstructorId
        };
    }

    // Request DTO -> an EXISTING entity (for PUT)
    public static void ApplyTo(this UpdateCourseDto dto, Course course)
    {
        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;
        course.InstructorId = dto.InstructorId;
    }
}
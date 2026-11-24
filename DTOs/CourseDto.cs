namespace TrainingCenterApi.DTOs;

// Response DTO: the shape of a course that goes OUT to the client
public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }

    // Not a column in the Courses table: built from the Instructor navigation property
    public string? InstructorName { get; set; }
}

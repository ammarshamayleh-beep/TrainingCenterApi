namespace TrainingCenterApi.DTOs;

// A course with its related data, for GET api/courses/7/details
public class CourseDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }

    // Related data: small DTOs, with no way back to the course (so no cycles)
    public InstructorSummaryDto? Instructor { get; set; }
    public List<EnrolledStudentDto> Students { get; set; } = new();
}

public class InstructorSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
}

public class EnrolledStudentDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public decimal ProgressPercent { get; set; }
    public DateTime EnrollmentDate { get; set; }
}
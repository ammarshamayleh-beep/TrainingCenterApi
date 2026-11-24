using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.DTOs;

public class CreateCourseDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, 99999999.99, ErrorMessage = "Price must be between 0 and 99999999.99.")]
    public decimal Price { get; set; }

    public int? InstructorId { get; set; }
}

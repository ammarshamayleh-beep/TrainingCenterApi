using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingCenterApi.DTOs;

// Request DTO: what the client sends IN to update a student (PUT)
// The Id comes from the URL, not from the body
public class UpdateStudentDto
{

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = "";

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = "";

    [Required]
    [MaxLength(100)]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; set; } = "";
}
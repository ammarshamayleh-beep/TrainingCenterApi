using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingCenterApi.DTOs;

// Request DTO: what the client sends IN to create a student (POST)
// No Id here: the database gives the Id
public class CreateStudentDto
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
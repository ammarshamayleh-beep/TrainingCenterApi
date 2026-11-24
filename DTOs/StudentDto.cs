using System;
using System.Collections.Generic;
using System.Text;
namespace TrainingCenterApi.DTOs;

// Response DTO: the shape of a student that goes OUT to the client
public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";

    // Not a column in the Students table: comes from the Profile navigation property
    public string? City { get; set; }
}
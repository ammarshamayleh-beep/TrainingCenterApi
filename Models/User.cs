using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TrainingCenterApi.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int UserId { get; set; }

    [MaxLength(100)]
    public string Email { get; set; } = "";

    // Never the real password: only its hash
    public string PasswordHash { get; set; } = "";

    [MaxLength(20)]
    public string Role { get; set; } = "Student";
}
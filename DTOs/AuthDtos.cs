using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.DTOs;

// What the client sends to create an account
public class RegisterDto
{
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";
}

// What the client sends to log in
public class LoginDto
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

// What the API returns after register or login
public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
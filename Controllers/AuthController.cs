using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterApi.Data;
using TrainingCenterApi.DTOs;
using TrainingCenterApi.Models;
using TrainingCenterApi.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TrainingContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(TrainingContext context, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        bool emailUsed = await _context.Users.AnyAsync(u => u.Email == dto.Email);

        if (emailUsed)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Email already used",
                detail: $"Email {dto.Email} is already used.");
        }

        // Everyone who registers is a Student. The client can't choose the role.
        var user = new User
        {
            Email = dto.Email,
            Role = "Student"
        };

        // Save the HASH of the password, never the password itself
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Role = user.Role
        });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // Same message for a wrong email and a wrong password,
        // so nobody can find out which emails exist
        if (user == null ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid email or password");
        }

        return Ok(new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Role = user.Role
        });
    }
    // GET api/auth/me
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        // "User" = the person from the token, filled in by UseAuthentication
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
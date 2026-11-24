using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterApi.Data;
using TrainingCenterApi.DTOs;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly TrainingContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(TrainingContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // How a Student entity becomes a StudentDto (translated to SQL with a LEFT JOIN)
    private static readonly Expression<Func<Student, StudentDto>> ToDto = s => new StudentDto
    {
        Id = s.StudentId,
        FirstName = s.FirstName,
        LastName = s.LastName,
        Email = s.Email,
        City = s.Profile != null ? s.Profile.City : null
    };
    // GET api/students
    // GET api/students?search=haddad
    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAll([FromQuery] string? search)
    {
        IQueryable<Student> query = _context.Students;

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Search in the first name OR the last name
            query = query.Where(s => s.FirstName.Contains(search) || s.LastName.Contains(search));
        }

        var students = await query
            .OrderBy(s => s.StudentId)
            .Select(ToDto)
            .ToListAsync();

        return Ok(students); // 200
    }

    // GET api/students/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById([FromRoute] int id)
    {
        var student = await _context.Students
            .Where(s => s.StudentId == id)
            .Select(ToDto)
            .FirstOrDefaultAsync();

        if (student == null)
        {
            return NotFound(); // 404
        }

        return Ok(student); // 200
    }

    // POST api/students
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
    {
        // The email is unique in the database, so check before saving
        bool emailUsed = await _context.Students.AnyAsync(s => s.Email == dto.Email);

        if (emailUsed)
        {
            _logger.LogWarning("Email {Email} is already used", dto.Email);

            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Email already used",
                detail: $"Email {dto.Email} is already used.");
        }

        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync(); // INSERT, then StudentId is filled in
        _logger.LogInformation("Student {StudentId} was created", student.StudentId);
        // Read it back as a DTO
        var result = await _context.Students
            .Where(s => s.StudentId == student.StudentId)
            .Select(ToDto)
            .FirstAsync();

        // 201 + a "location" header with the link to the new student
        return CreatedAtAction(nameof(GetById), new { id = student.StudentId }, result);
    }

    // PUT api/students/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return Problem(
    statusCode: StatusCodes.Status409Conflict,
    title: "Email already used",
    detail: $"Email {dto.Email} is already used.");
        }

        // Is this email used by ANOTHER student? The same student can keep his own email
        bool emailUsed = await _context.Students
            .AnyAsync(s => s.Email == dto.Email && s.StudentId != id);

        if (emailUsed)
        {
            return Conflict($"Email {dto.Email} is already used."); // 409
        }

        // Change Tracking: only the changed columns go into the UPDATE
        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.Email = dto.Email;

        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }

    // DELETE api/students/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound(); // 404
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync(); // His profile and enrollments are deleted too (Cascade)

        return NoContent(); // 204
    }
}
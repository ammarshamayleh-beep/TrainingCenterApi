using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.DTOs;
using TrainingCenterApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace TrainingCenterApi.Controllers;

// The controller only speaks HTTP: it reads the request, calls the service,
// and turns the answer into a status code.
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // GET api/courses?search=sql&minPrice=50&maxPrice=150&sortBy=price&desc=true&page=1&pageSize=5
    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDto>>> GetAll([FromQuery] CourseQuery query)
    {
        var result = await _courseService.GetAllAsync(query);
        return Ok(result); // 200
    }

    // GET api/courses/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById([FromRoute] int id)
    {
        var course = await _courseService.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound(); // 404
        }

        return Ok(course); // 200
    }

    // GET api/courses/5/details
    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<CourseDetailsDto>> GetDetails([FromRoute] int id)
    {
        var course = await _courseService.GetDetailsAsync(id);

        if (course == null)
        {
            return NotFound(); // 404
        }

        return Ok(course); // 200
    }

    // POST api/courses
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
    {
        var (status, course) = await _courseService.CreateAsync(dto);

        if (status == ServiceStatus.InstructorNotFound)
        {
            return InstructorNotFound(dto.InstructorId); // 400
        }

        return CreatedAtAction(nameof(GetById), new { id = course!.Id }, course); // 201
    }

    // PUT api/courses/5
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCourseDto dto)
    {
        var status = await _courseService.UpdateAsync(id, dto);

        if (status == ServiceStatus.NotFound)
        {
            return NotFound(); // 404
        }

        if (status == ServiceStatus.InstructorNotFound)
        {
            return InstructorNotFound(dto.InstructorId); // 400
        }

        return NoContent(); // 204
    }

    // DELETE api/courses/5
    // DELETE api/courses/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var status = await _courseService.DeleteAsync(id);

        if (status == ServiceStatus.NotFound)
        {
            return NotFound(); // 404
        }

        return NoContent(); // 204
    }

    private ObjectResult InstructorNotFound(int? instructorId)
    {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Instructor not found",
            detail: $"Instructor {instructorId} was not found.");
    }
}
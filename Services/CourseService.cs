using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TrainingCenterApi.Data;
using TrainingCenterApi.DTOs;
using TrainingCenterApi.Mappings;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Services;

// The business logic for courses. It knows the database, but nothing about HTTP.
public class CourseService : ICourseService
{
    private readonly TrainingContext _context;

    public CourseService(TrainingContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CourseDto>> GetAllAsync(CourseQuery q)
    {
        IQueryable<Course> query = _context.Courses;

        // 1. Filtering
        string? search = q.Search;
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Title.Contains(search));
        }

        if (q.MinPrice != null)
        {
            decimal minPrice = q.MinPrice.Value;
            query = query.Where(c => c.Price >= minPrice);
        }

        if (q.MaxPrice != null)
        {
            decimal maxPrice = q.MaxPrice.Value;
            query = query.Where(c => c.Price <= maxPrice);
        }

        // 2. Sorting
        string? sortBy = q.SortBy?.ToLower();

        if (sortBy == "title")
        {
            query = q.Desc ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title);
        }
        else if (sortBy == "price")
        {
            query = q.Desc ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price);
        }
        else
        {
            query = query.OrderBy(c => c.CourseId);
        }

        // 3. Pagination
        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(CourseMappings.ToDto)
            .ToListAsync();

        return new PagedResult<CourseDto>
        {
            Items = items,
            Page = q.Page,
            PageSize = q.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .Where(c => c.CourseId == id)
            .Select(CourseMappings.ToDto)
            .FirstOrDefaultAsync();
    }

    public async Task<CourseDetailsDto?> GetDetailsAsync(int id)
    {
        return await _context.Courses
            .Where(c => c.CourseId == id)
            .Select(c => new CourseDetailsDto
            {
                Id = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,

                Instructor = c.Instructor == null ? null : new InstructorSummaryDto
                {
                    Id = c.Instructor.InstructorId,
                    FullName = c.Instructor.FirstName + " " + c.Instructor.LastName
                },

                Students = c.Enrollments
                    .OrderBy(e => e.Student!.FirstName)
                    .Select(e => new EnrolledStudentDto
                    {
                        Id = e.StudentId,
                        FullName = e.Student!.FirstName + " " + e.Student!.LastName,
                        ProgressPercent = e.ProgressPercent,
                        EnrollmentDate = e.EnrollmentDate
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(ServiceStatus Status, CourseDto? Course)> CreateAsync(CreateCourseDto dto)
    {
        // Business rule: if a course has an instructor, the instructor must exist
        if (!await InstructorExistsAsync(dto.InstructorId))
        {
            return (ServiceStatus.InstructorNotFound, null);
        }

        var course = dto.ToEntity();

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(course.CourseId);

        return (ServiceStatus.Success, result);
    }

    public async Task<ServiceStatus> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return ServiceStatus.NotFound;
        }

        if (!await InstructorExistsAsync(dto.InstructorId))
        {
            return ServiceStatus.InstructorNotFound;
        }

        dto.ApplyTo(course);
        await _context.SaveChangesAsync();

        return ServiceStatus.Success;
    }

    public async Task<ServiceStatus> DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return ServiceStatus.NotFound;
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return ServiceStatus.Success;
    }

    private async Task<bool> InstructorExistsAsync(int? instructorId)
    {
        if (instructorId == null)
        {
            return true;
        }

        return await _context.Instructors.AnyAsync(i => i.InstructorId == instructorId);
    }
}
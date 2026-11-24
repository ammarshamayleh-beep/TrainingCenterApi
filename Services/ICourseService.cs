using TrainingCenterApi.DTOs;

namespace TrainingCenterApi.Services;

// What happened when we tried to change data
public enum ServiceStatus
{
    Success,
    NotFound,
    InstructorNotFound
}

// The contract: everything the API can do with courses
public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetAllAsync(CourseQuery query);
    Task<CourseDto?> GetByIdAsync(int id);                 // null = not found
    Task<CourseDetailsDto?> GetDetailsAsync(int id);       // null = not found
    Task<(ServiceStatus Status, CourseDto? Course)> CreateAsync(CreateCourseDto dto);
    Task<ServiceStatus> UpdateAsync(int id, UpdateCourseDto dto);
    Task<ServiceStatus> DeleteAsync(int id);
}
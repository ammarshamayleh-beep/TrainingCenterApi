using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.DTOs;

// All the options the client can send in the query string
public class CourseQuery
{
    // Filtering
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Sorting: "title" or "price"
    public string? SortBy { get; set; }
    public bool Desc { get; set; }

    // Pagination
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;
}
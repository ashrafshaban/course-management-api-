using System.ComponentModel.DataAnnotations;
using CourseManagement.Api.Models.Enums;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: CRS-01 to CRS-08.</summary>
public class Course
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public CourseLevel Level { get; set; }

    public string? ThumbnailUrl { get; set; }

    public CourseStatus Status { get; set; } = CourseStatus.Draft;

    public Guid InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

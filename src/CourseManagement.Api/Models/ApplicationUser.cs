using System.ComponentModel.DataAnnotations;
using CourseManagement.Api.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Api.Models;

/// <summary>
/// Application user backed by ASP.NET Core Identity (ADR-003).
/// Domain fields match data-model.md User; Identity supplies Id, Email, PasswordHash.
/// Satisfies: AUTH-01 to AUTH-07.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Mirrored in Identity AspNetRoles for [Authorize(Roles)] claims.</summary>
    public UserRole Role { get; set; } = UserRole.Student;

    public string? AvatarUrl { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Course> InstructedCourses { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}

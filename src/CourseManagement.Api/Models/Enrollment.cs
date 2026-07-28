using CourseManagement.Api.Models.Enums;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: ENR-01 to ENR-05.</summary>
public class Enrollment
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}

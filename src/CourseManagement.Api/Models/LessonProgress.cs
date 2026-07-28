namespace CourseManagement.Api.Models;

/// <summary>Satisfies: PRG-01 to PRG-05. Progress % is computed, not stored.</summary>
public class LessonProgress
{
    public Guid Id { get; set; }

    public Guid EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public DateTime? CompletedAt { get; set; }
}

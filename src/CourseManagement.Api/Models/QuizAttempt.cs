using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: QZ-03 to QZ-06. Score/Passed computed server-side at submission.</summary>
public class QuizAttempt
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    [Range(0, 100)]
    public int Score { get; set; }

    public bool Passed { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}

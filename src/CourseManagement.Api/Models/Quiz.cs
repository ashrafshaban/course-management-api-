using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: QZ-01.</summary>
public class Quiz
{
    public Guid Id { get; set; }

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Passing threshold 0–100 (percentage).</summary>
    [Range(0, 100)]
    public int PassingScore { get; set; }

    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

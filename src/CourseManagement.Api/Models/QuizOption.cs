using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: QZ-02. Exactly one IsCorrect=true per question — enforced in service layer.</summary>
public class QuizOption
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public QuizQuestion Question { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: QZ-02.</summary>
public class QuizQuestion
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}

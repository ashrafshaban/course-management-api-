using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: CNT-01, CNT-03, CNT-04.</summary>
public class Module
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}

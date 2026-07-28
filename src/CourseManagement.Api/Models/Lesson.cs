using System.ComponentModel.DataAnnotations;
using CourseManagement.Api.Models.Enums;

namespace CourseManagement.Api.Models;

/// <summary>Satisfies: CNT-02, CNT-03, CNT-04, CNT-05.</summary>
public class Lesson
{
    public Guid Id { get; set; }

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public ContentType ContentType { get; set; }

    [Required]
    public string Body { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}

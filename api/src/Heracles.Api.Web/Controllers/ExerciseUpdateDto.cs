using System.ComponentModel.DataAnnotations;
using Heracles.Core.Entities;

namespace Heracles.Web.Controllers;

public class ExerciseUpdateDto
{
    [MaxLength(Exercise.NameLength)]
    public string? Name { get; set; }

    [MaxLength(Exercise.DescriptionLength)]
    public string? Description { get; set; }

    public ExerciseCategory? Category { get; init; }
    public ExerciseBodyPart? BodyPart { get; init; }
}

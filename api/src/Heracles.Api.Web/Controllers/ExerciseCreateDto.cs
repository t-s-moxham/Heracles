using System.ComponentModel.DataAnnotations;
using Heracles.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Heracles.Web.Controllers;

public class ExerciseCreateDto
{
    [MaxLength(Exercise.NameLength)]
    public required string Name { get; init; }

    [MaxLength(Exercise.DescriptionLength)]
    public string? Description { get; init; }

    public ExerciseCategory Category { get; init; } = ExerciseCategory.None;

    public ExerciseBodyPart BodyPart { get; init; } = ExerciseBodyPart.None;
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Heracles.Core.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExerciseCategory
{
    None,

    Machine,
    MachinePlateLoaded,
    Barbell,
    Dumbbell,

    Duration, // or cardio
    BodyWeight,
    AssistedBodyWeight,

    Other,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExerciseBodyPart
{
    None,
    Legs,
    Arms,
    Core,
    Cardio,
    Back,
    FullBody,
}

public class Exercise : PublicEntity
{
    public const int NameLength = 100;
    public const int DescriptionLength = 200;

    [MaxLength(NameLength)]
    public required string Name { get; set; }

    [MaxLength(DescriptionLength)]
    public string? Description { get; set; }

    public ExerciseCategory Category { get; set; } = ExerciseCategory.None;
    public ExerciseBodyPart BodyPart { get; set; } = ExerciseBodyPart.None;
}

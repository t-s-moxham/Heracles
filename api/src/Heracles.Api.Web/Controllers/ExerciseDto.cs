using Heracles.Core.Entities;

namespace Heracles.Web.Controllers;

public record ExerciseDto(
    Guid Id,
    string Name,
    string? Description,
    ExerciseCategory Category,
    ExerciseBodyPart BodyPart,
    bool IsOfficial
) { }

public static class ExerciseDtoFactory
{
    public static ExerciseDto Create(Exercise exercise)
    {
        return new ExerciseDto(
            Id: exercise.Id,
            Name: exercise.Name,
            Description: exercise.Description,
            Category: exercise.Category,
            BodyPart: exercise.BodyPart,
            IsOfficial: exercise.IsOfficial
        );
    }
}

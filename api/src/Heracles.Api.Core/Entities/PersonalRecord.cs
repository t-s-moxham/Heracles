namespace Heracles.Core.Entities;

public class PersonalRecord : EntityBase
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid PerformedExerciseId { get; set; }
    public PerformedExercise PerformedExercise { get; set; } = null!;
}

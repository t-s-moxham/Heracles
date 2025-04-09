namespace Heracles.Core.Entities;

public class ExerciseSet : EntityBase
{
    public Guid PerformedExerciseId { get; set; }
    public PerformedExercise PerformedExercise { get; set; } = null!;

    public int Position { get; set; }
    public int Repetition { get; set; }
    public float Weight { get; set; }
    public int Duration { get; set; }
}

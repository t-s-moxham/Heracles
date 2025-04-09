namespace Heracles.Core.Entities;

public enum WeightUnit
{
    Kilograms,
    Pounds,
}

public class PerformedExercise : EntityBase
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid? WorkoutId { get; set; }
    public Workout? Workout { get; set; }

    public string Note { get; set; } = "";
    public string Variant { get; set; } = "";

    /// <summary>
    /// Display units for client use only (all weights in kg)
    /// </summary>
    public WeightUnit WeightDisplayUnits { get; set; }
}

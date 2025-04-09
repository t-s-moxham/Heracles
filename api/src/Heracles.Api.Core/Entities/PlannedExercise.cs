using Microsoft.EntityFrameworkCore;

namespace Heracles.Core.Entities;

[PrimaryKey(nameof(ExerciseId), nameof(TemplateId), nameof(Position))]
public class PlannedExercise : PublicEntity
{
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;

    public int Position { get; set; }

    public string Variant { get; set; } = "";

    public int NumberOfSets { get; set; }
    public int MinimumRepetitions { get; set; }
    public int MaximumRepetitions { get; set; }
}

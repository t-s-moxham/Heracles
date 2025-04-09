using Heracles.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heracles.Infrastructure.Config;

public class ExerciseEntityTypeConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder
            .Property(exercise => exercise.Category)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ExerciseCategory>(v));

        builder
            .Property(exercise => exercise.BodyPart)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ExerciseBodyPart>(v));

        builder.HasData(
            new Exercise()
            {
                Id = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Name = "Angled Leg Press",
                Category = ExerciseCategory.MachinePlateLoaded,
                BodyPart = ExerciseBodyPart.Legs,
                IsOfficial = true,
            },
            new Exercise()
            {
                Id = new Guid("123e4567-e89b-12d3-a456-426614174001"),
                Name = "Romanian Deadlift",
                Category = ExerciseCategory.Barbell,
                BodyPart = ExerciseBodyPart.Legs,
                IsOfficial = true,
            }
        );
    }
}

using Heracles.Core.Entities;
using Heracles.Infrastructure;
using Heracles.Web.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Web.Services;

public class ExerciseService(AppDbContext dbContext)
{
    public async Task<Exercise?> FindExerciseById(Guid id)
    {
        return await dbContext.Exercises.FindAsync(id);
    }

    public async Task<List<Exercise>> GetAllExercises(Guid userId)
    {
        return await dbContext
            .Exercises.Where(exercise => (exercise.UserId == userId) || exercise.IsOfficial)
            .ToListAsync();
    }

    public async Task<Exercise> AddExercise(Guid userId, ExerciseCreateDto exerciseCreateDto)
    {
        var exercise = new Exercise()
        {
            UserId = userId,
            Name = exerciseCreateDto.Name,
            Description = exerciseCreateDto.Description,
            Category = exerciseCreateDto.Category,
            BodyPart = exerciseCreateDto.BodyPart,
        };

        await dbContext.Exercises.AddAsync(exercise);
        await dbContext.SaveChangesAsync();

        return exercise;
    }

    public async Task<bool> ExerciseExists(Guid userId, string name, ExerciseCategory category)
    {
        return await dbContext
            .Exercises.Where(e => e.UserId == userId)
            .Where(e => e.Name == name)
            .Where(e => e.Category == category)
            .AnyAsync();
    }

    public async Task<bool> CanDelete(Guid exerciseId)
    {
        var inTemplates = dbContext.PlannedExercises.AnyAsync(p => p.ExerciseId == exerciseId);

        var inWorkouts = dbContext.PerformedExercises.AnyAsync(p => p.ExerciseId == exerciseId);

        var results = await Task.WhenAll(inTemplates, inWorkouts);

        return !results.Any(r => r);
    }
}

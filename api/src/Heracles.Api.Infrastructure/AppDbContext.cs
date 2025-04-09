using System.Reflection;
using Heracles.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure;

public class AppDbContext(DbContextOptions options)
    : IdentityDbContext<AppIdentityUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<PerformedExercise> PerformedExercises { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<PersonalRecord> PersonalRecords { get; set; }
    public DbSet<PlannedExercise> PlannedExercises { get; set; }
    public DbSet<ExerciseSet> ExerciseSets { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var addedUsers = ChangeTracker
            .Entries<AppIdentityUser>()
            .Where(e => e.State == EntityState.Added);

        var profilesToAdd = addedUsers.Select(user => new Profile(user.Entity.Id)).ToList();
        await Profiles.AddRangeAsync(profilesToAdd, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}

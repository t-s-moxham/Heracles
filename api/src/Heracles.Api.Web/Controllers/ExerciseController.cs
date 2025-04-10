using Heracles.Core.Entities;
using Heracles.Infrastructure;
using Heracles.Web.auth;
using Heracles.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Web.Controllers;

[Route("exercises")]
[Authorize]
public class ExerciseController(
    ExerciseService exerciseService,
    IAuthorizationService authorizationService,
    AppDbContext dbContext
) : CustomControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ExerciseDto>>> GetAllExercises()
    {
        var exercises = await exerciseService.GetAllExercises(CurrentUserId);
        return Ok(exercises.Select(ExerciseDtoFactory.Create));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(Guid id)
    {
        var exercise = await exerciseService.FindExerciseById(id);

        if (exercise == null)
        {
            return NotFound();
        }

        var authResult = await authorizationService.AuthorizeAsync(
            User,
            exercise,
            CrudRequirements.Read
        );

        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return Ok(ExerciseDtoFactory.Create(exercise));
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> PostExercise(ExerciseCreateDto exerciseCreateDto)
    {
        // don't create exercises with same name and category owned by the User

        var exerciseExists = await exerciseService.ExerciseExists(
            CurrentUserId,
            exerciseCreateDto.Name,
            exerciseCreateDto.Category
        );

        if (exerciseExists)
        {
            return BadRequest("Exercise with this name already exists");
        }

        var exercise = await exerciseService.AddExercise(CurrentUserId, exerciseCreateDto);

        return CreatedAtAction(
            nameof(GetExercise),
            new { id = exercise.Id },
            ExerciseDtoFactory.Create(exercise)
        );
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> UpdateExercise(
        Guid id,
        ExerciseUpdateDto exerciseUpdate
    )
    {
        var exercise = await exerciseService.FindExerciseById(id);

        if (exercise == null)
        {
            return NotFound();
        }

        var authResult = await authorizationService.AuthorizeAsync(
            User,
            exercise,
            CrudRequirements.Update
        );

        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        // update non-key fields

        exercise.Description = exerciseUpdate.Description ?? exercise.Description;
        exercise.BodyPart = exerciseUpdate.BodyPart ?? exercise.BodyPart;

        var alternateKeyUnchanged =
            (exerciseUpdate.Name == null || exercise.Name == exerciseUpdate.Name)
            && (exerciseUpdate.Category == null || exercise.Category == exerciseUpdate.Category);

        if (alternateKeyUnchanged)
        {
            await dbContext.SaveChangesAsync();
            return Ok(ExerciseDtoFactory.Create(exercise));
        }

        // update alternate key fields

        exercise.Name = exerciseUpdate.Name ?? exercise.Name;
        exercise.Category = exerciseUpdate.Category ?? exercise.Category;

        var alreadyExists = await exerciseService.ExerciseExists(
            CurrentUserId,
            exercise.Name,
            exercise.Category
        );

        if (alreadyExists)
        {
            return BadRequest("Exercise with this name and category already exists");
        }

        await dbContext.SaveChangesAsync();
        return Ok(ExerciseDtoFactory.Create(exercise));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> DeleteExercise(Guid id)
    {
        var exercise = await exerciseService.FindExerciseById(id);

        if (exercise == null)
        {
            return NotFound();
        }

        var authResult = await authorizationService.AuthorizeAsync(
            User,
            exercise,
            CrudRequirements.Delete
        );

        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        // TODO complete implementation

        return NoContent();
    }
}

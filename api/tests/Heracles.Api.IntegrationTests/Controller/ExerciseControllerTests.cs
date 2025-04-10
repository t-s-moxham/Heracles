using System.Net;
using Heracles.Core.Entities;
using Heracles.Web.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Heracles.IntegrationTests.Controller;

public class ExerciseControllerTests(TestEnvironment test) : IClassFixture<TestEnvironment>
{
    private async Task<HttpResponseMessage> CreateTestExercise(string name)
    {
        var exerciseCreateDto = new ExerciseCreateDto() { Name = name };

        var response = await test.AuthenticatedClient.PostAsJsonAsync(
            "/exercises",
            exerciseCreateDto
        );

        return response;
    }

    private static async Task<ExerciseDto> GetExerciseFromResponse(HttpResponseMessage response)
    {
        var exerciseDto = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        Assert.NotNull(exerciseDto);

        return exerciseDto;
    }

    [Fact]
    [Trait("Case", "Default")]
    public async Task CreateExercise_ReturnsNewlyCreatedExercise()
    {
        var name = Guid.NewGuid().ToString();
        var response = await CreateTestExercise(name);
        var dto = await GetExerciseFromResponse(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(name, dto.Name);
    }

    [Fact]
    public async Task CreateExercise_SameNameAndCategory()
    {
        var name = Guid.NewGuid().ToString();
        await CreateTestExercise(name);

        var response = await CreateTestExercise(name);
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Exercise with this name already exists", message);
    }

    [Fact]
    public async Task CreateExercise_SameNameDifferentCategory()
    {
        var name = Guid.NewGuid().ToString();
        await CreateTestExercise(name);

        var response = await test.AuthenticatedClient.PostAsJsonAsync(
            "/exercises",
            new ExerciseCreateDto() { Name = name, Category = ExerciseCategory.Barbell }
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    [Trait("Case", "Default")]
    public async Task GetExercise_ReturnsNewlyCreatedExercise()
    {
        var createResponse = await CreateTestExercise(Guid.NewGuid().ToString());
        var createdExerciseDto = await GetExerciseFromResponse(createResponse);

        var response = await test.AuthenticatedClient.GetAsync(
            $"/exercises/{createdExerciseDto.Id}"
        );
        var fetchedExerciseDto = await response.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(fetchedExerciseDto);
        Assert.Equal(fetchedExerciseDto.Id, createdExerciseDto.Id);
    }

    [Fact]
    public async Task GetExercise_GetOfficialExercise()
    {
        // test getting an official exercise, user should be able to retrieve personal as well as
        // official exercises

        var officialExercise = await test
            .DbContext.Exercises.Where(e => e.IsOfficial)
            .FirstOrDefaultAsync();

        Assert.NotNull(officialExercise);

        var response = await test.AuthenticatedClient.GetAsync($"/exercises/{officialExercise.Id}");

        var exerciseDto = await GetExerciseFromResponse(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(officialExercise.Id, exerciseDto.Id);
    }

    [Fact]
    public async Task GetExercise_OtherUsersExercise()
    {
        var name = Guid.NewGuid().ToString();
        var createResponse = await CreateTestExercise(name);
        var dto = await GetExerciseFromResponse(createResponse);

        var otherUserClient = await test.GenerateFakeUserClient();

        var response = await otherUserClient.GetAsync($"/exercises/{dto.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    [Trait("Case", "Default")]
    public async Task UpdateExercise_ReturnsUpdatedExercise()
    {
        // create exercise

        var name = Guid.NewGuid().ToString();
        var createResponse = await CreateTestExercise(name);
        var createDto = await GetExerciseFromResponse(createResponse);

        // update exercise

        var newName = Guid.NewGuid().ToString();
        const ExerciseCategory category = ExerciseCategory.Barbell;
        const string description = "description";
        const ExerciseBodyPart bodyPart = ExerciseBodyPart.Arms;

        var response = await test.AuthenticatedClient.PatchAsJsonAsync(
            $"/exercises/{createDto.Id}",
            new ExerciseUpdateDto()
            {
                Name = newName,
                Category = category,
                Description = description,
                BodyPart = bodyPart,
            }
        );

        var responseDto = await GetExerciseFromResponse(response);

        // Assertions

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(createDto.Id, responseDto.Id);
        Assert.Equal(category, responseDto.Category);
        Assert.Equal(description, responseDto.Description);
        Assert.Equal(bodyPart, responseDto.BodyPart);
    }

    [Fact]
    public async Task UpdateExercise_OfficialExercise()
    {
        // Retrieve an official exercise
        var officialExercise = await test
            .DbContext.Exercises.Where(e => e.IsOfficial)
            .FirstOrDefaultAsync();

        Assert.NotNull(officialExercise);

        // Attempt to update the official exercise
        var response = await test.AuthenticatedClient.PatchAsJsonAsync(
            $"/exercises/{officialExercise.Id}",
            new ExerciseUpdateDto() { Name = "UpdatedName" }
        );

        // Assert that the update is forbidden
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExercise_OtherUsersExercise()
    {
        // Create an exercise as the authenticated user
        var name = Guid.NewGuid().ToString();
        var createResponse = await CreateTestExercise(name);
        var createDto = await GetExerciseFromResponse(createResponse);

        // Generate a client for another user
        var otherUserClient = await test.GenerateFakeUserClient();

        // Attempt to update the exercise as the other user
        var response = await otherUserClient.PatchAsJsonAsync(
            $"/exercises/{createDto.Id}",
            new ExerciseUpdateDto() { Name = "UpdatedName" }
        );

        // Assert that the update is forbidden
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExercise_NameCategoryConflict()
    {
        // Create the first exercise
        var name = Guid.NewGuid().ToString();
        await CreateTestExercise(name);

        // Create a second exercise
        var secondExerciseName = Guid.NewGuid().ToString();
        var createResponse = await CreateTestExercise(secondExerciseName);
        var secondExerciseDto = await GetExerciseFromResponse(createResponse);

        // Attempt to update the second exercise to have the same name and category as the first exercise
        var response = await test.AuthenticatedClient.PatchAsJsonAsync(
            $"/exercises/{secondExerciseDto.Id}",
            new ExerciseUpdateDto() { Name = name }
        );
        var message = await response.Content.ReadAsStringAsync();

        // Assert that the update is rejected due to conflict
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Exercise with this name and category already exists", message);
    }

    // TODO write tests for deletion methods

    // [Fact]
    // public async Task DeleteExercise_NewlyCreatedExercise() { }

    // [Fact]
    // public async Task DeleteExercise_OfficialExercise() { }

    // [Fact]
    // public async Task DeleteExercise_OtherUsersExercise() { }

    // [Fact]
    // public async Task DeleteExercise_InTemplate() { }
}

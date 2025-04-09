using System.Net;
using Heracles.Core.Entities;
using Heracles.Web.Controllers;

namespace Heracles.IntegrationTests.ExerciseController;

public class CreateExerciseTests(TestEnvironment test) : IClassFixture<TestEnvironment>
{
    private async Task<HttpResponseMessage> CreateTestExercise(string exerciseName)
    {
        var exerciseCreateDto = new ExerciseCreateDto() { Name = exerciseName };

        var response = await test.AuthenticatedClient.PostAsJsonAsync(
            "/exercises",
            exerciseCreateDto
        );

        return response;
    }

    private async Task<ExerciseDto> GetExerciseFromResponse(HttpResponseMessage response)
    {
        var exerciseDto = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        Assert.NotNull(exerciseDto);

        return exerciseDto;
    }

    [Fact]
    public async Task CreateExercise_ReturnsNewlyCreatedExercise()
    {
        var name = Guid.NewGuid().ToString();
        var response = await CreateTestExercise(name);
        var dto = await GetExerciseFromResponse(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(name, dto.Name);
    }

    [Fact]
    public async Task CreateExercise_SameNameAndCategory_Fails()
    {
        var name = Guid.NewGuid().ToString();
        await CreateTestExercise(name);

        var response = await CreateTestExercise(name);
        var message = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Exercise with this name already exists", message);
    }

    [Fact]
    public async Task CreateExercise_SameNameDifferentCategory_Succeeds()
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
    public async Task GetExercise_GetOfficialExercise_Succeeds()
    {
        var response = await test.AuthenticatedClient.GetAsync(
            "/exercises/123e4567-e89b-12d3-a456-426614174000"
        );

        var exerciseDto = await GetExerciseFromResponse(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

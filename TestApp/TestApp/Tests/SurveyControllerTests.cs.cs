using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TestApp.Data;
using TestApp.Models;

public class SurveyControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "test")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateQuestion_ShouldAddQuestion()
    {
        var context = GetDbContext();
        var controller = new SurveyController(context);

        var question = new SurveyQuestion
        {
            SurveyId = 1,
            QuestionText = "Test?",
            QuestionType = "text"
        };

        var result = await controller.CreateQuestion(question);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, context.SurveyQuestions.Count());
    }

    [Fact]
    public async Task SubmitResponse_ShouldAddResponse()
    {
        var context = GetDbContext();
        var controller = new SurveyController(context);

        var response = new SurveyResponse
        {
            SurveyId = 1,
            UserId = 1,
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Very Satisfied" },
                new SurveyResponseAnswer { QuestionId = 2, Answer = "Speed, Usability" },
                new SurveyResponseAnswer { QuestionId = 3, Answer = "Great product, would buy again!" }
            }
        };

        var result = await controller.SubmitResponse(response);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, context.SurveyResponses.Count());
        Assert.Equal(3, context.SurveyResponseAnswers.Count());
    }

    [Fact]
    public async Task GetResponse_ShouldReturnResponse()
    {
        var context = GetDbContext();
        var controller = new SurveyController(context);

        var response = new SurveyResponse
        {
            SurveyId = 1,
            UserId = 1,
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Very Satisfied" }
            }
        };
        context.SurveyResponses.Add(response);
        await context.SaveChangesAsync();
        response.Answers[0].SurveyResponseId = response.Id;
        context.SurveyResponseAnswers.Add(response.Answers[0]);
        await context.SaveChangesAsync();

        var result = await controller.GetResponse(response.Id) as OkObjectResult;
        var returned = result?.Value as SurveyResponse;

        Assert.NotNull(returned);
        Assert.Equal(response.Id, returned.Id);
    }

    [Fact]
    public async Task EditResponse_ShouldCreateNewVersion()
    {
        var context = GetDbContext();
        var controller = new SurveyController(context);

        var original = new SurveyResponse
        {
            SurveyId = 1,
            UserId = 1,
            Version = 1,
            SubmittedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "A" }
            }
        };
        context.SurveyResponses.Add(original);
        await context.SaveChangesAsync();
        original.Answers[0].SurveyResponseId = original.Id;
        context.SurveyResponseAnswers.Add(original.Answers[0]);
        await context.SaveChangesAsync();

        var editPayload = new SurveyResponse
        {
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "B" }
            }
        };

        var result = await controller.EditResponse(original.Id, editPayload) as OkObjectResult;
        var newResponse = result?.Value as SurveyResponse;

        Assert.NotNull(newResponse);
        Assert.NotEqual(original.Id, newResponse.Id);
        Assert.Equal(original.Version + 1, newResponse.Version);
        Assert.Single(newResponse.Answers);
        Assert.Equal("B", newResponse.Answers.First().Answer);

        Assert.Equal(2, context.SurveyResponses.Count());
    }

    [Fact]
    public async Task ListResponsesByUser_ShouldReturnResponses()
    {
        var context = GetDbContext();
        var controller = new SurveyController(context);

        context.SurveyResponses.Add(new SurveyResponse
        {
            SurveyId = 1,
            UserId = 2,
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Satisfied" }
            }
        });
        await context.SaveChangesAsync();

        var result = await controller.ListResponsesByUser(2) as OkObjectResult;
        var list = result?.Value as List<SurveyResponse>;

        Assert.NotNull(list);
        Assert.NotEmpty(list);
        Assert.All(list, r => Assert.Equal(2, r.UserId));
    }
}
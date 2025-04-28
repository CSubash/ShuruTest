using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Very Satisfied" },
                new SurveyResponseAnswer { QuestionId = 2, Answer = "Speed, Usability" },
                new SurveyResponseAnswer { QuestionId = 3, Answer = "Great product, would buy again!" }
            }
        };
        context.SurveyResponses.Add(response);
        await context.SaveChangesAsync();

        var result = await controller.GetResponse(response.Id);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateResponse_ShouldIncrementVersion()
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
        context.SurveyResponses.Add(response);
        await context.SaveChangesAsync();

        var updated = new SurveyResponse
        {
            Answers = new List<SurveyResponseAnswer>
            {
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Neutral" },
                new SurveyResponseAnswer { QuestionId = 2, Answer = "Design, Price" }
            }
        };
        var result = await controller.UpdateResponse(response.Id, updated);

        Assert.IsType<OkObjectResult>(result);
        var updatedResponse = await context.SurveyResponses.FindAsync(response.Id);
        Assert.Equal(2, updatedResponse!.Version);
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
                new SurveyResponseAnswer { QuestionId = 1, Answer = "Satisfied" },
                new SurveyResponseAnswer { QuestionId = 2, Answer = "Speed, Usability" }
            }
        });
        await context.SaveChangesAsync();

        var result = await controller.ListResponsesByUser(2);

        Assert.IsType<OkObjectResult>(result);
    }
}
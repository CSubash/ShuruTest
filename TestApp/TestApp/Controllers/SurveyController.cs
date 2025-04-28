using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApp.Data;
using TestApp.Models;


[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly AppDbContext _context;

    public SurveyController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("questions")]
    public async Task<IActionResult> CreateQuestion([FromBody] SurveyQuestion question)
    {
        _context.SurveyQuestions.Add(question);
        await _context.SaveChangesAsync();
        return Ok(question);
    }

    [HttpPost("responses")]
    public async Task<IActionResult> SubmitResponse([FromBody] SurveyResponse response)
    {
        response.SubmittedAt = DateTime.UtcNow;
        response.UpdatedAt = DateTime.UtcNow;

        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        foreach (var answer in response.Answers)
        {
            answer.SurveyResponseId = response.Id;
            _context.SurveyResponseAnswers.Add(answer);
        }
        await _context.SaveChangesAsync();
        return Ok(response);
    }

    [HttpGet("responses")]
    public async Task<IActionResult> GetAllResponses()
    {
        var responses = await _context.SurveyResponses
                                      .Include(r => r.Answers)
                                      .ToListAsync();

        if (responses == null || responses.Count == 0)
        {
            return NotFound("No responses found.");
        }

        return Ok(responses);
    }

    [HttpGet("responses/{responseId}")]
    public async Task<IActionResult> GetResponse(int responseId)
    {
        var response = await _context.SurveyResponses.FindAsync(responseId);
        if (response == null) return NotFound();
        return Ok(response);
    }

    [HttpPost("responses/{responseId}/edit")]
    public async Task<IActionResult> EditResponse(int responseId, [FromBody] SurveyResponse updated)
    {
        var original = await _context.SurveyResponses
            .Include(r => r.Answers)
            .FirstOrDefaultAsync(r => r.Id == responseId);
        if (original == null) return NotFound();

        var newResponse = new SurveyResponse
        {
            SurveyId = original.SurveyId,
            UserId = original.UserId,
            Version = original.Version + 1,
            SubmittedAt = original.SubmittedAt,
            UpdatedAt = DateTime.UtcNow,
            Answers = updated.Answers
        };
        _context.SurveyResponses.Add(newResponse);
        await _context.SaveChangesAsync();

        foreach (var ans in newResponse.Answers)
        {
            ans.SurveyResponseId = newResponse.Id;
            _context.SurveyResponseAnswers.Add(ans);
        }
        await _context.SaveChangesAsync();

        return Ok(newResponse);
    }

    [HttpGet("user/{userId}/responses")]
    public async Task<IActionResult> ListResponsesByUser(int userId)
    {
        var responses = await _context.SurveyResponses
            .Where(r => r.UserId == userId)
            .ToListAsync();
        return Ok(responses);
    }
}
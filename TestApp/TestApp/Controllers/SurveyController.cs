using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
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

    [HttpPut("responses/{responseId}")]
    public async Task<IActionResult> UpdateResponse(int responseId, [FromBody] SurveyResponse updated)
    {
        var response = await _context.SurveyResponses.FindAsync(responseId);
        if (response == null) return NotFound();

        response.Answers = updated.Answers;
        response.UpdatedAt = DateTime.UtcNow;
        response.Version += 1;

        await _context.SaveChangesAsync();
        return Ok(response);
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoraBackend.Data;
using QuoraBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class VoteController : ControllerBase
{
    private readonly AppDbContext _context;

    public VoteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromQuery] int userId, [FromQuery] int questionId, [FromQuery] int value)
    {
        // ✅ Validate vote value
        if (value != 1 && value != -1)
            return BadRequest("Value must be 1 (upvote) or -1 (downvote)");

        // ✅ Check if question exists
        var questionExists = await _context.Questions.AnyAsync(q => q.Id == questionId);
        if (!questionExists)
            return NotFound("Question not found");

        // ✅ Check existing vote
        var existing = await _context.Votes
            .FirstOrDefaultAsync(v => v.UserId == userId && v.QuestionId == questionId);

        if (existing != null)
        {
            // 🔁 Toggle behavior
            if (existing.Value == value)
            {
                _context.Votes.Remove(existing); // remove vote if clicked again
            }
            else
            {
                existing.Value = value; // switch vote
            }
        }
        else
        {
            _context.Votes.Add(new Vote
            {
                UserId = userId,
                QuestionId = questionId,
                Value = value
            });
        }

        await _context.SaveChangesAsync();

        // ✅ Return updated vote count
        var totalVotes = await _context.Votes
            .Where(v => v.QuestionId == questionId)
            .SumAsync(v => v.Value);

        return Ok(new { votes = totalVotes });
    }
}

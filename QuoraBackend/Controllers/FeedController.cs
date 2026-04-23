using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoraBackend.Data;

namespace QuoraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFeed(int userId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // ✅ 1. Preload votes (optimized)
            var votesGrouped = await _context.Votes
                .GroupBy(v => v.QuestionId)
                .Select(g => new
                {
                    QuestionId = g.Key,
                    TotalVotes = g.Sum(v => v.Value)
                })
                .ToListAsync();

            var votesDict = votesGrouped
                .ToDictionary(v => v.QuestionId, v => v.TotalVotes);

            // ✅ 2. Get feed (ONLY followed users + self)
            var feed = await (
                from q in _context.Questions
                join u in _context.Users on q.UserId equals u.Id

                where
                    (
                        _context.Follows.Any(f =>
                            f.FollowerId == userId &&
                            f.FollowingId == q.UserId
                        )
                        || q.UserId == userId
                    )

                // ✅ filter real users only
                where !string.IsNullOrWhiteSpace(u.Name)
                      && u.Name.ToLower() != "user"

                orderby q.CreatedAt descending

                select new
                {
                    q.Id,
                    q.Content,

                    ImageUrl = string.IsNullOrEmpty(q.ImageUrl)
                        ? null
                        : baseUrl + "/" + q.ImageUrl,

                    q.CreatedAt,

                    UserId = u.Id,
                    UserName = u.Name,
                    UserAvatar = u.Avatar
                }
            ).ToListAsync();

            // ✅ 3. Attach votes efficiently
            var finalFeed = feed.Select(post => new
            {
                post.Id,
                post.Content,
                post.ImageUrl,
                post.CreatedAt,
                post.UserId,
                post.UserName,
                post.UserAvatar,

                Votes = votesDict.ContainsKey(post.Id)
                    ? votesDict[post.Id]
                    : 0
            });

            return Ok(finalFeed);
        }
    }
}
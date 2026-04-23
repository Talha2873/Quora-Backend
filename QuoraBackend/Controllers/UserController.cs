using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoraBackend.Data;

namespace QuoraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetUserPosts(int id)
        {
            var posts = await _context.Questions
                .Where(q => q.UserId == id)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return Ok(posts);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoraBackend.Data;
using QuoraBackend.Models;

namespace QuoraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL QUESTIONS
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _context.Questions
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return Ok(questions);
        }

        // ✅ GET QUESTIONS BY USER
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var posts = await _context.Questions
                .AsNoTracking()
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return Ok(posts);
        }

        // ✅ ADD QUESTION (FINAL FIX)
        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            // 🔥 Model validation (auto + manual safety)
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState
                });
            }

            if (question == null)
            {
                return BadRequest(new { message = "Invalid request body" });
            }

            if (string.IsNullOrWhiteSpace(question.Content))
            {
                return BadRequest(new { message = "Content is required" });
            }

            // 🔥 Check user exists
            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == question.UserId);

            if (!userExists)
            {
                return BadRequest(new { message = "Invalid userId" });
            }

            // ✅ Server-side values
            question.CreatedAt = DateTime.UtcNow;

            // ❗ Prevent EF/navigation issues
            question.User = null;

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Question added successfully",
                data = question
            });
        }

        // ✅ ADD QUESTION WITH IMAGE
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] string content,
            [FromForm] int userId,
            IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest(new { message = "Content is required" });

            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
                return BadRequest(new { message = "Invalid userId" });

            string imagePath = "";

            if (image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = $"images/{fileName}";
            }

            var question = new Question
            {
                Content = content,
                UserId = userId,
                ImageUrl = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Question with image added",
                data = question
            });
        }

        // ✅ DELETE QUESTION
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
                return NotFound(new { message = "Question not found" });

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
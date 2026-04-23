using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoraBackend.Data;
using QuoraBackend.Models;

namespace QuoraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ LOGIN (using Email + Password)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest(new { message = "Email and Password are required" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
                return NotFound(new { message = "Invalid email or password" });

            return Ok(user);
        }

        // ✅ SIGNUP
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest(new { message = "Email and Password are required" });

            var exists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (exists)
                return BadRequest(new { message = "Email already exists" });

            // ✅ Safe defaults
            user.Name ??= user.Email;
            user.Username ??= user.Email; // optional
            user.Avatar ??= "https://i.pravatar.cc/150";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace QuoraBackend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // 🔥 REQUIRED → no more NULL
        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        // default avatar
        public string Avatar { get; set; } = "https://i.pravatar.cc/150";

        public List<Question>? Questions { get; set; }
    }
}
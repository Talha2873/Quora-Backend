using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuoraBackend.Models
{
    public class Question
    {
        public int Id { get; set; }

        // ✅ REQUIRED FIELD
        [Required]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        // ✅ Better to set from backend (UTC)
        public DateTime CreatedAt { get; set; }

        // ✅ REQUIRED FK
        [Required]
        public int UserId { get; set; }

        // 🔥 IMPORTANT: prevent circular JSON issues
        [JsonIgnore]
        public User? User { get; set; }
    }
}
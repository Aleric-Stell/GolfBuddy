using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolfBuddy.Api.Models
{
    public class Round
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CourseName { get; set; } = "";

        [Required]
        public DateTime DatePlayed { get; set; } = DateTime.UtcNow;

        public int TotalScore { get; set; } = 0;

        // Relationship to ApplicationUser
        [Required]
        public string UserId { get; set; } = "";

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Optional: Later add navigation for Holes and Shots
        // public List<Hole> Holes { get; set; } = new();
    }
}
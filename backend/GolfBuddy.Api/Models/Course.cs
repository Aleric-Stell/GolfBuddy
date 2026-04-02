using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GolfBuddy.Api.Models
{
    public class Course
    {
        public int Id { get; set; }


        public required string Name { get; set; }

        public string? Location { get; set; }

        public ICollection<Hole> Holes { get; set; } = new List<Hole>();
        public ICollection<Round> Rounds { get; set; } = new List<Round>(); // Rounds played on this course
    }
}
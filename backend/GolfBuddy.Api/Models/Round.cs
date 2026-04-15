using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GolfBuddy.Api.Models
{
    public class Round
    {
        public int Id { get; set; }
  
        public DateTime DatePlayed { get; set; }

        // Foreign Keys
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public ICollection<Shot> Shots { get; set; } = new List<Shot>();
    }
}

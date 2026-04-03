using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GolfBuddy.Api.Models
{
    public class Hole
    {
        public int Id { get; set; }

        public required int HoleNumber { get; set; }

        public required int Par { get; set; }

        public int Yardage { get; set; }

        // Foreign Key
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public ICollection<Shot> Shots { get; set; } = new List<Shot>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace GolfBuddy.Api.Models
{
    public class Shot
    {
        public int Id { get; set; }

        public int HoleId { get; set; }
        public required Hole Hole { get; set; }

        public int RoundId { get; set; }
        public required Round Round { get; set; }

        public int ShotNumber { get; set; } // 1st, 2nd, etc.
        public int DistanceYards { get; set; }

        public required string Club { get; set; }
    }
}
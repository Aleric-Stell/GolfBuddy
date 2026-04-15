namespace GolfBuddy.Api.Contracts.Shots
{
    public class CreateShotRequest
    {
        public int HoleId { get; set; }
        public int RoundId { get; set; }
        public int ShotNumber { get; set; }
        public int DistanceYards { get; set; }
        public required string Club { get; set; }
        public string? Result { get; set; }
    }
}

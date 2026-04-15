namespace GolfBuddy.Api.Contracts.Shots
{
    public class UpdateShotRequest
    {
        public int HoleId { get; set; }
        public int ShotNumber { get; set; }
        public int DistanceYards { get; set; }
        public required string Club { get; set; }
        public string? Result { get; set; }
    }
}

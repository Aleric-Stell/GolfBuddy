namespace GolfBuddy.Api.Contracts.Shots
{
    public class ShotResponse
    {
        public int Id { get; set; }
        public int RoundId { get; set; }
        public int HoleId { get; set; }
        public int HoleNumber { get; set; }
        public int ShotNumber { get; set; }
        public int DistanceYards { get; set; }
        public string Club { get; set; } = string.Empty;
        public string? Result { get; set; }
    }
}

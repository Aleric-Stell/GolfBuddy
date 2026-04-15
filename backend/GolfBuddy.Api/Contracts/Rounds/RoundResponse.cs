namespace GolfBuddy.Api.Contracts.Rounds
{
    public class RoundResponse
    {
        public int Id { get; set; }
        public DateTime DatePlayed { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int HoleCount { get; set; }
        public int ShotCount { get; set; }
        public int TotalStrokes { get; set; }
        public int TotalPar { get; set; }
        public int ScoreToPar { get; set; }
    }
}

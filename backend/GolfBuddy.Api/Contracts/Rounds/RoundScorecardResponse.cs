namespace GolfBuddy.Api.Contracts.Rounds
{
    public class RoundScorecardResponse
    {
        public int RoundId { get; set; }
        public DateTime DatePlayed { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalPar { get; set; }
        public int TotalStrokes { get; set; }
        public int ScoreToPar { get; set; }
        public List<RoundHoleSummaryResponse> Holes { get; set; } = new();
    }
}

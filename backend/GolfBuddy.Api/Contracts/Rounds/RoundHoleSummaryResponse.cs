namespace GolfBuddy.Api.Contracts.Rounds
{
    public class RoundHoleSummaryResponse
    {
        public int HoleId { get; set; }
        public int HoleNumber { get; set; }
        public int Par { get; set; }
        public int Yardage { get; set; }
        public int Strokes { get; set; }
        public int ScoreToPar { get; set; }
    }
}

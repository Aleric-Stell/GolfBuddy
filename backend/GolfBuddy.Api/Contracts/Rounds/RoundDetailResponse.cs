using GolfBuddy.Api.Contracts.Shots;

namespace GolfBuddy.Api.Contracts.Rounds
{
    public class RoundDetailResponse : RoundResponse
    {
        public List<RoundHoleSummaryResponse> Holes { get; set; } = new();
        public List<ShotResponse> Shots { get; set; } = new();
    }
}

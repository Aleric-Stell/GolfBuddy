namespace GolfBuddy.Api.Contracts.Rounds
{
    public class CreateRoundRequest
    {
        public DateTime DatePlayed { get; set; }
        public int CourseId { get; set; }
    }
}

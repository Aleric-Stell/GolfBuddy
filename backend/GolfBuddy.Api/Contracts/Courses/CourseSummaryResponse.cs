namespace GolfBuddy.Api.Contracts.Courses
{
    public class CourseSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public int HoleCount { get; set; }
        public int TotalPar { get; set; }
    }
}

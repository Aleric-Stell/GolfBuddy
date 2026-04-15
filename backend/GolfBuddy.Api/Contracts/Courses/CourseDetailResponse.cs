namespace GolfBuddy.Api.Contracts.Courses
{
    public class CourseDetailResponse : CourseSummaryResponse
    {
        public List<CourseHoleResponse> Holes { get; set; } = new();
    }
}

namespace GolfBuddy.Api.Contracts.Courses
{
    public class CreateCourseRequest
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public required List<CourseHoleRequest> Holes { get; set; }
    }
}

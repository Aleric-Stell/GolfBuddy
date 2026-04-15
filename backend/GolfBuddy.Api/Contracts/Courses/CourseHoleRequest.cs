namespace GolfBuddy.Api.Contracts.Courses
{
    public class CourseHoleRequest
    {
        public int HoleNumber { get; set; }
        public int Par { get; set; }
        public int Yardage { get; set; }
    }
}

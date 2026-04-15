namespace GolfBuddy.Api.Contracts.Auth
{
    public class UserProfileResponse
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required List<string> Roles { get; set; }
        public double Handicap { get; set; }
    }
}

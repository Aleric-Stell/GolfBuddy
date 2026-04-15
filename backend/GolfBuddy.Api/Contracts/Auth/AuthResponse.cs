namespace GolfBuddy.Api.Contracts.Auth
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public required UserProfileResponse User { get; set; }
    }
}

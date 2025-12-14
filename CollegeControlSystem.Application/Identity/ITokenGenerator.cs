using CollegeControlSystem.Domain.Identity;

namespace CollegeControlSystem.Application.Identity
{
    public interface ITokenGenerator
    {
        Task<string> GenerateJwtTokenAsync(AppUser appUser);
        RefreshToken GenereteRefreshToken();
    }
}

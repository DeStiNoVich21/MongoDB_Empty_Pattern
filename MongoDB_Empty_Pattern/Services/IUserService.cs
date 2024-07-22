using MongoDB_Empty_Pattern.Models;
using System.Security.Claims;

namespace MongoDB_Empty_Pattern.Services
{
    public interface IUserService
    {
        Task<string> CreateToken(Users user);
        Task<string> RefreshToken(string token);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        ClaimsPrincipal ValidateTokenAndGetPrincipal(string token);
    }
}

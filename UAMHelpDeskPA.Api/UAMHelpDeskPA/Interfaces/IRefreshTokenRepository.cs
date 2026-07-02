using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);

        Task AddAsync(RefreshToken token);

        Task RevokeAsync(RefreshToken token);


        Task<List<RefreshToken>> GetActiveSessionsByUserIdAsync(int userId);

        Task<RefreshToken?> GetByIdAsync(int refreshTokenId);

        Task RevokeByIdAsync(RefreshToken refreshToken, string reason);

        Task RevokeAllByUserIdAsync(int userId, string reason);

        Task RevokeAllExceptCurrentAsync(
            int userId,
            string currentToken,
            string reason);
    }
}
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IOtpCodeRepository
    {
        Task<OtpCode?> GetValidOtpAsync(int userId, string code);

        Task AddAsync(OtpCode otp);

        Task InvalidateAllByUserIdAsync(int userId);
    }
}
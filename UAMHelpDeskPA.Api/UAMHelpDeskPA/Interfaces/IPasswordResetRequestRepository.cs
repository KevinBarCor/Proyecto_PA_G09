using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IPasswordResetRequestRepository : IRepository<PasswordResetRequest>
    {
        Task<PasswordResetRequest?> GetBySessionTokenAsync(string sessionToken);

        Task<PasswordResetRequest?> GetValidRequestAsync(string sessionToken, string code);

        Task InvalidatePendingRequestsAsync(int userId);

        Task MarkAsUsedAsync(PasswordResetRequest request);
    }
}
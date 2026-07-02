using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IPendingSessionRepository
    {
        Task<PendingSession?> GetByTokenAsync(
            string sessionToken);

        Task AddAsync(PendingSession session);

        Task InvalidateAsync(
            PendingSession session);

        Task InvalidateAllByUserIdAsync(
            int userId);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Repositories;

public class PendingSessionRepository(
    AppDbContext context,
    IStringLocalizer<PendingSessionRepository> localizer)
    : Repository<PendingSession>(context),
      IPendingSessionRepository
{
    public async Task<PendingSession?> GetByTokenAsync(
        string sessionToken)
    {
        return await context.PendingSessions
            .FirstOrDefaultAsync(x =>
                x.SessionToken == sessionToken &&
                !x.IsUsed);
    }

    public async Task AddAsync(
        PendingSession session)
    {
        await context.PendingSessions
            .AddAsync(session);
    }

    public async Task InvalidateAsync(
        PendingSession session)
    {
        session.IsUsed = true;
        await context.SaveChangesAsync();
    }

    public async Task InvalidateAllByUserIdAsync(
        int userId)
    {
        var sessions = await context.PendingSessions
            .Where(x =>
                x.UserId == userId &&
                !x.IsUsed)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.IsUsed = true;
        }

        await context.SaveChangesAsync();
    }
}
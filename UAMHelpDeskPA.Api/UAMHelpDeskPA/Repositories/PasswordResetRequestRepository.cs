using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Repositories;

public class PasswordResetRequestRepository(
    AppDbContext context,
    IStringLocalizer<PasswordResetRequestRepository> localizer)
    : Repository<PasswordResetRequest>(context),
      IPasswordResetRequestRepository
{
    public async Task<PasswordResetRequest?> GetBySessionTokenAsync(
        string sessionToken)
    {
        return await context.PasswordResetRequests
            .FirstOrDefaultAsync(x =>
                x.SessionToken == sessionToken);
    }

    public async Task<PasswordResetRequest?> GetValidRequestAsync(
        string sessionToken,
        string code)
    {
        return await context.PasswordResetRequests
            .FirstOrDefaultAsync(x =>
                x.SessionToken == sessionToken &&
                x.Code == code &&
                !x.IsUsed);
    }

    public async Task InvalidatePendingRequestsAsync(
        int userId)
    {
        var requests = await context.PasswordResetRequests
            .Where(x =>
                x.UserId == userId &&
                !x.IsUsed)
            .ToListAsync();

        foreach (var request in requests)
        {
            request.IsUsed = true;
        }

        await context.SaveChangesAsync();
    }
    public async Task MarkAsUsedAsync(
    PasswordResetRequest request)
    {
        request.IsUsed = true;
        request.UsedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }
}
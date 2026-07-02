using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Repositories;

public class RefreshTokenRepository(
    AppDbContext context,
    IStringLocalizer<RefreshTokenRepository> localizer)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task AddAsync(RefreshToken token)
    {
        await context.RefreshTokens.AddAsync(token);
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.RevokedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }
    public async Task<List<RefreshToken>> GetActiveSessionsByUserIdAsync(int userId)
    {
        return await context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiresAtUtc > DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<RefreshToken?> GetByIdAsync(int refreshTokenId)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == refreshTokenId);
    }
    public async Task RevokeByIdAsync(
    RefreshToken refreshToken,
    string reason)
    {
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAtUtc = DateTime.UtcNow;
        refreshToken.RevokedReason = reason;

        _context.RefreshTokens.Update(refreshToken);

        await context.SaveChangesAsync();
    }
    public async Task RevokeAllByUserIdAsync(
    int userId,
    string reason)
    {
        var tokens = await context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAtUtc = DateTime.UtcNow;
            token.RevokedReason = reason;
        }

        await context.SaveChangesAsync();
    }
    public async Task RevokeAllExceptCurrentAsync(
    int userId,
    string currentToken,
    string reason)
    {
        var tokens = await context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.Token != currentToken)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAtUtc = DateTime.UtcNow;
            token.RevokedReason = reason;
        }

        await context.SaveChangesAsync();
    }
    public async Task RevokeAllByUserIdAsync(int userId)
    {
        var tokens = await context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await context.SaveChangesAsync();
    }
}
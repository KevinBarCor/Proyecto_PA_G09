using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Repositories;

public class OtpCodeRepository(
    AppDbContext context,
    IStringLocalizer<OtpCodeRepository> _localizer)
    : Repository<OtpCode>(context), IOtpCodeRepository
{
    public async Task<OtpCode?> GetValidOtpAsync(
        int userId,
        string code)
    {
        return await context.OtpCodes
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Code == code &&
                !x.IsUsed);
    }

    public async Task AddAsync(OtpCode otp)
    {
        await context.OtpCodes.AddAsync(otp);
    }

    public async Task InvalidateAllByUserIdAsync(int userId)
    {
        var otps = await context.OtpCodes
            .Where(x => x.UserId == userId && !x.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }

        await context.SaveChangesAsync();
    }
}
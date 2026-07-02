using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;
using UamHelpDeskPA.Api.Repositories;

namespace UamHelpDeskPA.Api.Repositories;

public class UserRepository(
    AppDbContext context,
    IStringLocalizer<UserRepository> localizer)
    : Repository<User>(context), IUserRepository
{
    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await context.Users.AnyAsync(
            x => x.Email.ToLower() == normalizedEmail &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<ApiOperationResultDto<List<UserDto>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await context.Users
    .Include(x => x.Role)
    .Where(x => x.IsActive)
    .AsNoTracking()
    .Select(x => new UserDto(
        x.Id,
        x.RoleId,
        x.Role.Name,
        x.FirstName,
        x.LastName,
        x.Email,
        x.IsActive))
    .ToListAsync(cancellationToken);

        return new ApiOperationResultDto<List<UserDto>>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["OperationSuccessful"],
            Result = users
        };
    }

    public async Task<ApiOperationResultDto<UserDto>> GetUserByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(x => x.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["UserNotFound"]
            };
        }

        return new ApiOperationResultDto<UserDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["OperationSuccessful"],
            Result = MapToDto(user)
        };
    }

    public async Task<ApiOperationResultDto<List<UserDto>>> GetUsersByRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var users = await context.Users
            .Include(x => x.Role)
            .Where(x =>
    x.RoleId == roleId &&
    x.IsActive)
            .AsNoTracking()
            .Select(x => new UserDto(
                x.Id,
                x.RoleId,
                x.Role.Name,
                x.FirstName,
                x.LastName,
                x.Email,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return new ApiOperationResultDto<List<UserDto>>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["OperationSuccessful"],
            Result = users
        };
    }

    public async Task<ApiOperationResultDto<UserDto>> CreateUserAsync(
        CreateUserDto resource,
        CancellationToken cancellationToken = default)
    {
        if (await EmailExistsAsync(resource.Email, null, cancellationToken))
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["UserEmailExists"]
            };
        }

        var role = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == resource.RoleId && x.IsActive,
                cancellationToken);

        if (role is null)
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["RoleNotFound"]
            };
        }

        var user = new User
        {
            RoleId = resource.RoleId,
            FirstName = resource.FirstName,
            LastName = resource.LastName,
            Email = resource.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(resource.Password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        user.Role = role;

        return new ApiOperationResultDto<UserDto>
        {
            Success = true,
            Code = StatusCodes.Status201Created.ToString(),
            Message = localizer["UserCreatedSuccessfully"],
            Result = MapToDto(user)
        };
    }

    public async Task<ApiOperationResultDto<UserDto>> UpdateUserAsync(
        int id,
        UpdateUserDto resource,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["UserNotFound"]
            };
        }

        if (await EmailExistsAsync(resource.Email, id, cancellationToken))
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["UserEmailExists"]
            };
        }

        var role = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == resource.RoleId && x.IsActive,
                cancellationToken);

        if (role is null)
        {
            return new ApiOperationResultDto<UserDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["RoleNotFound"]
            };
        }

        user.RoleId = resource.RoleId;
        user.FirstName = resource.FirstName;
        user.LastName = resource.LastName;
        user.Email = resource.Email.Trim().ToLowerInvariant();
        user.UpdatedAtUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(resource.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resource.Password);
        }

        await context.SaveChangesAsync(cancellationToken);

        user.Role = role;

        return new ApiOperationResultDto<UserDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["UserUpdatedSuccessfully"],
            Result = MapToDto(user)
        };
    }

    public async Task<ApiOperationResultDto<object>> DeleteUserAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
        {
            return new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["UserNotFound"]
            };
        }

        user.IsActive = false;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new ApiOperationResultDto<object>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["UserDeletedSuccessfully"]
        };
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.RoleId,
            user.Role?.Name ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.Email,
            user.IsActive);
    }
}
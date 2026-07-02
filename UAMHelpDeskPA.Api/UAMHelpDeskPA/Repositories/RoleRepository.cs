using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;
using UamHelpDeskPA.Api.Repositories;

namespace UamHelpDeskPA.Api.Repositories;

public class RoleRepository(
    AppDbContext context,
    IStringLocalizer<RoleRepository> localizer)
    : Repository<Role>(context), IRoleRepository
{
    public async Task<bool> NameExistsAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();

        return await context.Roles.AnyAsync(
            x => x.Name.ToLower() == normalizedName &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<ApiOperationResultDto<List<RoleDto>>> GetAllRolesAsync(
    CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles
    .Where(x => x.IsActive)
    .AsNoTracking()
    .Select(x => new RoleDto(
        x.Id,
        x.Name,
        x.Description,
        x.IsActive))
    .ToListAsync(cancellationToken);

        return new ApiOperationResultDto<List<RoleDto>>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["OperationSuccessful"],
            Result = roles
        };
    }

    public async Task<ApiOperationResultDto<RoleDto>> GetRoleByIdAsync(
     int id,
     CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (role is null)
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["RoleNotFound"]
            };
        }

        return new ApiOperationResultDto<RoleDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["OperationSuccessful"],
            Result = MapToDto(role)
        };
    }

    public async Task<ApiOperationResultDto<RoleDto>> CreateRoleAsync(
     CreateRoleDto resource,
     CancellationToken cancellationToken = default)
    {
        if (await NameExistsAsync(
            resource.Name,
            cancellationToken: cancellationToken))
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["RoleNameExists"]
            };
        }
        var validRoles = new[]
{
    "Administrator",
    "Technician",
    "Instructor"
};

        if (!validRoles.Contains(resource.Name.Trim()))
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["InvalidRoleName"]
            };
        }

        var role = new Role
        {
            Name = resource.Name.Trim(),
            Description = resource.Description?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await context.Roles.AddAsync(role, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return new ApiOperationResultDto<RoleDto>
        {
            Success = true,
            Code = StatusCodes.Status201Created.ToString(),
            Message = localizer["RoleCreatedSuccessfully"],
            Result = MapToDto(role)
        };
    }

    public async Task<ApiOperationResultDto<RoleDto>> UpdateRoleAsync(
      int id,
      UpdateRoleDto resource,
      CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (role is null)
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["RoleNotFound"]
            };
        }

        if (await NameExistsAsync(
            resource.Name,
            id,
            cancellationToken))
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["RoleNameExists"]
            };
        }
        var validRoles = new[]
{
    "Administrator",
    "Technician",
    "Instructor"
};

        if (!validRoles.Contains(resource.Name.Trim()))
        {
            return new ApiOperationResultDto<RoleDto>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = localizer["InvalidRoleName"]
            };
        }
        role.Name = resource.Name.Trim();
        role.Description = resource.Description?.Trim();
        role.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new ApiOperationResultDto<RoleDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["RoleUpdatedSuccessfully"],
            Result = MapToDto(role)
        };
    }

    public async Task<ApiOperationResultDto<object>> DeleteRoleAsync(
      int id,
      CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (role is null)
        {
            return new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status404NotFound.ToString(),
                Message = localizer["RoleNotFound"]
            };
        }

        role.IsActive = false;
        role.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new ApiOperationResultDto<object>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = localizer["RoleDeletedSuccessfully"]
        };
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.IsActive);
    }
}
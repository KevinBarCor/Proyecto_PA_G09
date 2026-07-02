using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    /// Verifica si ya existe un nombre registrado,
    /// opcionalmente excluyendo un id.
    Task<bool> NameExistsAsync(
        string name,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    /// Obtiene todos los roles.
    Task<ApiOperationResultDto<List<RoleDto>>> GetAllRolesAsync(
        CancellationToken cancellationToken = default);

    /// Obtiene un rol por id.
    Task<ApiOperationResultDto<RoleDto>> GetRoleByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// Crea un nuevo rol.
    Task<ApiOperationResultDto<RoleDto>> CreateRoleAsync(
        CreateRoleDto resource,
        CancellationToken cancellationToken = default);

    /// Actualiza un rol existente.
    Task<ApiOperationResultDto<RoleDto>> UpdateRoleAsync(
        int id,
        UpdateRoleDto resource,
        CancellationToken cancellationToken = default);

    /// Realiza eliminación lógica de un rol.
    Task<ApiOperationResultDto<object>> DeleteRoleAsync(
        int id,
        CancellationToken cancellationToken = default);
}
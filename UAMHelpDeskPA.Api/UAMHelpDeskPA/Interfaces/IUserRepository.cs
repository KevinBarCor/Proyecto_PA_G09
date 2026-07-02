using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces;

public interface IUserRepository : IRepository<User>
{
    /// Verifica si existe un correo registrado.
    Task<bool> EmailExistsAsync(
        string email,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    /// Obtiene todos los usuarios.
    Task<ApiOperationResultDto<List<UserDto>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default);

    /// Obtiene un usuario por id.
    Task<ApiOperationResultDto<UserDto>> GetUserByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// Obtiene usuarios por rol.
    Task<ApiOperationResultDto<List<UserDto>>> GetUsersByRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default);

    /// Crea un usuario.
    Task<ApiOperationResultDto<UserDto>> CreateUserAsync(
        CreateUserDto resource,
        CancellationToken cancellationToken = default);

    /// Actualiza un usuario.
    Task<ApiOperationResultDto<UserDto>> UpdateUserAsync(
        int id,
        UpdateUserDto resource,
        CancellationToken cancellationToken = default);

    /// Eliminación lógica.
    Task<ApiOperationResultDto<object>> DeleteUserAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
}
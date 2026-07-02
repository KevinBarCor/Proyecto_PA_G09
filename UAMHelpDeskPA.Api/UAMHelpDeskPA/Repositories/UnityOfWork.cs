using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;


namespace UamHelpDeskPA.Api.Repositories;

/// <summary>
/// Implementa Unit of Work para centralizar acceso a repositorios y guardado.
/// </summary>
public class UnitOfWork(
    AppDbContext context,
    IStringLocalizer<LaboratoryRepository> laboratoryLocalizer,
    IStringLocalizer<EquipmentRepository> equipmentLocalizer,
    IStringLocalizer<RoleRepository> roleLocalizer,
    IStringLocalizer<UserRepository> userLocalizer,
    IStringLocalizer<RefreshTokenRepository> refreshTokenLocalizer,
    IStringLocalizer<OtpCodeRepository> otpCodeLocalizer,
    IStringLocalizer<PendingSessionRepository> pendingSessionLocalizer,
    IStringLocalizer<PasswordResetRequestRepository> passwordResetRequestLocalizer)
    : IUnitOfWork
{
    /// <summary>
    /// Instancia interna perezosa del repositorio de laboratorios.
    /// </summary>
    private ILaboratoryRepository? _laboratories;

    /// <summary>
    /// Instancia interna perezosa del repositorio de equipos.
    /// </summary>
    private IEquipmentRepository? _equipment;

    /// <summary>
    /// Instancia interna perezosa del repositorio de roles.
    /// </summary>
    private IRoleRepository? _roles;

    /// <summary>
    /// Instancia interna perezosa del repositorio de usuarios.
    /// </summary>
    private IUserRepository? _users;

    /// <summary>
    /// Instancia interna perezosa del repositorio de refresh tokens.
    /// </summary>
    private IRefreshTokenRepository? _refreshTokens;

    /// <summary>
    /// Instancia interna perezosa del repositorio de OTP.
    /// </summary>
    private IOtpCodeRepository? _otpCodes;

    /// <summary>
    /// Instancia interna perezosa del repositorio de sesiones pendientes.
    /// </summary>
    private IPendingSessionRepository? _pendingSessions;

    /// <summary>
    /// Instancia interna perezosa del repositorio de solicitudes de recuperación.
    /// </summary>
    private IPasswordResetRequestRepository? _passwordResetRequests;

    /// <summary>
    /// Exposición pública del repositorio de laboratorios.
    /// </summary>
    public ILaboratoryRepository Laboratories =>
        _laboratories ??= new LaboratoryRepository(
            context,
            laboratoryLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de equipos.
    /// </summary>
    public IEquipmentRepository Equipment =>
        _equipment ??= new EquipmentRepository(
            context,
            equipmentLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de roles.
    /// </summary>
    public IRoleRepository Roles =>
        _roles ??= new RoleRepository(
            context,
            roleLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de usuarios.
    /// </summary>
    public IUserRepository Users =>
        _users ??= new UserRepository(
            context,
            userLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de refresh tokens.
    /// </summary>
    public IRefreshTokenRepository RefreshTokens =>
        _refreshTokens ??= new RefreshTokenRepository(
            context,
            refreshTokenLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de OTP.
    /// </summary>
    public IOtpCodeRepository OtpCodes =>
        _otpCodes ??= new OtpCodeRepository(
            context,
            otpCodeLocalizer);

    /// <summary>
    /// Exposición pública del repositorio de sesiones pendientes.
    /// </summary>
    public IPendingSessionRepository PendingSessions =>
        _pendingSessions ??= new PendingSessionRepository(
            context,
            pendingSessionLocalizer);
    /// <summary>
    /// Exposición pública del repositorio de solicitudes de recuperación.
    /// </summary>
    public IPasswordResetRequestRepository PasswordResetRequests =>
        _passwordResetRequests ??= new PasswordResetRequestRepository(
            context,
            passwordResetRequestLocalizer);
    /// <summary>
    /// Guarda todos los cambios pendientes en base de datos.
    /// </summary>
    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
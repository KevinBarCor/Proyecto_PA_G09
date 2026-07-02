
using UamHelpDeskPA.Api.Interfaces;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IUnitOfWork
    {

        /// Repositorio de laboratorios.

        ILaboratoryRepository Laboratories { get; }


        /// Repositorio de equipos.
 
        IEquipmentRepository Equipment { get; }


        /// Repositorio de roles.
        IRoleRepository Roles { get; }

        //Repositorio de Users

        IUserRepository Users { get; }
        //Repositorio de Refreshtoken
        IRefreshTokenRepository RefreshTokens { get; }

        /// Guarda en base de datos todos los cambios pendientes.
        IOtpCodeRepository OtpCodes { get; }

        IPendingSessionRepository PendingSessions { get; }
        IPasswordResetRequestRepository PasswordResetRequests { get; }
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}

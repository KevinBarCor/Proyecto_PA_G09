using UamHelpDeskPA.Api.Interfaces;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IUnitOfWork
    {

        /// Repositorio de laboratorios.

        ILaboratoryRepository Laboratories { get; }


        /// Repositorio de equipos.
 
        IEquipmentRepository Equipment { get; }


        /// Guarda en base de datos todos los cambios pendientes.

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}

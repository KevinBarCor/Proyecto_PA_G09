
namespace UamHelpDeskPA.Api.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {


        /// Obtiene todos los registros de la entidad.

        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);



        /// Obtiene una entidad por su identificador.

        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);


        /// Agrega una nueva entidad al contexto.

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        // Marca un registro como actualizado. 

        void Update(TEntity entity);

    }
}

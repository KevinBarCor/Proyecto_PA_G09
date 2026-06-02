using Microsoft.EntityFrameworkCore;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;

namespace UamHelpDeskPA.Repositories
{
    /// <summary>
    /// Implementación genérica base para repositorios.
    /// </summary>
    public class Repository<TEntity>(AppDbContext context)
        : IRepository<TEntity>
        where TEntity : class
    {

        /// Contexto principal de el EF Core.

        protected readonly AppDbContext _context = context;

        /// Set EF Core de la entidad para consultar y persistir datos.

        protected readonly DbSet<TEntity> Set = context.Set<TEntity>();

        /// Obtiene todos los registros. Solo lectura

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await Set.AsNoTracking().ToListAsync(cancellationToken);



        /// Obtiene un registro por id.

        public virtual async Task<TEntity?> GetByIdAsync(int id,CancellationToken cancellationToken = default) =>
            await Set.FindAsync([id], cancellationToken);



        /// Agrega entidad nueva al contexto.

        public virtual async Task AddAsync(TEntity entity,CancellationToken cancellationToken = default) =>
            await Set.AddAsync(entity, cancellationToken);

        /// Actualiza una entidad.

        public virtual void Update(TEntity entity) =>
            Set.Update(entity);

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        public virtual void Remove(TEntity entity) =>
            Set.Remove(entity);
    }
}

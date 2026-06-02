using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Repositories;

namespace UamHelpDeskPA.Api.Repositories;

/// <summary>
/// Implementa Unit of Work para centralizar acceso a repositorios y guardado.
/// </summary>
public class UnitOfWork(AppDbContext context,IStringLocalizer<LaboratoryRepository> laboratoryLocalizer,IStringLocalizer<EquipmentRepository> equipmentLocalizer)
    : IUnitOfWork
{

    /// Instancia interna perezosa del repositorio de laboratorios.

    private ILaboratoryRepository? _laboratories;


    /// Instancia interna perezosa del repositorio de equipos.

    private IEquipmentRepository? _equipment;


    /// Exposición pública del repositorio de laboratorios.

    public ILaboratoryRepository Laboratories =>_laboratories ??=new LaboratoryRepository(context,laboratoryLocalizer);


    /// Exposición pública del repositorio de equipos.

    public IEquipmentRepository Equipment => _equipment ??= new EquipmentRepository(context,equipmentLocalizer);


    /// Guarda todos los cambios pendientes en base de datos.

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>context.SaveChangesAsync(cancellationToken);
}
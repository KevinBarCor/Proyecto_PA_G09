using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;
namespace UamHelpDeskPA.Api.Interfaces;

public interface IEquipmentRepository : IRepository<Equipment>
{

    /// Verifica si ya existe un código registrado,
    /// opcionalmente excluyendo un id.

    Task<bool> CodeExistsAsync(
        string code,
        int? excludeId = null,
        CancellationToken cancellationToken = default);


    /// Verifica si ya existe un serial registrado,
    /// opcionalmente excluyendo un id.

    Task<bool> SerialNumberExistsAsync(
        string serialNumber,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    /// Obtiene todos los equipos.
    Task<ApiOperationResultDto<List<EquipmentDto>>> GetAllEquipmentAsync(
        CancellationToken cancellationToken = default);


    /// Obtiene un equipo por id.
    Task<ApiOperationResultDto<EquipmentDto>> GetEquipmentByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


    /// Obtiene los equipos de un laboratorio específico.

    Task<ApiOperationResultDto<List<EquipmentDto>>> GetEquipmentByLaboratoryAsync(
        int laboratoryId,
        CancellationToken cancellationToken = default);


    /// Crea un nuevo equipo.

    Task<ApiOperationResultDto<EquipmentDto>> CreateEquipmentAsync(
        CreateEquipmentDto resource,
        CancellationToken cancellationToken = default);


    /// Actualiza un equipo existente.

    Task<ApiOperationResultDto<EquipmentDto>> UpdateEquipmentAsync(
        int id,
        UpdateEquipmentDto resource,
        CancellationToken cancellationToken = default);


    /// Realiza eliminación lógica de un equipo.

    Task<ApiOperationResultDto<object>> DeleteEquipmentAsync(
        int id,
        CancellationToken cancellationToken = default);
}

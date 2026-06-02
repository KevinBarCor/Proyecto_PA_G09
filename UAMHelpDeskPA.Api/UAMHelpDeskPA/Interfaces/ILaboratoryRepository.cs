using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Interfaces
{
    public interface ILaboratoryRepository : IRepository<Laboratory>
    {
        Task<bool> NameExistsAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<ApiOperationResultDto<List<LaboratoryDto>>> GetAllLaboratoriesAsync(
            CancellationToken cancellationToken = default);

        Task<ApiOperationResultDto<LaboratoryDto>> GetLaboratoryByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<ApiOperationResultDto<LaboratoryDto>> CreateLaboratoryAsync(
            CreateLaboratoryDto resource,
            CancellationToken cancellationToken = default);

        Task<ApiOperationResultDto<LaboratoryDto>> UpdateLaboratoryAsync(
            int id,
            UpdateLaboratoryDto resource,
            CancellationToken cancellationToken = default);

        Task<ApiOperationResultDto<object>> DeleteLaboratoryAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}

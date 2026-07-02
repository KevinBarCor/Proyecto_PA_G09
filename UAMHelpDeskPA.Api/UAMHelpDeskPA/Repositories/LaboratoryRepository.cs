using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Repositories
{
    public class LaboratoryRepository(AppDbContext context,IStringLocalizer<LaboratoryRepository> localizer) : Repository<Laboratory>(context), ILaboratoryRepository
    {
        /// <summary>
        /// Verifica si ya existe un laboratorio con el mismo nombre.
        /// </summary>
        public async Task<bool> NameExistsAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLowerInvariant();

            return await context.Laboratories.AnyAsync(
                x =>
                    x.Name.ToLower() == normalizedName &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los laboratorios.
        /// </summary>
        public async Task<ApiOperationResultDto<List<LaboratoryDto>>>
            GetAllLaboratoriesAsync(
            CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<List<LaboratoryDto>>();

            var laboratories = await context.Laboratories
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var hasRecords = laboratories.Count > 0;

            result.Success = hasRecords;
            result.Code = hasRecords
                ? StatusCodes.Status200OK.ToString()
                : StatusCodes.Status404NotFound.ToString();

            result.Message = hasRecords
                ? localizer["OperationSuccessful"].Value
                : localizer["LaboratoriesNotFound"].Value;

            result.Result = hasRecords
                ? laboratories.Select(MapToDto).ToList()
                : null;

            return result;
        }

        /// <summary>
        /// Obtiene un laboratorio por id.
        /// </summary>
        public async Task<ApiOperationResultDto<LaboratoryDto>>
            GetLaboratoryByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<LaboratoryDto>();

            var laboratory = await context.Laboratories
                .AsNoTracking()
                .Include(x => x.Equipments)
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            result.Success = laboratory is not null;

            result.Code = laboratory is not null
                ? StatusCodes.Status200OK.ToString()
                : StatusCodes.Status404NotFound.ToString();

            result.Message = laboratory is not null
                ? localizer["OperationSuccessful"].Value
                : localizer["LaboratoryNotFound"].Value;

            result.Result = laboratory is null
                ? null
                : MapToDto(laboratory);

            return result;
        }

        /// <summary>
        /// Crea un laboratorio.
        /// </summary>
        public async Task<ApiOperationResultDto<LaboratoryDto>>
            CreateLaboratoryAsync(
            CreateLaboratoryDto resource,
            CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<LaboratoryDto>();

            if (await NameExistsAsync(resource.Name, null, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["LaboratoryNameExists"].Value;

                return result;
            }

            var laboratory = new Laboratory
            {
                Name = resource.Name.Trim(),
                Building = resource.Building.Trim(),
                BuildingFloor = resource.BuildingFloor,
                Capacity = resource.Capacity,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await context.Laboratories.AddAsync(
                laboratory,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Code = StatusCodes.Status201Created.ToString();
            result.Message = localizer["LaboratoryCreatedSuccessfully"].Value;
            result.Result = MapToDto(laboratory);

            return result;
        }
        /// <summary>
        /// Actualiza un laboratorio.
        /// </summary>
        public async Task<ApiOperationResultDto<LaboratoryDto>>
            UpdateLaboratoryAsync(
            int id,
            UpdateLaboratoryDto resource,
            CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<LaboratoryDto>();

            var laboratory = await context.Laboratories
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            if (laboratory is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status404NotFound.ToString();
                result.Message = localizer["LaboratoryNotFound"].Value;

                return result;
            }

            if (await NameExistsAsync(resource.Name, id, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["LaboratoryNameExists"].Value;

                return result;
            }

            laboratory.Name = resource.Name.Trim();
            laboratory.Building = resource.Building.Trim();
            laboratory.BuildingFloor = resource.BuildingFloor;
            laboratory.Capacity = resource.Capacity;
            laboratory.UpdatedAtUtc = DateTime.UtcNow;

            context.Laboratories.Update(laboratory);

            await context.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = localizer["LaboratoryUpdatedSuccessfully"].Value;
            result.Result = MapToDto(laboratory);

            return result;
        }

        /// <summary>
        /// Eliminacion de un laboratorio.
        /// </summary>
        public async Task<ApiOperationResultDto<object>>
            DeleteLaboratoryAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<object>();

            var laboratory = await context.Laboratories
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            if (laboratory is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status404NotFound.ToString();
                result.Message = localizer["LaboratoryNotFound"].Value;

                return result;
            }

            laboratory.IsActive = false;
            laboratory.UpdatedAtUtc = DateTime.UtcNow;

            context.Laboratories.Update(laboratory);

            await context.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = localizer["LaboratoryDeletedSuccessfully"].Value;

            return result;
        }

        /// <summary>
        /// Convierte la entidad Laboratory a DTO.
        /// </summary>
        private static LaboratoryDto MapToDto(Laboratory l)
        {
            return new LaboratoryDto(l.Id,l.Name,l.Building,l.BuildingFloor,l.Capacity,l.IsActive,l.Equipments?
                    .Where(x => x.IsActive)
                    .Select(e => new EquipmentDto(e.Id,e.LaboratoryId,l.Name,e.Code,e.Brand,e.Model,e.SerialNumber,e.Type.ToString(),e.Status.ToString(),e.PurchaseDate,e.IsActive
                    )).ToList()
            );
        }
    }
}


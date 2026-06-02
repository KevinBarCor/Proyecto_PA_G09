using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Data;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Repositories
{
    public class EquipmentRepository (AppDbContext context,IStringLocalizer<EquipmentRepository> localizer) : Repository<Equipment>(context), IEquipmentRepository
    {
        public async Task<bool> CodeExistsAsync(
            string code,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToLowerInvariant();

            return await context.Equipment.AnyAsync(
                x => x.Code.ToLower() == normalizedCode &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }

        public async Task<bool> SerialNumberExistsAsync(
            string serialNumber,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedSerial = serialNumber.Trim().ToLowerInvariant();

            return await context.Equipment.AnyAsync(
                x => x.SerialNumber.ToLower() == normalizedSerial &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }
        public async Task<ApiOperationResultDto<List<EquipmentDto>>> GetAllEquipmentAsync(
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<List<EquipmentDto>>();

            var equipment = await context.Equipment
                .AsNoTracking()
                .Include(x => x.Laboratory)
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);

            var hasRecords = equipment.Count > 0;

            result.Success = hasRecords;
            result.Code = hasRecords
                ? StatusCodes.Status200OK.ToString()
                : StatusCodes.Status404NotFound.ToString();

            result.Message = hasRecords
                ? localizer["OperationSuccessful"].Value
                : localizer["EquipmentNotFound"].Value;

            result.Result = hasRecords
                ? equipment.Select(MapToDto).ToList()
                : null;

            return result;
        }
        public async Task<ApiOperationResultDto<EquipmentDto>> GetEquipmentByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<EquipmentDto>();

            var equipment = await context.Equipment
                .AsNoTracking()
                .Include(x => x.Laboratory)
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            result.Success = equipment is not null;

            result.Code = equipment is not null
                ? StatusCodes.Status200OK.ToString()
                : StatusCodes.Status404NotFound.ToString();

            result.Message = equipment is not null
                ? localizer["OperationSuccessful"].Value
                : localizer["EquipmentNotFound"].Value;

            result.Result = equipment is null
                ? null
                : MapToDto(equipment);

            return result;
        }
        public async Task<ApiOperationResultDto<List<EquipmentDto>>> GetEquipmentByLaboratoryAsync(
    int laboratoryId,
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<List<EquipmentDto>>();

            var equipment = await context.Equipment
                .AsNoTracking()
                .Include(x => x.Laboratory)
                .Where(x => x.LaboratoryId == laboratoryId && x.IsActive)
                .ToListAsync(cancellationToken);

            var hasRecords = equipment.Count > 0;

            result.Success = hasRecords;

            result.Code = hasRecords
                ? StatusCodes.Status200OK.ToString()
                : StatusCodes.Status404NotFound.ToString();

            result.Message = hasRecords
                ? localizer["OperationSuccessful"].Value
                : localizer["EquipmentNotFound"].Value;

            result.Result = hasRecords
                ? equipment.Select(MapToDto).ToList()
                : null;

            return result;
        }
        public async Task<ApiOperationResultDto<EquipmentDto>> CreateEquipmentAsync(
    CreateEquipmentDto resource,
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<EquipmentDto>();

            var laboratory = await context.Laboratories
                .FirstOrDefaultAsync(
                    x => x.Id == resource.LaboratoryId && x.IsActive,
                    cancellationToken);

            if (laboratory is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["LaboratoryInactive"].Value;
                return result;
            }

            if (await CodeExistsAsync(resource.Code, null, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["EquipmentCodeExists"].Value;
                return result;
            }

            if (await SerialNumberExistsAsync(resource.SerialNumber, null, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["EquipmentSerialExists"].Value;
                return result;
            }

            var equipment = new Equipment
            {
                LaboratoryId = resource.LaboratoryId,
                Code = resource.Code.Trim(),
                Brand = resource.Brand.Trim(),
                Model = resource.Model.Trim(),
                SerialNumber = resource.SerialNumber.Trim(),
                Type = resource.Type,
                Status = resource.Status,
                PurchaseDate = resource.PurchaseDate,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await context.Equipment.AddAsync(
                equipment,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            equipment.Laboratory = laboratory;

            result.Success = true;
            result.Code = StatusCodes.Status201Created.ToString();
            result.Message = localizer["EquipmentCreatedSuccessfully"].Value;
            result.Result = MapToDto(equipment);

            return result;
        }
        public async Task<ApiOperationResultDto<EquipmentDto>> UpdateEquipmentAsync(
    int id,
    UpdateEquipmentDto resource,
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<EquipmentDto>();

            var equipment = await context.Equipment
                .Include(x => x.Laboratory)
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            if (equipment is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status404NotFound.ToString();
                result.Message = localizer["EquipmentNotFound"].Value;
                return result;
            }

            var laboratory = await context.Laboratories
                .FirstOrDefaultAsync(
                    x => x.Id == resource.LaboratoryId && x.IsActive,
                    cancellationToken);

            if (laboratory is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["LaboratoryInactive"].Value;
                return result;
            }

            if (await CodeExistsAsync(resource.Code, id, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["EquipmentCodeExists"].Value;
                return result;
            }

            if (await SerialNumberExistsAsync(resource.SerialNumber, id, cancellationToken))
            {
                result.Success = false;
                result.Code = StatusCodes.Status400BadRequest.ToString();
                result.Message = localizer["EquipmentSerialExists"].Value;
                return result;
            }

            equipment.LaboratoryId = resource.LaboratoryId;
            equipment.Code = resource.Code.Trim();
            equipment.Brand = resource.Brand.Trim();
            equipment.Model = resource.Model.Trim();
            equipment.SerialNumber = resource.SerialNumber.Trim();
            equipment.Type = resource.Type;
            equipment.Status = resource.Status;
            equipment.PurchaseDate = resource.PurchaseDate;
            equipment.UpdatedAtUtc = DateTime.UtcNow;

            context.Equipment.Update(equipment);

            await context.SaveChangesAsync(cancellationToken);

            equipment.Laboratory = laboratory;

            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = localizer["EquipmentUpdatedSuccessfully"].Value;
            result.Result = MapToDto(equipment);

            return result;
        }
        public async Task<ApiOperationResultDto<object>> DeleteEquipmentAsync(
    int id,
    CancellationToken cancellationToken = default)
        {
            var result = new ApiOperationResultDto<object>();

            var equipment = await context.Equipment
                .FirstOrDefaultAsync(
                    x => x.Id == id && x.IsActive,
                    cancellationToken);

            if (equipment is null)
            {
                result.Success = false;
                result.Code = StatusCodes.Status404NotFound.ToString();
                result.Message = localizer["EquipmentNotFound"].Value;
                return result;
            }

            equipment.IsActive = false;
            equipment.UpdatedAtUtc = DateTime.UtcNow;

            context.Equipment.Update(equipment);

            await context.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Code = StatusCodes.Status200OK.ToString();
            result.Message = localizer["EquipmentDeletedSuccessfully"].Value;

            return result;
        }
        private static EquipmentDto MapToDto(Equipment e)
        {
            return new EquipmentDto(e.Id,e.LaboratoryId,e.Laboratory?.Name ?? string.Empty,e.Code,e.Brand,e.Model, e.SerialNumber, e.Type.ToString(),e.Status.ToString(),
                e.PurchaseDate,e.IsActive);
        }
    }
}

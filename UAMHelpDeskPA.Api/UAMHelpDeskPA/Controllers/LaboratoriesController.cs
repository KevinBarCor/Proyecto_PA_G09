using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Interfaces;

namespace UamHelpDeskPA.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LaboratoriesController(IUnitOfWork unitOfWork, IStringLocalizer<LaboratoriesController> stringLocalizer) : ControllerBase
    {


        /// <summary>
        /// Obtiene la lista completa de laboratorios.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
        /// <returns>Resultado de operación con lista de lab o mensaje de no encontrados.</returns>
        [HttpGet(nameof(GetAllLaboratories))]
        [ProducesResponseType(typeof(ApiOperationResultDto<List<LaboratoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllLaboratories(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Laboratories.GetAllLaboratoriesAsync(cancellationToken);
            return result.Success ? Ok(result) : NotFound(result);
        }
        /// <summary>
        /// Obtiene un laboratorio específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del lab.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
        /// <returns>Resultado con el lab encontrado o respuesta 404.</returns>
        [HttpGet(nameof(GetLaboratoryById) + "/{id:int}")]
        [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLaboratoryById(int id,CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Laboratories.GetLaboratoryByIdAsync(id, cancellationToken);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Crea un nuevo laboratorio en la base de datos.
        /// </summary>
        /// <param name="resource">Datos necesarios para crear el lab.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
        /// <returns>Resultado con el lab creado o error de validación.</returns>
        [HttpPost(nameof(CreateLaboratory))]
        [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLaboratory([FromBody] CreateLaboratoryDto resource,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiOperationResultDto<object>
                {
                    Success = false,
                    Code = StatusCodes.Status400BadRequest.ToString(),
                    Message = stringLocalizer["InvalidModel"]
                });
            }

            var result = await unitOfWork.Laboratories.CreateLaboratoryAsync(resource, cancellationToken);

            return result.Success ? Created(string.Empty, result) : BadRequest(result);
        }
        /// <summary>
        /// Actualiza los datos de un laboratorio existente.
        /// </summary>
        /// <param name="id">Identificador del lab a actualizar.</param>
        /// <param name="resource">Nuevos datos del lab.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
        /// <returns>Resultado con lab actualizado o mensaje de error.</returns>
        [HttpPut(nameof(UpdateLaboratory) + "/{id:int}")]
        [ProducesResponseType(typeof(ApiOperationResultDto<LaboratoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLaboratory(int id,[FromBody] UpdateLaboratoryDto resource,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiOperationResultDto<object>
                {
                    Success = false,
                    Code = StatusCodes.Status400BadRequest.ToString(),
                    Message = stringLocalizer["InvalidModel"]
                });
            }

            var result = await unitOfWork.Laboratories.UpdateLaboratoryAsync(id, resource, cancellationToken);

            if (result.Success)
            { 
                return Ok(result); 
            }
              

            return result.Code == StatusCodes.Status404NotFound.ToString() ? NotFound(result)  : BadRequest(result);
        }
        /// <summary>
        /// Elimina un laboratorio por su identificador.
        /// </summary>
        /// <param name="id">Identificador del lab a eliminar.</param>
        /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
        /// <returns>Resultado de éxito o respuesta 404 si no existe.</returns>
        [HttpDelete(nameof(DeleteLaboratory) + "/{id:int}")]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiOperationResultDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLaboratory(int id,CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Laboratories.DeleteLaboratoryAsync(id, cancellationToken);
            return result.Success
                ? Ok(result)
                : NotFound(result);
        }
    }
}

namespace UamHelpDeskPA.Mvc.Models
{
    public class ApiResponse<T>
    {
        /// Indica si la operación en el API fue exitosa.
        public bool Success { get; set; }

        /// Código HTTP retornado por el API en formato texto.
 
        public string Code { get; set; } = string.Empty;

        /// Mensaje funcional del API (éxito o error).

        public string Message { get; set; } = string.Empty;

        /// Resultado de la operación.

        public T? Result { get; set; }
    }

    /// Modelo para capturar la respuesta del login JWT.

    public class LoginResponseDto
    {
        /// Token JWT de acceso.

        public string AccessToken { get; set; } = string.Empty;

        /// Tipo de token, normalmente "Bearer".

        public string TokenType { get; set; } = "Bearer";

        /// Tiempo de expiración del token en segundos.

        public int ExpiresIn { get; set; }
    }
    public class LaboratoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public int BuildingFloor { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }

        public List<EquipmentDto> Equipments { get; set; } = new();
    }

    public class LaboratoryUpsertDto
    {
        public string Name { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public int BuildingFloor { get; set; }

        public int Capacity { get; set; }
    }
    public class EquipmentDto
    {
        public int Id { get; set; }

        public int LaboratoryId { get; set; }

        public string LaboratoryName { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string SerialNumber { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime? PurchaseDate { get; set; }

        public bool IsActive { get; set; }
    }
    public class EquipmentUpsertDto
    {
        public int LaboratoryId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string SerialNumber { get; set; } = string.Empty;

        public int Type { get; set; } 

        public int Status { get; set; } 

        public DateTime? PurchaseDate { get; set; }
    }
    public class ApiModels
    {
    }
}

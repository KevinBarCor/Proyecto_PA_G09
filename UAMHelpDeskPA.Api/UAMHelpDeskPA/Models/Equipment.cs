using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UamHelpDeskPA.Api.Models;
using UamHelpDeskPA.Models;

namespace UamHelpDeskPA.Api.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        public int LaboratoryId { get; set; } 

        [ForeignKey(nameof(LaboratoryId))]
        public Laboratory Laboratory { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        public EquipmentTypes Type { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
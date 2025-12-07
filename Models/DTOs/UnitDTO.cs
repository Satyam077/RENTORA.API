using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class UnitCreateDTO
    {
        [Required]
        public string OwnerId { get; set; }
        
        [Required]
        public string PropertyId { get; set; }
        
        public string TenantId { get; set; }
        
        [Required]
        public string UnitName { get; set; }
        
        [Required]
        public decimal RentAmount { get; set; }
        
        public decimal SecurityDeposit { get; set; }
        
        public bool IsOccupied { get; set; } = false;
        
        public int DueDay { get; set; } = 5;
        
        public string Notes { get; set; } = string.Empty;
        
        [Required]
        public string CreatedBy { get; set; }
    }

    public class UnitUpdateDTO
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        public string OwnerId { get; set; }
        
        [Required]
        public string PropertyId { get; set; }
        
        public string TenantId { get; set; }
        
        [Required]
        public string UnitName { get; set; }
        
        [Required]
        public decimal RentAmount { get; set; }
        
        public decimal SecurityDeposit { get; set; }
        
        public bool IsOccupied { get; set; } = false;
        
        public int DueDay { get; set; } = 5;
        
        public string Notes { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        public string UpdatedBy { get; set; }
    }
}


using RENTARA.API.Models;
using RENTORA.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class PropertyCreateDTO
    {
        [Required]
        public string OwnerId { get; set; }
        
        [Required]
        public string PropertyName { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public PropertyType Type { get; set; }
        
        [Required]
        public PropertyAddress Address { get; set; }
        
        public List<UnitModel> Units { get; set; } = new();
        public List<string> Images { get; set; } = new();
        public List<string> Documents { get; set; } = new();
        
        [Required]
        public decimal DefaultRentAmount { get; set; }
        
        public int DefaultDueDay { get; set; } = 5;
        public string Notes { get; set; }
        
        [Required]
        public string CreatedBy { get; set; }
    }

    public class PropertyUpdateDTO
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        public string OwnerId { get; set; }
        
        [Required]
        public string PropertyName { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public PropertyType Type { get; set; }
        
        [Required]
        public PropertyAddress Address { get; set; }
        
        public List<UnitModel> Units { get; set; } = new();
        public List<string> Images { get; set; } = new();
        public List<string> Documents { get; set; } = new();
        
        [Required]
        public decimal DefaultRentAmount { get; set; }
        
        public int DefaultDueDay { get; set; } = 5;
        public string Notes { get; set; }
        public bool IsActive { get; set; } = true;
        
        [Required]
        public string UpdatedBy { get; set; }
    }
}

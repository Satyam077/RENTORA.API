using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class FeaturesDto
    {
        [Required(ErrorMessage = "Feature name is required")]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class PlanDto
    {
        public string? Id { get; set; }
        [Required]
        public string PlanName { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int YearlyDiscount { get; set; }
        public bool IsMarkedAsPopular { get; set; }
        public List<string> FeatureIds { get; set; } = new List<string>();
    }
}

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
}

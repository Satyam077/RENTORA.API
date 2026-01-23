
namespace RENTORA.API.Models
{
    public class Features : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; }
        public string? ImageUrl { get; set; }
    }
}

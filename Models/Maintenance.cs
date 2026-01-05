using RENTORA.API.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;
using RENTARA.API.Models;

namespace RENTORA.API.Models
{
    public class Maintenance : BaseEntity
    {
        public string TenantId { get; set; }
        public string OwnerId { get; set; } = string.Empty; // Landlord/Owner ID for direct queries
        public string PropertyId { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string Category { get; set; } // Plumbing, Electrical, HVAC, Locks/Keys, Pest Control, General
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> PhotoUrls { get; set; } = new List<string>();
        public Priority Priority { get; set; } = Priority.Medium;
        public Status Status { get; set; } = Status.Open;
        public DateTime? ScheduledDate { get; set; } 
        public int UpdateCount { get; set; } = 0;
        public double? Rating { get; set; }
        
        [BsonIgnore]
        public PropertyModel? Property { get; set; }
        
        [BsonIgnore]
        public UnitModel? Unit { get; set; }
    }
}

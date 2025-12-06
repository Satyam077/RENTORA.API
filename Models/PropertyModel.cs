using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;

namespace RENTARA.API.Models
{
    public class PropertyModel : BaseEntity
    {
        public string OwnerId { get; set; } = string.Empty;
        public string PropertyName { get; set; }
        public string Description { get; set; }
        public PropertyType Type { get; set; }
        public PropertyAddress Address { get; set; }
        public List<UnitModel> Units { get; set; } = new();
        public List<string> Images { get; set; } = new();   // URLs
        public List<string> Documents { get; set; } = new(); // Agreement, Ownership docs, etc.
        public bool IsFullyOccupied { get; set; }
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public decimal DefaultRentAmount { get; set; }
        public int DefaultDueDay { get; set; } = 5;          // Rent due day (5th of every month)
        public string Notes { get; set; }
    }
    public class PropertyAddress
    {
        public string HouseNo { get; set; }
        public string Street { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Country { get; set; } = "India";
    }
}

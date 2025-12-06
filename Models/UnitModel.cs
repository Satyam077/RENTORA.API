
namespace RENTORA.API.Models
{
    public class UnitModel
    {
        public string PropertyId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string UnitName { get; set; }             // e.g., "Flat 101"
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public bool IsOccupied { get; set; } = false;
        public int DueDay { get; set; } = 5;
        public string Notes { get; set; } = string.Empty;


        //Optional Advanced Fields (for v2.0)
        //public decimal MaintenanceCharge { get; set; }
        //public bool HasLift { get; set; }
        //public bool HasParking { get; set; }
        //public int FloorCount { get; set; }
        //public string ReraNumber { get; set; }
        //public List<string> Amenities { get; set; }

    }
}


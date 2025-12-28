using RENTORA.API.Models.Enums;

namespace RENTORA.API.Models.DTOs
{
    /// <summary>
    /// DTO for updating maintenance status and priority
    /// </summary>
    public class MaintenanceUpdateDTO
    {
        public Status? Status { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }
}

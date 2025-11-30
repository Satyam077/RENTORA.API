namespace RENTORA.API.Models.DTOs
{
    public class OtpData
    {
        public string Code { get; set; }
        public DateTime GeneratedAt { get; set; }
        public bool IsUsed { get; set; } = false;
    }

}

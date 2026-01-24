namespace RENTORA.API.Models
{
    public class Plans : BaseEntity
    {
        public string PlanName { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int YearlyDiscount { get; set; } = 0;
        public bool IsMarkedAsPopular { get; set; } = false;
        public List<Features> Features { get; set; } = new List<Features>();
    }
}

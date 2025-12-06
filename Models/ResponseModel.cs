namespace RENTORA.API.Models
{
    public class ResponseModel
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object data { get; set; }
    }
}

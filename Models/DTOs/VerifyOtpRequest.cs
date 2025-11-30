using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email or mobile is required")]
        public string EmailOrMobile { get; set; }

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        public string OtpCode { get; set; }
    }
}

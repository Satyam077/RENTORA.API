using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email or mobile is required")]
        public string EmailOrMobile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

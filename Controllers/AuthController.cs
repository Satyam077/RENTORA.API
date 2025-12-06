using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registrationDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] string emailOrMobile)
        {
            if (string.IsNullOrWhiteSpace(emailOrMobile))
            {
                return BadRequest(new { success = false, message = CommonMessage.MessageError.Required });
            }

            var result = await _authService.SendOtpAsync(emailOrMobile);

            if (!result)
            {
                return BadRequest(new { success = false, message = CommonMessage.MessageError.FailedOtp });
            }

            return Ok(new { success = true, message =CommonMessage.MessageSuccess.OtpSent });
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyOtpAsync(request);

            if (!result)
            {
                return BadRequest(new { success = false, message = CommonMessage.MessageError.OtpExpired });
            }

            return Ok(new { success = true, message = CommonMessage.MessageSuccess.OtpVerified });
        }
    }
}

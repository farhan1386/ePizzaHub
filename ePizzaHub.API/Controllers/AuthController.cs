using ePizzaHub.BusinessLogic.Services;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result == null)
                {
                    return BadRequest(new { message = "Email is already registered in our system." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required fields." });
            }

            try
            {
                var authenticatedUser = await _userService.AuthenticateUserAsync(request.Email, request.Password);
                if (authenticatedUser == null)
                {
                    return Unauthorized(new { message = "Invalid email or password combination." });
                }

                return Ok(authenticatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Email address is required." });
            }

            try
            {
                var success = await _userService.GeneratePasswordResetTokenAsync(request.Email);
                return Ok(new { message = "If the account exists, a recovery link has been dispatched safely." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}

using CarStore.Applcation.DTOs.Authentication;
using CarStore.Applcation.Services;
using CarStore.Application.DTOs;
using CarStore.Application.Services;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;

        public AuthController(IAuthService authService, IMailService mailService)
        {
            _authService = authService;
            _mailService = mailService;
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> SignUpAsync([FromBody] RegisterRequest request)
        {
            try
            {
                if (request is null)
                    return BadRequest(new { Message = "Invalid register request" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _authService.Register(request);

                if (!string.IsNullOrEmpty(user.Message))
                    return BadRequest(new { Message = user.Message });

                var token = user.EmailVerificationToken;
                var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                    new { email = user.Email, token = token }, Request.Scheme);

                await _mailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

                return Ok(new { Message = "Registration successful. Please check your email for confirmation." });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> SignInAsync([FromBody] LoginRequest request)
        {
            try
            {
                if (request is null)
                    return BadRequest(new { Message = "Invalid login request" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.Login(request);
                return !string.IsNullOrEmpty(result.Message) ? BadRequest(new { Message = result.Message }) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("SignInWithGoogle")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> SignInWithGoogleAsync([FromBody] GoogleLoginRequest request)
        {
            if (request is null)
                return BadRequest(new { Message = "Invalid login request" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GoogleLogin(request);
            return !string.IsNullOrEmpty(result.Message) ? BadRequest(new { Message = result.Message }) : Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                var result = await _authService.ConfirmEmail(new ConfirmRequest { Email = email, Token = token });
                if (result)
                {
                    // Return HTML confirmation page
                    return Content(@"
                    <html>
                        <head>
                            <title>Email Confirmed</title>
                            <style>
                                body { font-family: Arial, sans-serif; text-align: center; padding: 50px; }
                                h1 { color: #4CAF50; }
                            </style>
                        </head>
                        <body>
                            <h1>Email Confirmed Successfully!</h1>
                            <p>Your email has been verified. You can now log in to your account.</p>
                        </body>
                    </html>", "text/html");
                }

                return BadRequest(new { Message = "Email confirmation failed" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("SignOut")]
        [Authorize]
        public async Task<ActionResult<string>> SignOutAsync()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (token is null)
                    return BadRequest(new { Message = "Token not found" });

                var result = await _authService.SignOut(token);

                return result.Contains("successfully") ? Ok(new { Message = result }) : BadRequest(new { Message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("RefreshToken")]
        [Authorize]
        public async Task<ActionResult<AuthResult>> RefreshTokenAsync([FromBody] string request)
        {
            var result = await _authService.RefreshToken(request);

            return !result.IsAuthenticated ? BadRequest(new { Message = result.Message }) : Ok(result);
        }

        [HttpPost("RevokeToken")]
        [Authorize]
        public async Task<ActionResult<string>> RevokeTokenAsync([FromBody] string request)
        {
            var result = await _authService.RevokeToken(request);

            return !result ? BadRequest(new { Message = "InValid Token!" }) : Ok(new { Message = "Token Revoked Successfully!" });

        }
    }
}

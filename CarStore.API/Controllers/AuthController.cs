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
        public async Task<ActionResult<AuthResult>> SignUpAsync(RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var user = await _authService.Register(request);

                var token = user.EmailVerificationToken;
                var confirmationLink = Url.Action("ConfirmEmail", "Auth", 
                    new { email = user.Email, token = token}, Request.Scheme);

                await _mailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

                return Ok(new { Message = "Registration successful. Please check your email for confirmation." });
            }
            catch (Exception e)
            {
                return BadRequest(new {Message = e.Message});
            }
        }
        
        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> SignInAsync(LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var  result = await _authService.Login(request);
                return !string.IsNullOrEmpty(result.Message) ? BadRequest(result.Message) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                var  result = await _authService.ConfirmEmail(new ConfirmRequest {Email = email, Token = token});
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

                return BadRequest(new {Message = "Email confirmation failed" });
            }
            catch (Exception e)
            {
                return BadRequest(new {Message = e.Message});
            }
        }

        [HttpPost("SignOut")]
        [Authorize]
        public async Task<ActionResult<string>> SignOutAsync()
        {
           try
           {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if(token is null)
                    return BadRequest("Token is missing");
                
                var result = await _authService.SignOut(token);

                return result.Contains("successfully") ? Ok(result) : BadRequest(result);
           }
           catch (Exception e)
           {
               return BadRequest(e.Message);
           }
        }

    }
}

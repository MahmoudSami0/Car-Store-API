using CarStore.Applcation.Services;
using CarStore.Application.Common.Consts;
using CarStore.Application.Common.Interrfaces;
using CarStore.Application.DTOs;
using CarStore.Application.Interfaces;
using CarStore.Application.Services;
using CarStore.Domain.Entities;
using CarStore.Domain.Helpers;
using CarStore.InfraStructure.Helper;
using Org.BouncyCastle.Ocsp;
using System.IdentityModel.Tokens.Jwt;

namespace CarStore.InfraStructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailService _mailService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUnitOfWork unitOfWork,
        IMailService mailService,
        IJwtTokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _mailService = mailService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResult> Register(RegisterRequest request)
    {
        var authResult = new AuthResult();
        if (!EmailValidator.IsValidEmail(request.email))
        {
            authResult.Message = "Invalid Email";
            return authResult;
        }

        if (await _unitOfWork.Users.FindByEmailAsync(request.email) is not null)
        {
            authResult.Message = "Email Already Exists";
            return authResult;
        }

        if (!string.IsNullOrEmpty(request.Phone))
        {
            if (!PhoneValidator.IsValid(request.Phone))
            {
                authResult.Message = "Invalid Phone Number";
                return authResult;
            }
            if (await _unitOfWork.Users.FindByPhoneAsync(request.Phone) is not null)
            {
                authResult.Message = "Phone Number Already In Use";
                return authResult;
            }
        }
        
        if (!PasswordValidator.IsValid(request.password))
            return new AuthResult{Message = "Password must contain at least \n" +
                          "8 Character, One Capital Character, One Small Character, One Special Character. \n" +
                          "EX: Example@123"};

        var newUser = new User
        {
            Name = request.FullName,
            Email = request.email,
            UserName = request.email.Split('@')[0],
            Phone = request.Phone,
            Password = PasswordHelper.HashPassword(request.password),
            EmailVerificationToken = GenerateEmailConfirmationToken(request.email),
            EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(1)
        };

        await _unitOfWork.Users.AddAsync(newUser);

        //await _mailService.SendEmailConfirmationAsync(newUser.Email, newUser.EmailVerificationToken);

        authResult.UserId = newUser.UserId;
        authResult.UserName = newUser.UserName;
        authResult.Email = newUser.Email;
        authResult.Phone = newUser.Phone!;
        authResult.EmailVerificationToken = newUser.EmailVerificationToken;
        authResult.EmailVerificationTokenExpires = newUser.EmailVerificationTokenExpires;

        return authResult;
    }

    public async Task<AuthResult> Login(LoginRequest request)
    {
        var authResult = new AuthResult();

        if (!EmailValidator.IsValidEmail(request.email))
        {
            authResult.Message = "Invalid Email";
            return authResult;
        }
        var user = await _unitOfWork.Users.FindAsync(u => u.Email == request.email, ["RefreshTokens"]);
        if (user is null || !PasswordHelper.VerifyPassword(request.password, user.Password))
        {
            authResult.Message = "Email Or Password Is Incorrect";
            return authResult;
        }
        if (!user.IsVerified)
        {
            authResult.Message = "Email is not verified";
            return authResult;
        }
        if (user.IsDeleted)
        {
            authResult.Message = "This account is locked contact us to unlock";
            return authResult;
        }

        var jwtSecurityToken = await _tokenGenerator.GenerateJwtToken(user);
        var roleList = await _unitOfWork.Roles.GetRolesAsync(user);

        authResult.IsAuthenticated = user.IsVerified;
        authResult.UserId = user.UserId;
        authResult.UserName = user.UserName;
        authResult.Email = user.Email;
        authResult.Phone = user.Phone!;
        authResult.Roles = roleList;
        authResult.Token = jwtSecurityToken;

        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            authResult.RefreshToken = activeRefreshToken.Token;
            authResult.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = _tokenGenerator.GenerateRefreshToken();
            authResult.RefreshToken = refreshToken.Token;
            authResult.RefreshTokenExpiration = refreshToken.ExpiresOn;
            
            user.RefreshTokens.Add(refreshToken);
            await _unitOfWork.Users.UpdateAsync(user);
        }
        return authResult;
    }

    public async Task<bool> ConfirmEmail(ConfirmRequest request)
    {
        var user = await _unitOfWork.Users.FindByEmailAsync(request.Email);
        if (user is null || user.EmailVerificationToken != request.Token || user.EmailVerificationTokenExpires < DateTime.UtcNow)
             throw new Exception("Invalid confirmation link") ;

        if(user.IsVerified) 
            throw new Exception("Email already confirmed");

        user.IsVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpires = null;
        
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.Users.AddToRoleAsync(user.UserId, Roles.User.GetDescription());

        return true;
    }

    public string GenerateEmailConfirmationToken(string email)
    {
        var token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        token = token.Replace("-", "");
        return token;
    }

    public async Task<string> SignOut(string token)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var expiration = jwtToken.ValidTo;

                var blackListToken = new BlacklistedToken
                {
                    Token = token,
                    Expiration = expiration
                };

                await _unitOfWork.blacklistedTokens.AddAsync(blackListToken);
            }
            return "Signed out successfully";
        }
        catch (Exception e)
        {
            return $"Error signing out: {e.Message}";
        }

    }
}
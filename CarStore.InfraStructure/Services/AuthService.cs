using AutoMapper;
using CarStore.Applcation.DTOs.Authentication;
using CarStore.Applcation.Services;
using CarStore.Application.Common.Consts;
using CarStore.Application.Common.Interrfaces;
using CarStore.Application.DTOs;
using CarStore.Application.Interfaces;
using CarStore.Application.Services;
using CarStore.Domain.Entities;
using CarStore.Domain.Helpers;
using CarStore.InfraStructure.Helper;
using Google.Apis.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace CarStore.InfraStructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailService _mailService;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;


    public AuthService(IUnitOfWork unitOfWork,
        IMailService mailService,
        IJwtTokenGenerator tokenGenerator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mailService = mailService;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
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

        var penddingUser = await _unitOfWork.PenddingUsers.FindAsync(u => u.Email == request.email);
        if(penddingUser is not null)
        {
            if(penddingUser.EmailVerificationTokenExpires < DateTime.UtcNow)
                await _unitOfWork.PenddingUsers.DeleteAsync(penddingUser);
            else
            {
                authResult.Message = "Email Already Exixts, Please Verify It";
                return authResult;
            }
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
            var pendUser = await _unitOfWork.PenddingUsers.FindAsync(u => u.Phone == request.Phone);
            if(pendUser is not null)
            {
                if (pendUser.EmailVerificationTokenExpires < DateTime.UtcNow)
                    await _unitOfWork.PenddingUsers.DeleteAsync(pendUser);
                else
                {
                    authResult.Message = "Phone Number Pending To Another User, Please Verify Your Email Or Wait For 1 Hour";
                    return authResult;
                }
            }
        }

        if (!PasswordValidator.IsValid(request.password))
            return new AuthResult
            {
                Message = "Password must contain at least \n" +
                          "8 Character, One Capital Character, One Small Character, One Special Character. \n" +
                          "EX: Example@123"
            };


        var newUser = _mapper.Map<PenddingUser>(request);

        //newUser.AuthProvider = "Email";
        newUser.IsVerified = false;
        newUser.EmailVerificationToken = GenerateEmailConfirmationToken(request.email);
        newUser.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(1);


        await _unitOfWork.PenddingUsers.AddAsync(newUser);

        //await _mailService.SendEmailConfirmationAsync(newUser.Email, newUser.EmailVerificationToken);

        authResult = _mapper.Map<AuthResult>(newUser);


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
        if (user is null || !PasswordHelper.VerifyPassword(request.password, user.PasswordHash))
        {
            authResult.Message = "Email Or Password Is Incorrect";
            return authResult;
        }
        //if (!user.IsVerified)
        //{
        //    authResult.Message = "Email is not verified";
        //    return authResult;
        //}
        if (user.IsDeleted)
        {
            authResult.Message = "This account is locked contact us to unlock";
            return authResult;
        }

        var jwtSecurityToken = await _tokenGenerator.GenerateJwtToken(user);
        var roleList = await _unitOfWork.Roles.GetRolesAsync(user);

        authResult = _mapper.Map<AuthResult>(user);

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

    public async Task<AuthResult> GoogleLogin(GoogleLoginRequest request)
    {
        var authResult = new AuthResult();
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>
                {
                    "462255915759-dlojksj0qrrn8kjj6j2ehvjs0pvi67lp.apps.googleusercontent.com"
                }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var userId = payload.Subject;
            var email = payload.Email;
            var name = payload.Name;

            var user = await _unitOfWork.Users.FindAsync(u => u.Email == email, ["RefreshTokens"]);
            if (user is null)
            {
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    UserName = email.Split('@')[0],
                    AuthProvider = "Google",
                    GoogleId = userId,
                };

                await _unitOfWork.Users.AddAsync(newUser);
                await _unitOfWork.Users.AddToRoleAsync(newUser.UserId, Roles.User.GetDescription());

                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                authResult = _mapper.Map<AuthResult>(newUser);

                authResult.Roles = await _unitOfWork.Roles.GetRolesAsync(newUser);
                authResult.Token = await _tokenGenerator.GenerateJwtToken(newUser);
                authResult.RefreshToken = refreshToken.Token;
                authResult.RefreshTokenExpiration = refreshToken.ExpiresOn;

                newUser.RefreshTokens.Add(refreshToken);
                await _unitOfWork.Users.UpdateAsync(newUser);

                return authResult;
            }

            var jwtSecurityToken = await _tokenGenerator.GenerateJwtToken(user);
            var roleList = await _unitOfWork.Roles.GetRolesAsync(user);


            authResult = _mapper.Map<AuthResult>(user);

            authResult.AuthProvider = user.AuthProvider;
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
        catch (Exception ex)
        {
            authResult.Message = ex.Message;
            return authResult;
        }
    }

    public async Task<bool> ConfirmEmail(ConfirmRequest request)
    {
        var user = await _unitOfWork.PenddingUsers.FindAsync(u => u.Email == request.Email);
        if (user is null || user.EmailVerificationToken != request.Token)
            throw new Exception("Invalid confirmation link");

        if(user.EmailVerificationTokenExpires < DateTime.UtcNow)
            throw new Exception("Confirmation link has expired");

        if (user.IsVerified)
            throw new Exception("Email already confirmed");

        user.IsVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpires = null;

        var newUser = _mapper.Map<User>(user);
        newUser.UserName = user.Email.Split('@')[0];
        newUser.AuthProvider = "Email";

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.Users.AddToRoleAsync(newUser.UserId, Roles.User.GetDescription());

        await _unitOfWork.PenddingUsers.DeleteAsync(user);

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

    public async Task<AuthResult> RefreshToken(string token)
    {
        var authResult = new AuthResult();
        if (string.IsNullOrEmpty(token))
        {
            authResult.IsAuthenticated = false;
            authResult.Message = "Invalid Refresh Token";
            return authResult;
        }

        var user = await _unitOfWork.Users.FindAsync(u => u.RefreshTokens.Any(t => t.Token == token), ["RefreshTokens"]);

        if (user is null)
        {
            authResult.IsAuthenticated = false;
            authResult.Message = "Invalid Refresh Token";
            return authResult;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            authResult.IsAuthenticated = false;
            authResult.Message = "Invalid Refresh Token";
            return authResult;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _unitOfWork.Users.UpdateAsync(user);

        var jwtToken = await _tokenGenerator.GenerateJwtToken(user);
        var roles = await _unitOfWork.Roles.GetRolesAsync(user);

        authResult = _mapper.Map<AuthResult>(user);
        authResult.IsAuthenticated = true;
        authResult.Roles = roles;
        authResult.Token = jwtToken;
        authResult.RefreshToken = newRefreshToken.Token;
        authResult.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

        return authResult;
    }

    public async Task<bool> RevokeToken(string token)
    {
        var user = await _unitOfWork.Users.FindAsync(u => u.RefreshTokens.Any(t => t.Token == token), ["RefreshTokens"]);

        if (user is null)
            return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);

        return true;
    }
}
using CarStore.Application.DTOs;
using CarStore.Domain.Entities;

namespace CarStore.Application.Services;

public interface IAuthService
{
    Task<AuthResult> Register(RegisterRequest request);
    Task<AuthResult> Login(LoginRequest request);
    Task<bool> ConfirmEmail(ConfirmRequest request);
    string GenerateEmailConfirmationToken(string email);
    Task<string> SignOut(string token);
}
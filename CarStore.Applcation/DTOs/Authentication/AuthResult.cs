using CarStore.Application.DTOs.User;
using System.Text.Json.Serialization;

namespace CarStore.Application.DTOs;

public class AuthResult : UserDto
{
    public string? Message { get; set; }
    
    public string AuthProvider { get; set; }
    public List<string>? Roles { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    
    [JsonIgnore]
    public string? EmailVerificationToken { get; set; }
    [JsonIgnore]
    public DateTime? EmailVerificationTokenExpires { get; set; }
}
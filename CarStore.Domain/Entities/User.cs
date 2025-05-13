namespace CarStore.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; }
    public bool IsVerified { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpires { get; set; }
    public bool IsDeleted { get; set; }
    
    public List<UserRoles> UserRoles { get; set; }
    public List<RefreshToken>? RefreshTokens { get; set; }
}
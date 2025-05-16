namespace CarStore.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public string AuthProvider { get; set; }
    public string? GoogleId { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public bool IsDeleted { get; set; }
    
    public ICollection<UserRoles> UserRoles { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<Rate>? Rates { get; set; }
    public ICollection<Favorite>? Favorites { get; set; }
}
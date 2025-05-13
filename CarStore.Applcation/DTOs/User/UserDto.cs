namespace CarStore.Application.DTOs.User;

public class UserDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string? Phone { get; set; }
    public byte[]? ProfilePicture { get; set; }
}
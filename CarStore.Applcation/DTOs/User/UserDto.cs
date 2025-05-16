using CarStore.Applcation.DTOs.User;

namespace CarStore.Application.DTOs.User;

public class UserDto : BasicUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}
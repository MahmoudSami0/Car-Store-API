namespace CarStore.Application.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; }
    public string email { get; set; }
    public string? Phone { get; set; }
    public string password { get; set; }
}
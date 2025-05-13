namespace CarStore.Domain.Entities;

public class Role
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }

    public List<UserRoles> UserRoles { get; set; }
}
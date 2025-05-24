namespace CarStore.Applcation.DTOs.Role
{
    public class RoleDto : AddRoleDto
    {
        public Guid RoleId { get; set; }
        public List<string> UserNames { get; set; }
    }
}

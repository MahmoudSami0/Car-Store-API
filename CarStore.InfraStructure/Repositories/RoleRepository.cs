using CarStore.Application.Interfaces;
using CarStore.InfraStructure.Data;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Repositorries;

namespace CarStore.InfraStructure.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    private readonly CarStoreDbContext _context;
    public RoleRepository(CarStoreDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<string>?> GetRolesAsync(User user)
    {
        var roles = await CustomFindAsync<UserRoles, string>(
            predicate: ur => ur.UserId == user.UserId,
            selector: r => r.Role.RoleName);

        return roles;
    }
}
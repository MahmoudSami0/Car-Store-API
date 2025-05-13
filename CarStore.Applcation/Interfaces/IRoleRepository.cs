using CarStore.Domain.Entities;

namespace CarStore.Application.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<List<string>?> GetRolesAsync(User user);
}
using CarStore.Domain.Entities;

namespace CarStore.Application.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByNameAsync(string userName);
    Task<User?> FindByPhoneAsync(string phone);
    Task<string?> AddToRoleAsync(Guid userId, string role);
}
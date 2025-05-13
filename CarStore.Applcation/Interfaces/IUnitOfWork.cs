
using CarStore.Domain.Entities;

namespace CarStore.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IBaseRepository<UserRoles> UserRoles { get; }
    IBaseRepository<BlacklistedToken> blacklistedTokens { get; }
    Task SaveChangesAsync();
}
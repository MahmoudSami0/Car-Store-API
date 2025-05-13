using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Data;
using CarStore.InfraStructure.Repositories;

namespace CarStore.InfraStructure.Repositorries;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly CarStoreDbContext _context;
    public IUserRepository Users { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IBaseRepository<UserRoles> UserRoles { get; private set; }
    public IBaseRepository<BlacklistedToken> blacklistedTokens { get; private set; }

    public UnitOfWork(CarStoreDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Roles = new RoleRepository(_context);
        UserRoles = new BaseRepository<UserRoles>(_context);
        blacklistedTokens = new BaseRepository<BlacklistedToken>(_context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
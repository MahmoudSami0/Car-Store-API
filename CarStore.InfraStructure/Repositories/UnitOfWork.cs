using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Data;
using CarStore.InfraStructure.Repositories;

namespace CarStore.InfraStructure.Repositorries;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly CarStoreDbContext _context;
    public IUserRepository Users { get; private set; }
    public IBaseRepository<PenddingUser> PenddingUsers { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IBaseRepository<UserRoles> UserRoles { get; private set; }
    public IBaseRepository<Favorite> Favorites { get; private set; }
    public IBaseRepository<Rate> Rates { get; private set; }
    public IBaseRepository<CarBrand> CarBrands { get; private set; }
    public IBaseRepository<CarModel> CarModels { get; private set; }
    public IBaseRepository<CarFeatures> CarFeatures { get; private set; }
    public IBaseRepository<RefreshToken> RefreshTokens { get; private set; }
    public IBaseRepository<BlacklistedToken> blacklistedTokens { get; private set; }

    public UnitOfWork(CarStoreDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        PenddingUsers = new BaseRepository<PenddingUser>(_context);
        Roles = new RoleRepository(_context);
        UserRoles = new BaseRepository<UserRoles>(_context);
        Favorites = new BaseRepository<Favorite>(_context);
        Rates = new BaseRepository<Rate>(_context);
        CarBrands = new BaseRepository<CarBrand>(_context);
        CarModels = new BaseRepository<CarModel>(_context);
        CarFeatures = new BaseRepository<CarFeatures>(_context);
        RefreshTokens = new BaseRepository<RefreshToken>(_context);
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
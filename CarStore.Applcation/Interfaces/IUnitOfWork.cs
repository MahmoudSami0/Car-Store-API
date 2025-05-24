
using CarStore.Applcation.Interfaces;
using CarStore.Domain.Entities;

namespace CarStore.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBaseRepository<PenddingUser> PenddingUsers { get; }
    IRoleRepository Roles { get; }
    IBaseRepository<UserRoles> UserRoles { get; }
    IBaseRepository<Favorite> Favorites { get; }
    IRateRepository Rates { get; }
    IBaseRepository<CarBrand> CarBrands { get; }
    IBaseRepository<CarFeatures> CarFeatures { get; }
    IBaseRepository<CarModel> CarModels { get; }
    IFeatureRepository Features { get; }
    IBaseRepository<ModelGallery> ModelGalleries { get; }
    IBaseRepository<RefreshToken> RefreshTokens { get; }
    IBaseRepository<BlacklistedToken> blacklistedTokens { get; }
    Task SaveChangesAsync();
}
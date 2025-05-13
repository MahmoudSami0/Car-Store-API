using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarStore.InfraStructure.Data;

public class CarStoreDbContext : DbContext
{
    public CarStoreDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<BlacklistedToken> blacklistedTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarStoreDbContext).Assembly);
    }
}
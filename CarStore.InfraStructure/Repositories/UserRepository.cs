using CarStore.Application.Interfaces;
using CarStore.InfraStructure.Data;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Repositorries;
using Microsoft.EntityFrameworkCore;

namespace CarStore.InfraStructure.Repositories;

public class UserRepository : BaseRepository<User> ,IUserRepository
{
    private readonly CarStoreDbContext _context;

    public UserRepository(CarStoreDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByNameAsync(string userName)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> FindByPhoneAsync(string phone)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<string?> AddToRoleAsync(Guid userId, string roleName)
    {
        var user = await FindAsync(u => u.UserId == userId, ["UserRoles"]);
        var role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        if (user is null || role is null)
            return "Invalid User Id Or Role";
        
        if (user.UserRoles.Any(r => r.RoleId == role.RoleId))
            return "User Already Assigned To This Role";
        
        var userRole = new UserRoles
        {
            UserId = user.UserId,
            RoleId = role.RoleId
        };
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
        return "User Added To Role Successfully";
    }
}
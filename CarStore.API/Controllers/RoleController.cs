using AutoMapper;
using CarStore.Applcation.DTOs.Pagination;
using CarStore.Applcation.DTOs.Role;
using CarStore.Application.DTOs.User;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class RoleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<IEnumerable<RoleDto>>>> GetRolesAsync([FromQuery] PaginationRequest request)
        {
            try
            {
                var query = await _unitOfWork.Roles.GetAllAsync();
                var totalCount = await query.CountAsync();

                var roles = await _unitOfWork.Roles.CustomFindAsync<Role, RoleDto>(
                    selector: r => new RoleDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        UserNames = r.UserRoles.Select(ur => ur.User.UserName).ToList(),
                    });

                var items = roles.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var result = new PaginationResponse<RoleDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return result == null ? NotFound(new { Message = "No roles found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var role = await _unitOfWork.Roles.FindAsync(r => r.RoleId == id, ["UserRoles"]);

                if (role == null)
                    return NotFound(new { Message = "Role not found" });

                var result = _mapper.Map<RoleDto>(role);
                result.UserNames = new List<string>();
                foreach (var userRole in role.UserRoles)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userRole.UserId);
                    result.UserNames.Add(user.UserName);
                }

                return Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("UsersInRole/{id}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<UserDto>>>> GetUsersInRoleAsync(Guid id, [FromQuery] PaginationRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var query = await _unitOfWork.UserRoles.FindAllAsync(ur => ur.RoleId == id);
                var totalCount = query.Count();

                var users = await _unitOfWork.UserRoles.CustomFindAsync<UserRoles, UserDto>(
                    predicate: ur => ur.RoleId == id,
                    selector: ur => new UserDto
                    {
                        UserId = ur.UserId,
                        Email = ur.User.Email,
                        Name = ur.User.Name,
                        UserName = ur.User.UserName,
                        Phone = ur.User.Phone,
                        ProfilePicture = ur.User.ProfilePicture
                    });

                var items = users.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var result = new PaginationResponse<UserDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return result == null ? NotFound(new { Message = "No users assigned to this role" }) : Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddRoleAsync([FromBody] AddRoleDto request)
        {
            try
            {
                if (request is null)
                    return BadRequest(new { Message = "Invalid request" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existedRole = await _unitOfWork.Roles.FindAsync(r => r.RoleName == request.RoleName);

                if (existedRole is not null)
                    return BadRequest(new { Message = "Role already exists" });

                var role = new Role
                {
                    RoleName = request.RoleName,
                };

                await _unitOfWork.Roles.AddAsync(role);
                return Ok(new { Message = "Role added successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("AddUserToRole/{userId}/{roleId}")]
        public async Task<ActionResult<string>> AddUserToRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                if (userId == Guid.Empty || roleId == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (user is null || role is null)
                    return NotFound(new { Message = "User or role not found" });

                var existUserRole = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existUserRole is not null)
                    return BadRequest(new { Message = "User already assigned to this role" });

                var userRole = new UserRoles
                {
                    UserId = userId,
                    RoleId = roleId
                };

                await _unitOfWork.UserRoles.AddAsync(userRole);
                return Ok(new { Message = "User assigned to role successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateRoleAsync(Guid id, [FromForm] AddRoleDto request)
        {
            try
            {
                if (id == Guid.Empty || request is null)
                    return BadRequest(new { Message = "Invalid request" });

                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role is null)
                    return NotFound(new { Message = "Role not found" });

                var updatedRole = await _unitOfWork.Roles.FindAsync(r => r.RoleName == request.RoleName);

                if (updatedRole is not null && role.RoleName.ToLower() != request.RoleName.ToLower())
                    return BadRequest(new { Message = "Role already exists" });

                role.RoleName = request.RoleName;
                await _unitOfWork.Roles.UpdateAsync(role);
                return Ok(new { Message = "Role updated successfully" });

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteRoleAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role is null)
                    return NotFound(new { Message = "Role not found" });

                await _unitOfWork.Roles.DeleteAsync(role);

                var userRoles = await _unitOfWork.UserRoles.FindAllAsync(ur => ur.RoleId == id);
                foreach (var user in userRoles)
                {
                    await _unitOfWork.UserRoles.DeleteAsync(user);
                }

                return Ok(new { Message = "Role deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("RemoveUserFromRole/{userId}/{roleId}")]
        public async Task<ActionResult<string>> RemoveUserFromRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                if (userId == Guid.Empty || roleId == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                if (user is null || role is null)
                    return NotFound(new { Message = "User or role not found" });

                var existUserRole = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existUserRole is null)
                    return BadRequest(new { Message = "User not assigned to this role" });

                await _unitOfWork.UserRoles.DeleteAsync(existUserRole);
                return Ok(new { Message = "User removed from role successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}

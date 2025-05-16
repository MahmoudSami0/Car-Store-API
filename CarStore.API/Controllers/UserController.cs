using AutoMapper;
using CarStore.Applcation.DTOs.User;
using CarStore.Application.DTOs.User;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using CarStore.Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("Users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users is null || users.Count() < 1 ? NotFound(new { Message = "No users found" }) : Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user is null ? NotFound(new { Message = "User not found" }) : Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet("User/me")]
        public async Task<ActionResult<UserDto>> GetUserByTokenAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Unauthorized" });

            var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPut("User/Update/{id}")]
        public async Task<ActionResult<string>> UpdateUserAsync(Guid id, [FromBody] BasicUserDto model)
        {
            if (model is null)
                return BadRequest(new { Message = "Invalid update request" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(id.ToString()))
                return Unauthorized(new { Message = "Unauthorized" });

            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (model.UserName != user.UserName && await _unitOfWork.Users.FindAsync(u => u.UserName.ToLower() == model.UserName.ToLower()) != null)
            {
                return BadRequest(new { Message = "Username already exists" });
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                if (!PhoneValidator.IsValid(model.Phone))
                    return BadRequest(new { Message = "Invalid phone number" });
                if (model.Phone != user.Phone && await _unitOfWork.Users.FindAsync(u => u.Phone == model.Phone) != null)
                {
                    return BadRequest(new { Message = "Phone number already exists" });
                }
            }

            var modifiedUser = _mapper.Map<User>(model);
            await _unitOfWork.Users.UpdateAsync(modifiedUser);
            return Ok(new { Message = "User updated successfully" });

        }

        [HttpPut("User/Update")]
        public async Task<ActionResult<string>> UpdateUserAsync([FromBody] BasicUserDto model)
        {
            if(model is null)
                return BadRequest(new { Message = "Invalid update request" });
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Unauthorized" });

            var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));

            if (user == null)
                return NotFound(new { Message = "User not found" });

            if(model.UserName != user.UserName && await _unitOfWork.Users.FindAsync(u => u.UserName.ToLower() == model.UserName.ToLower()) != null)
            {
                return BadRequest(new { Message = "Username already exists" });
            }
            if(!string.IsNullOrEmpty(model.Phone))
            {
                if(!PhoneValidator.IsValid(model.Phone))
                    return BadRequest(new { Message = "Invalid phone number" });
                if(model.Phone != user.Phone && await _unitOfWork.Users.FindAsync(u => u.Phone == model.Phone) != null)
                {
                    return BadRequest(new { Message = "Phone number already exists" });
                }
            }
               
            var modifiedUser = _mapper.Map<User>(model);
            await _unitOfWork.Users.UpdateAsync(modifiedUser);
            return Ok(new { Message = "User updated successfully" });

        }

        //TODO: Deactivate/Activate user Endpoint

        [HttpDelete("User/Delete/{id}")] 
        public async Task<ActionResult<string>> DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            user.IsDeleted = true;
            await _unitOfWork.UserRoles.DeleteWhereAsync(ur => ur.UserId == id);
            await _unitOfWork.Favorites.DeleteWhereAsync(f => f.UserId == id);
            await _unitOfWork.RefreshTokens.DeleteWhereAsync(rt => rt.UserId == id);
            await _unitOfWork.Rates.DeleteWhereAsync(r => r.UserId == id);

            await _unitOfWork.Users.DeleteAsync(user);
            return Ok(new { Message = "User deleted successfully" });
        }
    }
}

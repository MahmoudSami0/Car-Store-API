using AutoMapper;
using CarStore.Applcation.DTOs.Favorite;
using CarStore.Applcation.DTOs.Favourite;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("ToggleFavorite/{UserId}/{CarId}")]
        public async Task<ActionResult<string>> ToggleFavoriteAsync([FromRoute]ToggleFavoriteDto request)
        {
            if (request is null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            var car = await _unitOfWork.CarModels.GetByIdAsync(request.CarId);

            if (user is null || car is null)
            {
                return NotFound(new { Message = "User Or Car not found" });
            }

            var favorite = await _unitOfWork.Favorites.FindAsync(f => f.UserId == request.UserId && f.CarId == request.CarId);

            if (favorite is not null)
            {
                if (favorite.IsRemoved)
                {
                    favorite.IsRemoved = false;
                    await _unitOfWork.Favorites.UpdateAsync(favorite);
                    return Ok(new { Message = "Car added to favorites successfully" });
                }
                else
                {
                    favorite.IsRemoved = true;
                    await _unitOfWork.Favorites.UpdateAsync(favorite);
                    return Ok(new { Message = "Car removed from favorites successfully" });
                }
            }

            var newFavorite = _mapper.Map<Favorite>(request);

            await _unitOfWork.Favorites.AddAsync(newFavorite);
            return Ok(new { Message = "Car added to favorites successfully" });


        }

        [HttpPost("ToggleFavorite/{CarId}")]
        public async Task<ActionResult<string>> ToggleFavoriteAsync(Guid CarId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Unauthorized" });
            }

            if (CarId == Guid.Empty)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            var car = await _unitOfWork.CarModels.GetByIdAsync(CarId);

            if (car is null)
            {
                return NotFound(new { Message = "Car not found" });
            }

            var favorite = await _unitOfWork.Favorites.FindAsync(f => f.UserId == Guid.Parse(userId) && f.CarId == CarId);

            if (favorite is not null)
            {
                if (favorite.IsRemoved)
                {
                    favorite.IsRemoved = false;
                    await _unitOfWork.Favorites.UpdateAsync(favorite);
                    return Ok(new { Message = "Car added to favorites successfully" });
                }
                else
                {
                    favorite.IsRemoved = true;
                    await _unitOfWork.Favorites.UpdateAsync(favorite);
                    return Ok(new { Message = "Car removed from favorites successfully" });
                }
            }

            var newFavorite = new Favorite { 
                UserId = Guid.Parse(userId),
                CarId = CarId 
            };

            await _unitOfWork.Favorites.AddAsync(newFavorite);
            return Ok(new { Message = "Car added to favorites successfully" });

        }

        [HttpGet("Favorites/me")]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavoritesAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Unauthorized" });

            var favorites = await _unitOfWork.Favorites.CustomFindAsync<Favorite, FavoriteDto>(
                predicate: f => f.UserId == Guid.Parse(userId) && f.IsRemoved == false,
                selector: f => new FavoriteDto
                {
                    UserId = f.UserId,
                    CarId = f.CarId,
                    CarBrand =f.CarModel.CarBrand.BrandName,
                    ModelName = f.CarModel.ModelName,
                    YearOfProduction = f.CarModel.YearOfProduction,
                    Price = f.CarModel.Price,
                    IsFeatured = f.CarModel.IsFeatured,
                    IsRecommended = f.CarModel.IsRecommended,
                    Description = f.CarModel.Description,
                    ImageUrls = f.CarModel.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList()

                });

            return favorites == null || favorites.Count < 1 ? NotFound(new { Message = "No favorites found" }) : Ok(favorites);
            
        }
    }
}

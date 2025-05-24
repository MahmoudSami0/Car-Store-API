using CarStore.Applcation.DTOs.Pagination;
using CarStore.Applcation.DTOs.Rate;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("CarRate/{carId}")]
        public async Task<ActionResult<PaginationResponse<RateResponseDto>>> GetCarRating(Guid carId, [FromQuery] PaginationRequest request)
        {
            try
            {
                if (carId == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                if (await _unitOfWork.CarModels.GetByIdAsync(carId) is null)
                    return NotFound(new { Message = "Car not found" });

                var query = await _unitOfWork.Rates.FindAllAsync(r => r.CarModelId == carId);
                var totalCount = query.Count();

                var rates = await _unitOfWork.Rates.CustomFindAsync<Rate, RateResponseDto>(
                    predicate: r => r.CarModelId == carId,
                    selector: r => new RateResponseDto
                    {
                        UserId = r.UserId,
                        UserName = r.User.UserName,
                        Email = r.User.Email,
                        CarModelId = r.CarModelId,
                        CarModel = r.CarModel.ModelName,
                        RateValue = r.RateValue,
                    });

                var items = rates.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var result = new PaginationResponse<RateResponseDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return result is null ? NotFound(new { Message = "No rates found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("Average/{carId}")]
        public async Task<ActionResult<double>> GetAverageRate(Guid carId)
        {
            try
            {
                if (carId == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var averageRate = await _unitOfWork.Rates.GetAverageRate(carId);

                return averageRate == 0 ? NotFound(new { Message = "No rates found" }) : Ok(new { AverageRate = averageRate });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddUpdateRateAsync(AddRateDto request)
        {
            try
            {
                if (request is null)
                    return BadRequest(new { Message = "Invalid request" });

                if (request.RateValue < 0 || request.RateValue > 5)
                    return BadRequest("Rating must be between 0 and 5");

                var existedRate = await _unitOfWork.Rates.FindAsync(r => r.UserId == request.UserId && r.CarModelId == request.CarModelId);

                if (existedRate is not null)
                {
                    existedRate.RateValue = request.RateValue;
                    await _unitOfWork.Rates.UpdateAsync(existedRate);
                    return Ok(new { Message = "Rate updated successfully" });
                }

                var rate = new Rate
                {
                    UserId = request.UserId,
                    CarModelId = request.CarModelId,
                    RateValue = request.RateValue
                };
                await _unitOfWork.Rates.AddAsync(rate);
                return Ok(new { Message = "Rate added successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{carId}/{userId}")]
        public async Task<ActionResult<string>> DeleteRateAsync(Guid carId, Guid userId)
        {
            try
            {
                if (carId == Guid.Empty || userId == Guid.Empty)
                    return BadRequest(new { Message = "Invalid request" });

                var car = await _unitOfWork.CarModels.GetByIdAsync(carId);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (car is null || user is null)
                    return NotFound(new { Message = "Car or user not found" });

                var existRate = await _unitOfWork.Rates.FindAsync(r => r.UserId == userId && r.CarModelId == carId);
                
                if (existRate is null)
                    return BadRequest(new { Message = "Rate not found" });

                await _unitOfWork.Rates.DeleteAsync(existRate);
                return Ok(new { Message = "Rate deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}
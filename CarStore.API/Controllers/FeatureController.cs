using AutoMapper;
using CarStore.Applcation.DTOs.Feature;
using CarStore.Applcation.DTOs.Pagination;
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
    public class FeatureController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeatureController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<FeatureDto>>> GetFeaturesAsync([FromQuery] PaginationRequest request)
        {
            try
            {
                var query = await _unitOfWork.Features.GetAllAsync();
                var totalCount = await query.CountAsync();

                var features = await _unitOfWork.Features.CustomFindAsync<Feature, FeatureDto>(
                    predicate: f => f.IsDeleted == false,
                    selector: f => new FeatureDto
                    {
                        FeatureId = f.FeatureId,
                        FeatureName = f.FeatureName,
                    });

                var items = features.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var result = new PaginationResponse<FeatureDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return result == null ? NotFound(new { Message = "No features found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeatureDto>> GetFeatureByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return NotFound(new { Message = "Invalid request" });

                var feature = await _unitOfWork.Features.FindAsync(f => f.FeatureId == id && f.IsDeleted == false);

                if (feature == null)
                    return NotFound(new { Message = "Feature not found" });

                var result = _mapper.Map<FeatureDto>(feature);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddFeatureAsync(AddFeatureDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { Message = "Invalid request" });

                var requestFeature = await _unitOfWork.Features.FindAsync(cf => cf.FeatureName.ToLower() == request.FeatureName.ToLower(), ["CarFeatures"]);

                if (requestFeature != null)
                {
                    if (requestFeature.IsDeleted == false)
                        return BadRequest(new { Message = "Feature already exists" });

                    requestFeature.IsDeleted = false;
                    foreach (var carfeature in requestFeature.CarFeatures)
                    {
                        carfeature.IsDeleted = false;
                        await _unitOfWork.CarFeatures.UpdateAsync(carfeature);
                    }
                    await _unitOfWork.Features.UpdateAsync(requestFeature);
                    return Ok(new { Message = "Feature added successfully" });
                }

                var feature = _mapper.Map<Feature>(request);

                await _unitOfWork.Features.AddAsync(feature);
                return Ok(new { Message = "Feature added successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("AddFeatureToCar")]
        public async Task<ActionResult<string>> AddFeatureToCarAsync([FromQuery]ToggleFeatureFromCarDto request)
        {
            try
            {
                if (request is null)
                    return BadRequest(new { Message = "Invaild request" });

                var result = await _unitOfWork.Features.AddFeatureToCar(request);

                return result.Contains("successfully") ? Ok(new { Message = result }) : BadRequest(new { Message = result });

            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateFeatureAsync(Guid id, AddFeatureDto request)
        {
            try
            {
                if (id == Guid.Empty || request == null)
                    return BadRequest(new { Message = "Invalid request" });

                var feature = await _unitOfWork.Features.GetByIdAsync(id);
                if (feature == null)
                    return NotFound(new { Message = "Feature not found" });

                if (await _unitOfWork.Features.FindAsync(f => f.FeatureName.ToLower() == request.FeatureName.ToLower()) != null && feature.FeatureName.ToLower() != request.FeatureName.ToLower())
                    return BadRequest(new { Message = "Feature already exists" });

                if (await _unitOfWork.Features.FindAsync(f => f.FeatureName.ToLower() == request.FeatureName.ToLower()) != null && feature.FeatureName.ToLower() == request.FeatureName.ToLower())
                    return NoContent();

                feature.FeatureName = request.FeatureName;
                await _unitOfWork.Features.UpdateAsync(feature);
                return Ok(new { Message = "Feature updated successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteFeatureAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invaild request" });

                var feature = await _unitOfWork.Features.FindAsync(f => f.FeatureId == id, ["CarFeatures"]);

                if (feature == null)
                    return NotFound(new { Message = "Feature not found" });

                feature.IsDeleted = true;

                foreach (var car in feature.CarFeatures)
                {
                    car.IsDeleted = true;
                    await _unitOfWork.CarFeatures.UpdateAsync(car);
                }

                await _unitOfWork.Features.UpdateAsync(feature);
                return Ok(new { Message = "Feature deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("RemoveFeatureFromCar")]
        public async Task<ActionResult<string>> RemoveFeatureFromCarAsync([FromQuery] ToggleFeatureFromCarDto request)
        {
            if(request == null)
                return BadRequest(new { Message = "Invalid request" });

            var result = await _unitOfWork.Features.RemoveFeatureFromCar(request);
            return result.Contains("successfully") ? Ok(new { Message = result }) : BadRequest(new { Message = result });
        }
    }
}
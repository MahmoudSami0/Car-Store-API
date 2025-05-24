using AutoMapper;
using CarStore.Applcation.DTOs.Car;
using CarStore.Applcation.DTOs.Pagination;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CarController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<CarDto>>> GetCarsAsync(
            [FromQuery] PaginationRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var query = await _unitOfWork.CarModels.GetAllAsync();
                var totalCount = await query.CountAsync();

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Unauthorized" });

                var cars = await _unitOfWork.CarModels.CustomFindAsync<CarModel, CarDto>(
                predicate: c => c.IsDeleted == false,
                selector: c => new CarDto
                {
                    CarId = c.CarId,
                    ModelName = c.ModelName,
                    Price = c.Price,
                    Description = c.Description,
                    YearOfProduction = c.YearOfProduction,
                    IsFeatured = c.IsFeatured,
                    IsRecommended = c.IsRecommended,
                    CarBrand = c.CarBrand.BrandName,
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList(),
                    IsLiked = c.Favorites.Any(f => f.UserId == Guid.Parse(userId) && f.CarId == c.CarId && f.IsRemoved == false)
                });

                var items = cars.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var result = new PaginationResponse<CarDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return result is null ? NotFound(new { Message = "No cars found" }) : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCarById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Unauthorized" });

            if (id == Guid.Empty)
                return BadRequest(new { Message = "Invalid request" });

            var car = await _unitOfWork.CarModels.FindAsync(cm => cm.CarId == id && cm.IsDeleted == false, ["ModelGalleries", "Favorites", "CarBrand"]);

            if (car == null)
                return NotFound(new { Message = "Car not found" });


            var result = _mapper.Map<CarDto>(car);
            result.CarBrand = car.CarBrand.BrandName;
            result.ImagesUrls = car.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList();
            result.IsLiked = car.Favorites.Any(f => f.UserId == Guid.Parse(userId) && f.CarId == car.CarId && f.IsRemoved == false);

            return Ok(result);
        }

        [HttpGet("Featured")]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetFeaturedCarsAsync()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Unauthorized" });

                var featuredCars = await _unitOfWork.CarModels.CustomFindAsync<CarModel, CarDto>(
                predicate: c => c.IsFeatured == true && c.IsDeleted == false,
                selector: c => new CarDto
                {
                    CarId = c.CarId,
                    ModelName = c.ModelName,
                    Price = c.Price,
                    Description = c.Description,
                    YearOfProduction = c.YearOfProduction,
                    IsFeatured = c.IsFeatured,
                    CarBrand = c.CarBrand.BrandName,
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList(),
                    IsLiked = c.Favorites.Any(f => f.UserId == Guid.Parse(userId) && f.CarId == c.CarId && f.IsRemoved == false)
                }
              );
                if (featuredCars is null || featuredCars.Count() < 1)
                {
                    return NotFound(new { Message = "No featured cars found" });
                }

                return featuredCars is null || featuredCars.Count() < 1 ? NotFound(new { Message = "No featured cars found" }) : Ok(featuredCars);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Recommended")]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetRecommendedCarsAsync()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Unauthorized" });

                var RecommendedCars = await _unitOfWork.CarModels.CustomFindAsync<CarModel, CarDto>(
                predicate: c => c.IsRecommended == true && c.IsDeleted == false,
                selector: c => new CarDto
                {
                    CarId = c.CarId,
                    ModelName = c.ModelName,
                    Price = c.Price,
                    Description = c.Description,
                    YearOfProduction = c.YearOfProduction,
                    IsRecommended = c.IsRecommended,
                    CarBrand = c.CarBrand.BrandName,
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList(),
                    IsLiked = c.Favorites.Any(f => f.UserId == Guid.Parse(userId) && f.CarId == c.CarId && f.IsRemoved == false)
                });


                return RecommendedCars is null || RecommendedCars.Count() < 1 ? NotFound(new { Message = "No Recommended cars found" }) : Ok(RecommendedCars);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Brands")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrandsAsync()
        {
            try
            {
                var brands = await _unitOfWork.CarBrands.CustomFindAsync<CarBrand, BrandDto>(
                selector: b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName,
                    Logo = $"{Request.Scheme}://{Request.Host}{b.Logo}"
                }
                );

                if (brands is null || brands.Count() < 1)
                    return NotFound(new { Message = "No brands found" });

                return Ok(brands);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<CarDto>>> SearchCarsAsync(string search)
        {
            try
            {
                var cars = await _unitOfWork.CarModels.CustomFindAsync<CarModel, CarDto>(
                predicate: c => (c.ModelName.Contains(search) && c.IsDeleted == false) || (c.CarBrand.BrandName.Contains(search) && c.CarBrand.IsDeleted == false),
                selector: c => new CarDto
                {
                    CarId = c.CarId,
                    ModelName = c.ModelName,
                    Price = c.Price,
                    Description = c.Description,
                    YearOfProduction = c.YearOfProduction,
                    IsFeatured = c.IsFeatured,
                    IsRecommended = c.IsRecommended,
                    CarBrand = c.CarBrand.BrandName,
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList()
                }
                );

                return cars is null || !cars.Any() ? NotFound(new { Message = "No cars found" }) : Ok(cars);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("Brand")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> CreateBrandAsync([FromForm] AddBrandDto request)
        {
            try
            {
                var brand = new CarBrand
                {
                    BrandName = request.BrandName,
                };


                if (request is null)
                {
                    return BadRequest(new { Message = "Invalid brand request" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.Logo is not null)
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "carbrands");
                    Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Logo.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Logo.CopyToAsync(stream);
                    }

                    brand.Logo = $"/uploads/carbrands/{fileName}";

                    await _unitOfWork.CarBrands.AddAsync(brand);

                }

                return Created($"{Request.Scheme}://{Request.Host}/api/Car/Brand", "Brand added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> CreateCarAsync([FromForm] AddCarDto request)
        {
            try
            {
                if (request is null)
                {
                    return BadRequest(new { Message = "Invalid car request" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var car = _mapper.Map<CarModel>(request);
                car.ModelGalleries = new List<ModelGallery>();

                await _unitOfWork.CarModels.AddAsync(car);

                if (request.Images is not null && request.Images.Any())
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "carmodels");
                    Directory.CreateDirectory(uploadPath);

                    foreach (var image in request.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        car.ModelGalleries.Add(new ModelGallery
                        {
                            CarModelId = car.CarId,
                            ImageUrl = $"/uploads/carmodels/{fileName}"
                        });
                    }

                    await _unitOfWork.SaveChangesAsync();
                }


                return Created($"{Request.Scheme}://{Request.Host}/api/Car", "Car Added successfully");
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("MarkAsFeatured")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> MarkAsFeaturedAsync(Guid carId)
        {
            try
            {
                if (carId == Guid.Empty)
                {
                    return BadRequest(new { Message = "Invalid car id" });
                }

                var car = await _unitOfWork.CarModels.GetByIdAsync(carId);

                if (car is null)
                {
                    return NotFound(new { Message = "Car not found" });
                }

                car.IsFeatured = !car.IsFeatured;

                await _unitOfWork.CarModels.UpdateAsync(car);
                return car.IsFeatured ? Ok(new { Message = "Car marked as featured" }) : Ok(new { Message = "Car marked as not featured" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("MarkAsRecommended")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> MarkAsRecommendedAsync(Guid carId)
        {
            try
            {
                if (carId == Guid.Empty)
                {
                    return BadRequest(new { Message = "Invalid car id" });
                }

                var car = await _unitOfWork.CarModels.GetByIdAsync(carId);

                if (car is null)
                {
                    return NotFound(new { Message = "Car not found" });
                }

                car.IsRecommended = !car.IsRecommended;

                await _unitOfWork.CarModels.UpdateAsync(car);
                return car.IsRecommended ? Ok(new { Message = "Car marked as recommended" }) : Ok(new { Message = "Car marked as not recommended" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("AddImages/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> AddImagesToCarAsync(Guid id, [FromForm] AddImagesToCar request)
        {
            if(id == Guid.Empty || request is null)
                return BadRequest(new { Message = "Invalid request" });

            var car = await _unitOfWork.CarModels.FindAsync(c => c.CarId == id && c.IsDeleted == false, ["ModelGalleries"]);

            if (car is null)
                return NotFound(new { Message = "Car not found" });

            if (request.Images is not null && request.Images.Any())
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "carmodels");
                Directory.CreateDirectory(uploadPath);

                foreach (var image in request.Images)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var gallery = new ModelGallery
                    {
                        CarModelId = car.CarId,
                        ImageUrl = $"/uploads/carmodels/{fileName}"
                    };
                    //car.ModelGalleries.Add(new ModelGallery
                    //{
                    //    CarModelId = car.CarId,
                    //    ImageUrl = $"/uploads/carmodels/{fileName}"
                    //});
                await _unitOfWork.ModelGalleries.AddAsync(gallery);
                }
            }
            return Ok(new { Message = "Images added successfully" });
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> UpdateCarAsync(Guid id, [FromForm] UpdateCarDto request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid car id" });

                if (request is null)
                    return BadRequest(new { Message = "Invalid car request" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var car = await _unitOfWork.CarModels.GetByIdAsync(id);

                if (car is null)
                    return NotFound(new { Message = "Car not found" });

                _mapper.Map<CarModel>(request);

                await _unitOfWork.CarModels.UpdateAsync(car);

                return Ok(new { Message = "Car updated successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> DeleteCarAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid car id" });

                var car = await _unitOfWork.CarModels.FindAsync(c => c.CarId == id, ["ModelGalleries", "Favoirites"]);

                if (car is null)
                    return NotFound(new { Message = "Car not found" });

                if (car.Favorites is not null && car.Favorites.Any())
                    await _unitOfWork.Favorites.DeleteRangeAsync(car.Favorites);

                if (car.ModelGalleries is not null && car.ModelGalleries.Any())
                {
                    foreach (var image in car.ModelGalleries)
                    {
                        var filename = Path.GetFileName(new Uri(image.ImageUrl).LocalPath);
                        var filePath = Path.Combine(_env.WebRootPath, "uploads/carmodels", filename);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    await _unitOfWork.ModelGalleries.DeleteRangeAsync(car.ModelGalleries);
                }

                await _unitOfWork.CarModels.DeleteAsync(car);

                return Ok(new { Message = "Car deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpDelete("RemoveImage/{id}/{imageUrl}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> RemoveImageFromCarAsync(Guid id, string imageUrl)
        {
            try
            {
                if (id == Guid.Empty || string.IsNullOrEmpty(imageUrl))
                    return BadRequest(new { Message = "Invalid request" });

                var car = await _unitOfWork.CarModels.FindAsync(c => c.CarId == id && c.IsDeleted == false, ["ModelGalleries"]);

                if (car is null)
                    return NotFound(new { Message = "Car not found" });

                var image = car.ModelGalleries.FirstOrDefault(c => c.ImageUrl == imageUrl);
                if (image is null)
                    return NotFound(new { Message = "Image not found" });

                var filename = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine(_env.WebRootPath, "uploads/carmodels", filename);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                await _unitOfWork.ModelGalleries.DeleteAsync(image);

                return Ok(new { Message = "Image deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

    }
}

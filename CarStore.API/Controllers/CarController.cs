using AutoMapper;
using CarStore.Applcation.DTOs.Car;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CarStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<CarDto>>> GetCarsAsync()
        {
            try
            {
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
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList()
                }
                );

                return cars is null || cars.Count() < 1 ? NotFound(new { Message = "No cars found" }) : Ok(cars);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("Featured")]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetFeaturedCarsAsync()
        {
            try
            {
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
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList()
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
                    ImagesUrls = c.ModelGalleries.Select(mg => $"{Request.Scheme}://{Request.Host}{mg.ImageUrl}").ToList()
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

        [HttpPost("Brand")]
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
    }
}

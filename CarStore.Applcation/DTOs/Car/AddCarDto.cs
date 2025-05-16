
using Microsoft.AspNetCore.Http;

namespace CarStore.Applcation.DTOs.Car
{
    public class AddCarDto : BasicCarDto
    {
        public List<IFormFile> Images { get; set; }
        public Guid CarBrandId { get; set; }
    }
}

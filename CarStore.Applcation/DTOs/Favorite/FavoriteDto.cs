using CarStore.Applcation.DTOs.Car;

namespace CarStore.Applcation.DTOs.Favorite
{
    public class FavoriteDto : BasicCarDto
    {
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }
        public string CarBrand { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}

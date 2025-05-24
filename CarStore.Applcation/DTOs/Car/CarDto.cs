namespace CarStore.Applcation.DTOs.Car
{
    public class CarDto : BasicCarDto
    {
        public Guid CarId { get; set; }
        public string CarBrand { get; set; }
        public List<string>? ImagesUrls { get; set; }
        public bool IsLiked { get; set; }
    }
}

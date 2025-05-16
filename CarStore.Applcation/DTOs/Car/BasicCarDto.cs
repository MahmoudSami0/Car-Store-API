namespace CarStore.Applcation.DTOs.Car
{
    public class BasicCarDto
    {
        public string ModelName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int YearOfProduction { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsRecommended { get; set; }
    }
}

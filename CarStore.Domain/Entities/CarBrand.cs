namespace CarStore.Domain.Entities
{
    public class CarBrand
    {
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string? Logo { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<CarModel> CarModels { get; set; }
    }
}

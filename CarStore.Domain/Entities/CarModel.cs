namespace CarStore.Domain.Entities
{
    public class CarModel
    {
        public Guid CarId { get; set; }
        public string ModelName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int YearOfProduction { get; set; }
        public Guid CarBrandId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsDeleted { get; set; }

        public CarBrand CarBrand { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<CarFeatures> CarFeatures { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<ModelGallery>? ModelGalleries { get; set; }
    }
}

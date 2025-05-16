namespace CarStore.Domain.Entities
{
    public class Feature
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<CarFeatures>? CarFeatures { get; set; }
    }
}

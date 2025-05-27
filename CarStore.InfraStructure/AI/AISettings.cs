
namespace CarStore.InfraStructure.AI
{
    public class AISettings
    {
        public const string SectionName = "AISettings";
        public string ApiKey { get; set; }
        public string OpenRouterUrl { get; set; }
        public string Model { get; set; }
    }
}

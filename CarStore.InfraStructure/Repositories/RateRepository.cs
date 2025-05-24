using CarStore.Applcation.Interfaces;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Data;
using CarStore.InfraStructure.Repositorries;
using Microsoft.EntityFrameworkCore;

namespace CarStore.InfraStructure.Repositories
{
    public class RateRepository : BaseRepository<Rate>, IRateRepository
    {
        private readonly CarStoreDbContext _context;

        public RateRepository(CarStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<double> GetAverageRate(Guid carId)
        {
            var average = await _context.Rates
            .Where(r => r.CarModelId == carId)
            .AverageAsync(r => (double?)r.RateValue) ?? 0;

            return Math.Round(average, 2);
        }
    }
}

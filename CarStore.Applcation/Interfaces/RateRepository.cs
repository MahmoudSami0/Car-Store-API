
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;

namespace CarStore.Applcation.Interfaces
{
    public interface IRateRepository : IBaseRepository<Rate>
    {
        Task<double> GetAverageRate(Guid carId);
    }
}

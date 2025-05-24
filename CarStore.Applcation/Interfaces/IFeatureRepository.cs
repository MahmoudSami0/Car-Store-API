using CarStore.Applcation.DTOs.Feature;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.Interfaces
{
    public interface IFeatureRepository : IBaseRepository<Feature>
    {
        Task<string> AddFeatureToCar(ToggleFeatureFromCarDto request);
        Task<string> RemoveFeatureFromCar(ToggleFeatureFromCarDto request);
    }
}

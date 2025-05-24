using CarStore.Applcation.DTOs.Feature;
using CarStore.Applcation.Interfaces;
using CarStore.Application.Interfaces;
using CarStore.Domain.Entities;
using CarStore.InfraStructure.Data;
using CarStore.InfraStructure.Repositorries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.InfraStructure.Repositories
{
    public class FeatureRepository : BaseRepository<Feature>, IFeatureRepository
    {
        private readonly CarStoreDbContext _context;

        public FeatureRepository(CarStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> AddFeatureToCar(ToggleFeatureFromCarDto request)
        {
            try
            {
                var car = await _context.CarModels.FindAsync(request.CarId);
                var feature = await GetByIdAsync(request.FeatureId);

                if (car == null ||car.IsDeleted ||feature == null || feature.IsDeleted)
                    return "Car or Feature not found";

                var carFeature = await _context.CarFeatures.FirstOrDefaultAsync(cf => cf.CarModelId == request.CarId && cf.FeatureId == request.FeatureId);

                if (carFeature != null)
                {
                    if(!carFeature.IsDeleted)
                        return "Feature already assigned to this car";

                    carFeature.IsDeleted = false;
                    return "Feature added to car successfully";
                }

                var newCarFeature = new CarFeatures
                {
                    CarModelId = request.CarId,
                    FeatureId = request.FeatureId
                };

                await _context.CarFeatures.AddAsync(newCarFeature);
                await _context.SaveChangesAsync();

                return "Feature added to car successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> RemoveFeatureFromCar(ToggleFeatureFromCarDto request)
        {
            try
            {
                if (request.CarId == Guid.Empty || request.FeatureId == Guid.Empty)
                    return "Invaild request";

                var car = await _context.CarModels.FindAsync(request.CarId);
                var feature = await GetByIdAsync(request.FeatureId);

                if (car == null || car.IsDeleted || feature == null || feature.IsDeleted)
                    return "Car Or Feature not found";

                var carFeature = await _context.CarFeatures.FirstOrDefaultAsync(cf => cf.CarModelId == request.CarId && cf.FeatureId == request.FeatureId);

                if (carFeature == null || carFeature.IsDeleted)
                    return "This feature is not assigned to this car";

                carFeature.IsDeleted = true;
                _context.CarFeatures.Update(carFeature);
                await _context.SaveChangesAsync();
                return "Feature removed from car successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}

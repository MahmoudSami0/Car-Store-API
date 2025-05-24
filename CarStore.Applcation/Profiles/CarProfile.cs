using AutoMapper;
using CarStore.Applcation.DTOs.Car;
using CarStore.Domain.Entities;

namespace CarStore.Applcation.Profiles
{
    public class CarProfile :Profile
    {
        public CarProfile()
        {
            CreateMap<CarModel, CarDto>()
                .ForMember(dest => dest.CarBrand, opt => opt.Ignore())
                .ForMember(dest => dest.ImagesUrls, opt => opt.Ignore())
                .ForMember(dest => dest.IsLiked, opt => opt.Ignore());

            CreateMap<CarBrand, BrandDto>();

            CreateMap<AddCarDto, CarModel>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Rates, opt => opt.Ignore())
                .ForMember(dest => dest.CarFeatures, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.ModelGalleries, opt => opt.Ignore());

            CreateMap<UpdateCarDto, CarModel>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Rates, opt => opt.Ignore())
                .ForMember(dest => dest.CarFeatures, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.ModelGalleries, opt => opt.Ignore());
        }
    }
}
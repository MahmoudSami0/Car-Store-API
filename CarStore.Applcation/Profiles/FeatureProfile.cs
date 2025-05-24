using AutoMapper;
using CarStore.Applcation.DTOs.Feature;
using CarStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.Profiles
{
    public class FeatureProfile : Profile
    {
        public FeatureProfile()
        {
            CreateMap<Feature, FeatureDto>();
            CreateMap<AddFeatureDto, Feature>()
                .ForMember(dest => dest.FeatureId, opt => opt.Ignore())
                .ForMember(dest => dest.CarFeatures, opt => opt.Ignore());
        }
    }
}

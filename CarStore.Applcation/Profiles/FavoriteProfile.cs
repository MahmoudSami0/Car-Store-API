using AutoMapper;
using CarStore.Applcation.DTOs.Favourite;
using CarStore.Domain.Entities;

namespace CarStore.Applcation.Profiles
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            CreateMap<ToggleFavoriteDto, Favorite>();
        }
    }
}

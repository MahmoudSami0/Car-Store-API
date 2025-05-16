using AutoMapper;
using CarStore.Applcation.DTOs.User;
using CarStore.Application.DTOs;
using CarStore.Application.DTOs.User;
using CarStore.Domain.Entities;

namespace CarStore.Applcation.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<BasicUserDto, User>();
            CreateMap<RegisterRequest, PenddingUser>();

            CreateMap<User, AuthResult>()
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthProvider, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleId, opt => opt.Ignore())
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Rates, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

            CreateMap<PenddingUser, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthProvider, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleId, opt => opt.Ignore())
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Rates, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

        }
    }
}
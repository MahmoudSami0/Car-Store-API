
using AutoMapper;
using CarStore.Applcation.DTOs.Role;
using CarStore.Domain.Entities;

namespace CarStore.Applcation.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.UserNames, opt => opt.Ignore());
        }
    }
}

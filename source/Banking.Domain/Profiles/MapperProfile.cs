using AutoMapper;
using Banking.Persistance.Entities;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;

namespace Banking.Domain.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //. source -> destination
            CreateMap<CreateUserDTO, CustomUser>();
            CreateMap<CustomUser, ReadUserDTO>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles.Select(role => role.Role.Name)));
        }

    }
}
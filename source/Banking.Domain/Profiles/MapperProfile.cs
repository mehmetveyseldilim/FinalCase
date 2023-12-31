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
            CreateMap<CustomUser, ReadUserDTO>();
        }

    }
}
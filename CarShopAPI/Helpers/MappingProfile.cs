using AutoMapper;
using Entites;
using CarShopAPI.Dto;

namespace CarShopAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Car, CarDto>();
            CreateMap<User, UserDto>();

        }

    }
}
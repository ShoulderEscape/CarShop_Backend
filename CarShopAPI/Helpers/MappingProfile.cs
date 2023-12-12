using AutoMapper;
using CarShopAPI.Dto;
using Entites;

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
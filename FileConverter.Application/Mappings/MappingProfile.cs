using AutoMapper;
using FileConverter.Application.DTOs.Auth;
using FileConverter.Domain.Entities;

namespace FileConverter.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, UserDto>();
        }
    }
}
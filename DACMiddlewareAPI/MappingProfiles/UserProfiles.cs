using AutoMapper;
using DACMiddlewareAPI.Entities;
using DACMiddlewareAPI.Models;

namespace DACMiddlewareAPI.MappingProfiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        // fromat: CreateMap<source, destination>();
        CreateMap<UserForCreationDto, User>();
        CreateMap<UserForUpdateDto, User>();
        CreateMap<User, UserDto>();
        CreateMap<AttachUserDto, AssignedUser>();
    }
}

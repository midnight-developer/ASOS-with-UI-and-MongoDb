using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASOS.Api.Models;
using AutoMapper;

namespace ASOS.Api
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<User, UserDto>();
      CreateMap<UserDto, User>();
      CreateMap<CreateUserDto, User>();
    }
  }
}

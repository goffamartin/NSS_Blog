using AutoMapper;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;

namespace Blog.ApiService.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}

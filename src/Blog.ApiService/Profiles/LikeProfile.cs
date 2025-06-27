using AutoMapper;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;

namespace Blog.ApiService.Profiles
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<Like, LikeDto>().ReverseMap();
        }
    }
}

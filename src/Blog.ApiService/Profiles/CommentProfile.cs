using AutoMapper;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;

namespace Blog.ApiService.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}

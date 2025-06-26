using AutoMapper;
using Blog.ApiService.Models;
using Blog.Shared.Dtos;

namespace Blog.ApiService.Profiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>().ReverseMap();
        CreateMap<Article, ArticleSearchDto>().ReverseMap();
    }
}
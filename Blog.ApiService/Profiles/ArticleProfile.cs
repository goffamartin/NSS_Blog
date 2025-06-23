using AutoMapper;
using Blog.ApiService.Dtos;
using Blog.ApiService.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Blog.ApiService.Profiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>().ReverseMap();
    }
}
﻿using Blog.Shared.Dtos;

namespace Blog.ApiService.Services
{
    public interface ILikeService
    {
        Task<bool> ToggleLikeAsync(LikeDto dto);
        Task<int> GetLikeCountAsync(int articleId);
    }
}
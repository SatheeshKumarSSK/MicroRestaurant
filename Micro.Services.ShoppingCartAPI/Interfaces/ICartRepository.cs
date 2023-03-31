﻿using Micro.Services.ShoppingCartAPI.DTOs;

namespace Micro.Services.ShoppingCartAPI.Interfaces
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailId);
        Task<bool> ClearCart(string userId);
        Task<bool> ApplyCoupon(string userId, string couponCode);
        Task<bool> RemoveCoupon(string userId);
    }
}

using AutoMapper;
using Micro.Services.ShoppingCartAPI.DTOs;
using Micro.Services.ShoppingCartAPI.Models;

namespace Micro.Services.ShoppingCartAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            CreateMap<CartDetail, CartDetailDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
        }
    }
}

using AutoMapper;
using Micro.Services.ProductAPI.DTOs;
using Micro.Services.ProductAPI.Models;

namespace Micro.Services.ProductAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}

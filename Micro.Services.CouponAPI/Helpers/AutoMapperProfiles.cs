using AutoMapper;
using Micro.Services.CouponAPI.Models;
using Micro.Services.CouponAPI.DTOs;

namespace Micro.Services.CouponAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}

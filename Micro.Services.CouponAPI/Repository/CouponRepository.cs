using AutoMapper;
using Micro.Services.CouponAPI.Data;
using Micro.Services.CouponAPI.DTOs;
using Micro.Services.CouponAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CouponRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(coupon);
        }
    }
}

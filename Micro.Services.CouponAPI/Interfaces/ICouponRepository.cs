using Micro.Services.CouponAPI.DTOs;

namespace Micro.Services.CouponAPI.Interfaces
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}

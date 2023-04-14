using Micro.Services.ShoppingCartAPI.DTOs;

namespace Micro.Services.ShoppingCartAPI.Interfaces
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName, string token);
    }
}

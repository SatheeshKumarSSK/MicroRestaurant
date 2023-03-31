using Micro.Web.Models;

namespace Micro.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<T> GetCoupon<T>(string couponCode, string token);
    }
}

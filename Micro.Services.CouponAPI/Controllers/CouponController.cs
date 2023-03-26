using Micro.Services.CouponAPI.Interfaces;
using Micro.Services.CouponAPI.Repository;
using Micro.Services.ShoppingCartAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        protected ResponseDto _response;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            _response = new ResponseDto();
        }

        [HttpGet("{code}"), Authorize]
        public async Task<object> GetDiscountForCode(string code)
        {
            try
            {
                var coupon = await _couponRepository.GetCouponByCode(code);
                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}

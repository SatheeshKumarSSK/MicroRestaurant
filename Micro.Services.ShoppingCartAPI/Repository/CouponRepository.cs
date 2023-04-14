using Micro.Services.ShoppingCartAPI.DTOs;
using Micro.Services.ShoppingCartAPI.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;

namespace Micro.Services.ShoppingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDto> GetCoupon(string couponName, string token)
        {
            _client.DefaultRequestHeaders.Add("Authorization", token);
            var response = await _client.GetAsync($"/api/couponAPI/{couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}

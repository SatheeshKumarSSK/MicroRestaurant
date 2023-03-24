using System.ComponentModel.DataAnnotations;

namespace Micro.Services.ShoppingCartAPI.DTOs
{
    public class CartHeaderDto
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}

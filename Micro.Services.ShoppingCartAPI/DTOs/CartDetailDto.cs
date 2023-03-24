using System.ComponentModel.DataAnnotations.Schema;

namespace Micro.Services.ShoppingCartAPI.DTOs
{
    public class CartDetailDto
    {
        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        [ForeignKey("CartHeaderId")]
        public virtual CartHeaderDto CartHeader { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}

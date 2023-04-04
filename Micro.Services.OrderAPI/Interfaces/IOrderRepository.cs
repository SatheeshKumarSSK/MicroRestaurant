using Micro.Services.OrderAPI.Models;

namespace Micro.Services.OrderAPI.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paymentStatus);
    }
}

using Micro.Services.OrderAPI.Data;
using Micro.Services.OrderAPI.Interfaces;
using Micro.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<DataContext> _context;

        public OrderRepository(DbContextOptions<DataContext> context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            try
            {
                await using var _db = new DataContext(_context);
                _db.OrderHeaders.Add(orderHeader);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paymentStatus)
        {
            await using var _db = new DataContext(_context);
            var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.OrderHeaderId == orderHeaderId);
            if (orderHeader != null)
            {
                orderHeader.PaymentStatus = paymentStatus;
                await _db.SaveChangesAsync();
            }
        }
    }
}

using Micro.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.OrderAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}

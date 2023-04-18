using Micro.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.EmailAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}

using Micro.Services.EmailAPI.Data;
using Micro.Services.EmailAPI.Interfaces;
using Micro.Services.EmailAPI.Messages;
using Micro.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Services.EmailAPI.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<DataContext> _dbContext;

        public EmailRepository(DbContextOptions<DataContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            //implement an email sender or call some other class library
            EmailLog emailLog = new EmailLog()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully."
            };

            await using var _db = new DataContext(_dbContext);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }
    }
}

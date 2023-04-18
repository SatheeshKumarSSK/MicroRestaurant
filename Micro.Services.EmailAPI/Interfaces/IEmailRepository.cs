using Micro.Services.EmailAPI.Messages;

namespace Micro.Services.EmailAPI.Interfaces
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}

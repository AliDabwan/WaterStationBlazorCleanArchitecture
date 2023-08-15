using System.Threading.Tasks;
using WaterS.Application.Requests.Mail;

namespace WaterS.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
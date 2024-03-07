using IndentityApp.DTOs.Account;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace IndentityApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailSendDto emailSend)
    }
}

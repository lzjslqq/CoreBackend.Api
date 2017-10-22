using Microsoft.Extensions.Logging;

namespace CoreBackend.Api.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = "admin@qq.com";
        private string _mailFrom = "mailsource@126.com";
        private readonly ILogger<CloudMailService> _logger;
        public CloudMailService(ILogger<CloudMailService> logger)
        {
            _logger = logger;
        }
        public void Send(string subject, string msg)
        {
            _logger.LogInformation($"从{_mailFrom}给{_mailTo}通过{nameof(LocalMailService)}发送了邮件");
        }
    }
}

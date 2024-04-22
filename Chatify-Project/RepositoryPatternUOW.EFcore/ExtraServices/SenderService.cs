using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;


namespace RepositoryPattern.EFcore.ExtraServices
{
    public class SenderService(IOptions<SenderSerivceOptions>senderOptions) : ISenderService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            var mimiMessage = new MimeMessage();
            mimiMessage.Subject = subject;
            mimiMessage.From.Add(new MailboxAddress(senderOptions.Value.Name, senderOptions.Value.Email));
            mimiMessage.To.Add(MailboxAddress.Parse(to));
            var bodyBuilder=new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            mimiMessage.Body = bodyBuilder.ToMessageBody();
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(senderOptions.Value.Host, senderOptions.Value.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderOptions.Value.Email, senderOptions.Value.Password);
            await smtp.SendAsync(mimiMessage);

                
        }
    }
}

using System;
using System.IO;
using System.Net;
using animalFinder.Service.Interface;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.SettingsObject;
using Microsoft.Extensions.Options;

namespace animalFinder.Service
{
    public class EmailService : IEmailService
    {
        private readonly IViewRenderService viewRender;
        private readonly MailSettings mailSettings;

        public EmailService(IViewRenderService viewRender, IOptions<MailSettings> mailSettings)
        {
            this.viewRender = viewRender;
            this.mailSettings = mailSettings.Value;
        }

        public async Task Generate(string email, string mailTemplate, object mailData, string subject, Tuple<byte[], string>[] attachments = null)
        {
            var html = viewRender.Render(mailTemplate, mailData);

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(mailSettings.From, mailSettings.Login));
            mimeMessage.To.Add(new MailboxAddress("", email));
            mimeMessage.Subject = subject;

            var multipart = new Multipart("mixed");

            var body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = html
            };
            multipart.Add(body);

            if (!(attachments is null))
            {
                for (var i = 0; i < attachments.Length; i++)
                {
                    var typeAndExt = attachments[i].Item2.Split('/');
                    var file = new MimePart(typeAndExt[0], typeAndExt[1])
                    {
                        Content = new MimeContent(new MemoryStream(attachments[i].Item1), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = $"Attachment{i + 1}"
                    };

                    multipart.Add(file);
                }
            }

            mimeMessage.Body = multipart;

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(mailSettings.ServiceUri, mailSettings.ServicePort, true);
                    await client.AuthenticateAsync(mailSettings.Login, mailSettings.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception)
            {
                throw new ApiException(HttpStatusCode.Conflict, ApiError.MailServerDeclinedEmail);
            }
        }
    }
}

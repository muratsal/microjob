using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Threading.Tasks;
using Google.Apis;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.IO;
using MimeKit;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using NanoGo.Models;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace LMS.Jobs
{
    public class EmailHelper
    {

        public static void SendMail(EmailConfig config,
            Email email,
            List<EmailAttachment> emailAttachments,
            IWebHostEnvironment hostEnvironment)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            var emailFromParts = GetMailAddressFromText(email.EmailFrom);
            var emailToParts = GetMailAddressFromText(email.EmailTo);
            var emailCcParts = GetMailAddressFromText(email.EmailToCc);
            var emailBCCParts = GetMailAddressFromText(email.EmailToBcc);

            MailMessage message = new MailMessage();

            message.From = new System.Net.Mail.MailAddress(emailFromParts[0].EmailAddress, emailFromParts[0].DisplayName);


            emailToParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.To.Add(new System.Net.Mail.MailAddress(x.EmailAddress, x.DisplayName));
            });

            emailCcParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.CC.Add(new System.Net.Mail.MailAddress(x.EmailAddress, x.DisplayName));
            });

            emailBCCParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.Bcc.Add(new System.Net.Mail.MailAddress(x.EmailAddress, x.DisplayName));
            });

            message.Body = email.Body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = email.Subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            if (emailAttachments != null && emailAttachments.Count > 0)
            {
                emailAttachments.ForEach(x =>
                {
                    message.Attachments.Add(new System.Net.Mail.Attachment(hostEnvironment.ContentRootPath + x.FilePath));
                });
            }

            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(config.UserName, config.Password);
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            client.EnableSsl = config.EnableSsl.HasValue ? config.EnableSsl.Value : false;
            client.Port = int.Parse(config.Port);
            client.Host = config.Host;

            client.Send(message);
        }

        private static List<EmailAddressInfo> GetMailAddressFromText(string text)
        {

            List<EmailAddressInfo> emailAddressInfoList = new List<EmailAddressInfo>();

            if (!string.IsNullOrEmpty(text))
            {
                var emailWithName = text.Trim().Split(";").ToList();

                emailWithName.ForEach(x =>
                {
                    var emailParts = x.Split("|").ToList();

                    emailAddressInfoList.Add(new EmailAddressInfo
                    {
                        EmailAddress = emailParts[0],
                        DisplayName = emailParts.Count > 1 ? emailParts[1] : emailParts[0]
                    }); ;

                });
            }

            return emailAddressInfoList;
        }

        private static String GetOAuthCredentialViaP12Key(EmailConfig config, EmailAddressInfo email, IWebHostEnvironment hostEnvironment)
        {
            var path = Path.Combine(hostEnvironment.ContentRootPath, "certificate\\ddlogi-df6f3e57e6fe.p12");


            string serviceAccountEmail = config.UserName;
            var certificate = new X509Certificate2(path, "notasecret", X509KeyStorageFlags.Exportable);

            var scopes = new[] { "https://mail.google.com/" };
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                User = email.EmailAddress,
                Scopes = scopes
            }.FromCertificate(certificate));

            if (credential.RequestAccessTokenAsync(CancellationToken.None).Result == false)
            {

                return null;
            }

            return credential.Token.AccessToken;
        }

        public static async void SendEmailFromGoogleService(EmailConfig config,
            Email email,
            List<EmailAttachment> emailAttachments,
            IWebHostEnvironment hostEnvironment)
        {
            var message = new MimeMessage();
            var emailFromParts = GetMailAddressFromText(email.EmailFrom);
            var emailToParts = GetMailAddressFromText(email.EmailTo);
            var emailCcParts = GetMailAddressFromText(email.EmailToCc);
            var emailBCCParts = GetMailAddressFromText(email.EmailToBcc);
            string token = GetOAuthCredentialViaP12Key(config, emailFromParts[0], hostEnvironment);

            message.From.Add(new MailboxAddress(emailFromParts[0].EmailAddress, emailFromParts[0].DisplayName));


            emailToParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.To.Add(new MailboxAddress(x.EmailAddress, x.DisplayName));
            });

            emailCcParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.Cc.Add(new MailboxAddress(x.EmailAddress, x.DisplayName));
            });

            emailBCCParts.Where(x => !string.IsNullOrEmpty(x.EmailAddress.Trim())).ToList().ForEach(x =>
            {
                message.Bcc.Add(new MailboxAddress(x.EmailAddress, x.DisplayName));
            });

            message.Subject = email.Subject;
            message.Body = new TextPart("html") { Text = email.Body };


            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(config.Host, int.Parse(config.Port), (config.EnableSsl.HasValue ? config.EnableSsl.Value : false));

                var oauth2 = new SaslMechanismOAuth2(emailFromParts[0].EmailAddress, token);
                await client.AuthenticateAsync(oauth2, CancellationToken.None);

                client.Send(message);
                client.Disconnect(true);
            }




        }


        //public static void SendMailWithGSuiteUser()
        //{
        //    try
        //    {
        //        // service account email address
        //        const string serviceAccount = "maiservice@ddlogi.iam.gserviceaccount.com";

        //        // import service account key p12 certificate.
        //        var certificate = new X509Certificate2(@"C:\Users\murat\source\repos\LMS\certificate\ddlogi-21499cbf49e9.p12",
        //            "notasecret", X509KeyStorageFlags.Exportable);

        //        // G Suite user email address
        //        var gsuiteUser = "info@ddlogi.com";

        //        var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(serviceAccount)
        //        {
        //            User = gsuiteUser,
        //            Scopes = new[] { "https://mail.google.com/" }

        //        }.FromCertificate(certificate);

        //        // if service account key is in json format, copy the private key from json file:
        //        // "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n"
        //        // and import it like this:
        //        // string privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEv...revdd\n-----END PRIVATE KEY-----\n";

        //        // var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(serviceAccount)
        //        //{
        //        //    User = gsuiteUser,
        //        //    Scopes = new[] { "https://mail.google.com/" }
        //        // }.FromPrivateKey(privateKey);

        //        // request access token
        //        var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);
        //        if (!credential.RequestAccessTokenAsync(CancellationToken.None).Result)
        //            throw new InvalidOperationException("Access token failed.");

        //        var server = new SmtpServer("smtp.gmail.com 587");
        //        server.ConnectType = SmtpConnectType.ConnectSSLAuto;

        //        server.User = gsuiteUser;
        //        server.Password = credential.Token.AccessToken;

        //        server.AuthType = SmtpAuthType.XOAUTH2;

        //        var mail = new SmtpMail("TryIt");

        //        mail.From = gsuiteUser;
        //        // Please change recipient address to yours for test
        //        mail.To = "muratsalweb@gmail.com";

        //        mail.Subject = "merhaba sana";
        //        mail.TextBody = "this is a test, don't reply";

        //        var smtp = new EASendMail.SmtpClient();
        //        smtp.SendMail(server, mail);

        //        Console.WriteLine("Message delivered!");
        //    }
        //    catch (Exception ep)
        //    {
        //        Console.WriteLine(ep.ToString());
        //    }
        //}

        public class EmailAddressInfo
        {
            public string EmailAddress { get; set; }
            public string DisplayName { get; set; }
        }


        public class EmailMessage
        {
            public string To { get; set; }
            public string From { get; set; }
            public string Subject { get; set; }
            public string MessageText { get; set; }

            public MimeMessage GetMessage()
            {
                var body = MessageText;
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("From a user", From));
                message.To.Add(new MailboxAddress("To a user", To));
                message.Subject = Subject;
                message.Body = new TextPart("plain") { Text = body };
                return message;
            }
        }

    }
}

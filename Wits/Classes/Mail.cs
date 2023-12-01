using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Wits.Properties;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Wits.Classes
{
    public static class Mail
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "resources"))
        .AddJsonFile("appsettings.json")
        .Build();

        private static readonly string FROM_EMAIL = Configuration["EmailSettings:FromEmail"];
        private static readonly string SMTP_HOST = Configuration["EmailSettings:SmtpHost"];
        private static readonly int SMTP_PORT = Configuration.GetSection("EmailSettings")["SmtpPort"] != null
            ? int.Parse(Configuration.GetSection("EmailSettings")["SmtpPort"])
            : 0;

        private static MailMessage mail = new MailMessage();
        private const string DISPLAY_NAME = "Wits And Wagers";
        private static SmtpClient client = new SmtpClient(SMTP_HOST, SMTP_PORT)
        {
            EnableSsl = true
        };
        
        public static string sendConfirmationMail(string to, string username)
        {
            string msge = Properties.Resources.ConfirmationEmailError;
            string htmlFilePath = "resources/CreatedUserEmail.html";
            string body;

            try
            {
                using (StreamReader reader = new StreamReader(htmlFilePath))
                {
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reading HTML file error: " + ex.Message);
                return msge;
            }

            try
            {
                mail.From = new MailAddress(FROM_EMAIL, DISPLAY_NAME);
                mail.To.Add(to);

                mail.Subject = "User Created. Welcome, " + username + "!";
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.Credentials = new NetworkCredential(FROM_EMAIL, Decrypt(Configuration["EmailSettings:EmailPassword"]));

                client.Send(mail);
                msge = Properties.Resources.ConfirmationEmailSent;
            }
            catch (Exception ex)
            {
                msge = ex.Message + Properties.Resources.Failed;
                Console.WriteLine(msge);
            }

            return msge;
        }

        public static string sendInvitationMail(string to, int gameId)
        {
            string msge = Properties.Resources.InvitationEmailError;
            string htmlFilePath = "resources/CreatedUserEmail.html";
            string body;

            try
            {
                using (StreamReader reader = new StreamReader(htmlFilePath))
                {
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reading HTML file error: " + ex.Message);
                return msge;
            }

            try
            {
                mail.From = new MailAddress(FROM_EMAIL, DISPLAY_NAME);
                mail.To.Add(to);

                mail.Subject = "You are invited, use this code: " + gameId + "!";
                mail.Body = body;
                mail.IsBodyHtml = true;

                client.Credentials = new NetworkCredential(FROM_EMAIL, Decrypt(Configuration["EmailSettings:EmailPassword"]));

                client.Send(mail);
                msge = Properties.Resources.InvitationEmailSent;
            }
            catch (Exception ex)
            {
                msge = ex.Message + Properties.Resources.Failed;
                Console.WriteLine(msge);
            }

            return msge;
        }

        static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

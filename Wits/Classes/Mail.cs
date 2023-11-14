using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wits.Classes
{
    public class Mail
    {

        public static string sendConfirmationMail(string to, string username)
        {
            string msge = "An error occurred while sending the confirmation email";
            string from = "witsandwagers12@hotmail.com";
            string displayName = "Wits And Wagers";
            string htmlFilePath = @"C:\Users\dplat\OneDrive\Documentos\Codes n shit\WITS\Wits---Client\Wits\resources\CreatedUserEmail.html";
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
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from, displayName);
                mail.To.Add(to);

                mail.Subject = "User Created. Welcome, " + username + "!";
                mail.Body = body;
                mail.IsBodyHtml = true;


                SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                client.Credentials = new NetworkCredential(from, "xbcvotmftplusccm");
                client.EnableSsl = true;


                client.Send(mail);
                msge = "Confirmation Email sent!";

            }
            catch (Exception ex)
            {
                msge = ex.Message + ". An error occurred...";
                Console.WriteLine(msge);
            }

            return msge;
        }

        public static string sendInvitationMail(string to, int gameId)
        {
            string msge = "An error occurred while sending the invitation email";
            string from = "witsandwagers12@hotmail.com";
            string displayName = "Wits And Wagers";
            string htmlFilePath = @"D:\UV\Tecnologias\Wits\Wits\Wits\resources\CreatedUserEmail.html";
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
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from, displayName);
                mail.To.Add(to);

                mail.Subject = "You are invited, use this code: " + gameId + "!";
                mail.Body = body;
                mail.IsBodyHtml = true;


                SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                client.Credentials = new NetworkCredential(from, "xbcvotmftplusccm");
                client.EnableSsl = true;


                client.Send(mail);
                msge = "Invitation Email sent!";

            }
            catch (Exception ex)
            {
                msge = ex.Message + ". An error occurred...";
                Console.WriteLine(msge);
            }

            return msge;
        }
    }
}

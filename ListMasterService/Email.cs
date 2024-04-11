namespace ListMasterService;

using System.Net;
using System.Net.Mail;

static class Email
{
    public static void Send(string toEmail, string subject, string body)
    {
        try
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("listsmaster@yandex.ru");  
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;  

            SmtpClient smtpClient = new SmtpClient("smtp.yandex.ru");  
            smtpClient.Port = 587; 
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("listsmaster@yandex.ru", "csmnybhnfoycucjl"); 
            smtpClient.EnableSsl = true;

            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending email: " + ex.Message);
        }
    }
}
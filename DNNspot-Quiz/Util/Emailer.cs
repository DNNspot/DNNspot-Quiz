using System.Net;
using System.Net.Mail;
using DotNetNuke.Entities.Host;

namespace DNNspot.Quiz.Util
{
    public class Emailer
    {
        public static void SendMail(MailMessage email)
        {
            var hostSettings = HostSettings.GetHostSettings();

            string server = hostSettings["SMTPServer"].ToString();
            bool smtpSSL = WA.Parser.ToBool(hostSettings["SMTPEnableSSL"]).GetValueOrDefault(false);
            string smtpAuthMode = hostSettings["SMTPAuthentication"].ToString();

            SmtpClient smtp = new SmtpClient(server);
            if (smtpAuthMode == "1")
            {
                smtp.UseDefaultCredentials = false;

                string smtpUser = hostSettings["SMTPUsername"].ToString();
                string smtpPassword = hostSettings["SMTPPassword"].ToString();

                smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            }
            else if (smtpAuthMode == "2")
            {
                smtp.UseDefaultCredentials = true;
            }
            smtp.Send(email);
        }
    }
}
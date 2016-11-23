using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Mail;
using WA.Components;
using MailPriority = DotNetNuke.Services.Mail.MailPriority;

namespace DNNspot.Quiz
{
    public class EmailService
    {
        public static void SendEmail(string from, string to, string cc, string bcc, string subject, string body)
        {
            MailMessage email = new MailMessage();
            if(!string.IsNullOrEmpty(from))
            {
                email.From = new MailAddress(from);
            }
            else
            {
                string hostEmail = HostSettings.GetHostSetting("HostEmail");
                email.From = new MailAddress(hostEmail);
            }
            to.Split(',').ToList().ForEach(t => email.To.Add(t));

            if(!string.IsNullOrEmpty(cc))
            {
                cc.Split(',').ToList().ForEach(x => email.CC.Add(x));
            }
            if (!string.IsNullOrEmpty(bcc))
            {
                bcc.Split(',').ToList().ForEach(x => email.Bcc.Add(x));
            }
            email.Subject = subject;
            email.IsBodyHtml = true;
            email.BodyEncoding = Encoding.UTF8;
            email.Body = body;

            Util.Emailer.SendMail(email);             
        }

        //public static void SendEmailTemplate(string templateName, string emailTo, Dictionary<string,string> templateTokens)
        //{
        //    var template = EmailTemplate.Load(templateName);
        //    if(template != null)
        //    {
        //        TokenProcessor tokenizer = new TokenProcessor("[", "]");
        //        template.ToList.Add(emailTo);
        //        template.Subject = tokenizer.ReplaceTokensInString(template.Subject, templateTokens);
        //        template.Body = tokenizer.ReplaceTokensInString(template.Body, templateTokens);

        //        // Send Email using DNN SMTP Settings NOTE - BROKEN in 5.4.4 !!!!
        //        //DotNetNuke.Services.Mail.Mail.SendMail(
        //        //    template.FromEmail, 
        //        //    template.ToList.ToCsv(),
        //        //    template.CcList.ToCsv(),
        //        //    template.BccList.ToCsv(), 
        //        //    MailPriority.Normal, 
        //        //    template.Subject,
        //        //    MailFormat.Html,
        //        //    Encoding.UTF8, 
        //        //    template.Body,
        //        //    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty
        //        //);

        //        MailMessage email = new MailMessage();
        //        email.From = new MailAddress(template.FromEmail);
        //        template.ToList.ForEach(t => email.To.Add(t));
        //        template.CcList.ForEach(t => email.CC.Add(t));
        //        template.BccList.ForEach(t => email.Bcc.Add(t));
        //        email.Subject = template.Subject;
        //        email.IsBodyHtml = true;
        //        email.BodyEncoding = Encoding.UTF8;
        //        email.Body = template.Body;

                
        //        Util.Emailer.SendMail(email);            
        //    }
        //}
    }
}
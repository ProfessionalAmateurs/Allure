using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace AllureRemodeling.Models
{
    public class Email
    {
        public string sendEmail(string strEmailBody, string strEmailSubject)
        {
            string Message;

            try
            {
                //Configuring webMail class to send emails
                //gmail smtp server
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application
                WebMail.UserName = "capstonefinal2018@gmail.com";
                WebMail.Password = "bluestar2018";

                //Sender email address.
                WebMail.From = "capstonefinal2018@gmail.com";
                string EmailSubject = strEmailSubject;
                string EMailBody = strEmailBody;


                //string OwnerEmailAddress = "justlia86@gmail.com";
                string OwnerEmailAddress = "sani.arab@gmail.com";

                string ContactAddress = "sani.arab@gmail.com";
                //Send email
                WebMail.Send(to: OwnerEmailAddress, subject: EmailSubject, body: EMailBody, cc: ContactAddress, bcc: "", isBodyHtml: true);
                Message = "Email Sent Successfully.";
            }
            catch (Exception)
            {
                Message = "Problem while sending email, Please check details.";

            }
            return Message;

        }
    }
}
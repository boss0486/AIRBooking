using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WebCore.Entities;

namespace Helper.Email
{
    public class EmailModel
    {
        public readonly string From = "mailboss4086@gmail.com";
        public readonly string Password = "123456@@";
        public readonly int Port = 587;
        public readonly string Host = "smtp.gmail.com";
        public readonly string Ident = "Hệ Thống";
    }

    public class MailService
    {

        public static int SendOTP_ForGotPassword(string _to, string _subject, string otpCode)
        {
            string _content = "";
            _content += "<div class='mail-content' style='margin: 0;font-family: Verdana, Geneva, Tahoma, sans-serif, sans-serif; color:#122e39'>";
            _content += "    <div style='border-radius: 0;border: 1px solid #285162;overflow: hidden;text-align: center;max-width: 576px'>";
            _content += "        <div class='header' style='background:#285162;padding: 20px 0; color:#FFF'>" + _subject + "</div>";
            _content += "        <div class='body' style='padding-top: 40px;padding-bottom: 50px;margin-left: 90px;margin-right: 90px;text-align: justify;'>";
            _content += "            <p class='desc' style='margin: 0;font-size: 13px;line-height: 30px;'>";
            _content += "                Quý khách đang thực hiện chức năng quên mật khẩu.";
            _content += "                <br />Mã xác thực: <b style='color: #4868e8'>" + otpCode + "</b>. Xin vui lòng không chia sẻ mã này cho bất kỳ ai.";
            _content += "            </p>";
            _content += "        </div>";
            _content += "        <div class='footer'><hr style='border: none; border-bottom: 1px solid #ccc;' />";
            _content += "            <p class='desc' style='padding-left: 25px;padding-right: 25px;font-size: 12px;margin: 0;padding: 20px 25px; color:#636768'>Hỗ trợ xin vui lòng liên hệ: support@hrm.com</p>";
            _content += "        </div>";
            _content += "    </div>";
            _content += "</div>";
            MailService mailService = new MailService();
            int status = mailService.MailExcute(_to, _subject, _content);
            return status;
        }

        public static int SendBooking_TicketingInfo(string _to, string siteId, string inUrlPage)
        {
            // contact
            if (!string.IsNullOrWhiteSpace(siteId))
            {
                WebCore.Services.AirAgentService customerService = new WebCore.Services.AirAgentService();
                AirAgent customer = customerService.GetAlls(m => m.ID == siteId).FirstOrDefault();
                if (customer == null)
                    return -1;
                // 
                string title = customer.Title;
                string address = customer.Address;
                string phone = customer.CompanyPhone;
                string email = customer.ContactEmail;
                string _subject = "Thông tin đặt vé tại phòng vé " + title;
                string _content = "";
                _content += "<div style='background-color:#ff017e;color:#fff;padding:15px 20px'>";
                _content += "   <div style='font-size:24px;color:#ff0'>";
                _content += "    <div style='font-size:24px;color:#ff0'>" + title + "</div>";
                _content += "   </div>";
                _content += "</div>";
                _content += "<div style='background-color:#eee;padding:10px 20px 5px 20px;line-height:150%'>";
                _content += "    <strong>Kính chào Quý khách! </strong>";
                _content += "    <p>";
                _content += "        - Xin cám ơn quý khách hàng, đã sử dụng dịch vụ đặt vé máy bay tại focus travel. <br />- Đơn hàng của quý khách đang ở trạng thái chờ thanh toán.";
                _content += "    </p> ";
                _content += "</div>";

                string attmPath = Helper.File.AttachmentFile.AttachmentPDF(Helper.Page.Library.FormatToUni2NONE(_subject), inUrlPage, "/Files/Export/Order/");
                if (string.IsNullOrWhiteSpace(attmPath))
                    return 0;
                //
                if (customer != null)
                {
                    title = customer.Title;
                    address = customer.Address;
                    phone = customer.ContactPhone;
                    email = customer.ContactEmail;
                }

                _content += "<div style='width:100%;background-color:#ff017e;padding:15px 20px;color:#fff'>";
                _content += "    <p style='margin:0px'>";
                _content += "        <a style='font-weight:bold;text-decoration:none;color:#fff'>";
                _content += "            <strong style='text-transform:uppercase;font-size:14px'>Phòng vé: " + title + "  </strong></a>";
                _content += "    </p>";
                _content += "    <p style='margin:0px'>- ĐT: " + phone + "<br />- Email: " + email + "<br />- ĐC: " + address + ". <br /> </p><br>";
                _content += "</div>";
                MailService mailService = new MailService();

                int mailStaus = mailService.MailExcute(_to, _subject, _content, attmPath);
                return mailStaus;
            }
            return 0;
        }
        // Mail Activated
        public static int MailRegAccount(string _to, string _user)
        {
            string _subject = "Register Account";
            string _content = "";
            MailService mailService = new MailService();
            int status = mailService.MailExcute(_to, _subject, _content);
            return status;
        }
        public static int MailForgotPassword(string _to, string _name)
        {
            string _subject = "Your password has been changed";
            string _content = "";
            MailService mailService = new MailService();
            int status = mailService.MailExcute(_to, _subject, _content);
            return status;
        }
        public static int MailFeedback(string _to, string _name, string body)
        {
            string _subject = "Customer feedback";
            string _content = "";
            MailService mailService = new MailService();
            int status = mailService.MailExcute(_to, _subject, _content);
            return status;
        }

        public int MailExcute(string _to, string _subject, string _content, string attmPath = null, string siteId = null)
        {
            if (!Helper.Page.Validate.TestEmail(_to))
                return 0;
            EmailModel mailModel = new EmailModel();
            //Task t = Task.Run(async () =>
            //{
            //    // You should use using block so .NET can clean up resources

            //});

            Task t = Task.Run(async () =>
            {
                // You should use using block so .NET can clean up resources
                using (var client = new SmtpClient())
                {
                    using (MailMessage msg = new MailMessage())
                    {
                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attmPath);
                        msg.Attachments.Add(attachment);
                        //
                        msg.From = new MailAddress(mailModel.From, mailModel.Ident);
                        msg.To.Add(_to);
                        msg.Subject = _subject;
                        msg.Body = _content;
                        msg.IsBodyHtml = true;
                        //
                        client.UseDefaultCredentials = true;
                        client.Host = mailModel.Host;
                        client.Port = mailModel.Port;
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Credentials = new NetworkCredential(mailModel.From, mailModel.Password);
                        client.Timeout = 20000;
                        await client.SendMailAsync(msg);
                    }
                }
            });
            t.Wait();
            return 1;

            //using (var client = new SmtpClient())
            //{
            //    using (MailMessage msg = new MailMessage())
            //    {
            //        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attmPath);
            //        msg.Attachments.Add(attachment);
            //        //
            //        msg.From = new MailAddress(mailModel.From, mailModel.Ident);
            //        msg.To.Add(_to);
            //        msg.Subject = _subject;
            //        msg.Body = _content;
            //        msg.IsBodyHtml = true;

            //        client.UseDefaultCredentials = true;
            //        client.Host = mailModel.Host;
            //        client.Port = mailModel.Port;
            //        client.EnableSsl = true;
            //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        client.Credentials = new NetworkCredential(mailModel.From, mailModel.Password);
            //        client.Timeout = 20000;
            //        await client.SendMailAsync(msg);

            //        return 1;
            //    }
            //}
        }



        public static string MailExcuteTest(string _to, string _subject, string _content, string attmPath)
        {
            try
            {
                EmailModel mailModel = new EmailModel();
                MailMessage msg = new MailMessage();

                if (!Helper.Page.Validate.TestEmail(_to))
                    return "0";
                // 
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attmPath);
                msg.Attachments.Add(attachment);
                //
                msg.From = new MailAddress(mailModel.From, mailModel.Ident);
                msg.To.Add(_to);
                msg.Subject = _subject;
                msg.Body = _content;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    UseDefaultCredentials = true,
                    Host = mailModel.Host,
                    Port = mailModel.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(mailModel.From, mailModel.Password),
                    Timeout = 20000
                };
                smtp.Send(msg);
                return "1";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

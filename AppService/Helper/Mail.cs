using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{

    public class MailModel
    {
        public readonly string From = "mailboss4086@gmail.com";
        public readonly string Password = "123456@@";
        public readonly int Port = 587;
        public readonly string Host = "smtp.gmail.com";
        public readonly string Ident = "Hệ Thống";
    }

    public class Mail
    {


        public static string TestMail(string _to)
        {
            string _content = "Test mail";
            string _subject = "Mail suppost";
            string status = MailExcuteTest(_to, _subject, _content);
            return status;
        }
        public static int SendOTP_ForGotPassword(string _to, string _subject, string otpCode)
        {
            try
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
                int status = MailExcute(_to, _subject, _content);
                return status;

            }
            catch (Exception)
            {
                return 0;
            }
        }
        // Mail Activated
        public static int MailRegAccount(string _to, string _user)
        {
            string _subject = "Register Account";
            string _content = "";
            int status = MailExcute(_to, _subject, _content);
            return status;
        }
        public static int MailForgotPassword(string _to, string _name)
        {
            string _subject = "Your password has been changed";
            string _content = "";
            int status = MailExcute(_to, _subject, _content);
            return status;
        }
        public static int MailFeedback(string _to, string _name, string body)
        {
            string _subject = "Customer feedback";
            string _content = "";
            int status = MailExcute(_to, _subject, _content);
            return status;
        }
        public static int MailExcute(string _to, string _subject, string _content)
        {
            //try
            //{
                MailMessage msg = new MailMessage();
                MailModel mailModel = new MailModel();
                if (!Helper.Validate.FormatEmail(_to))
                    return 0;

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
                return 1;
            //}
            //catch
            //{
            //    return -1;
            //}
        }
        public static string MailExcuteTest(string _to, string _subject, string _content)
        {
            try
            {
                MailModel mailModel = new MailModel();
                MailMessage msg = new MailMessage();

                if (!Helper.Validate.FormatEmail(_to))
                    return "0";

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

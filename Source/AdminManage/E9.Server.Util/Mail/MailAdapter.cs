using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration; 

namespace NM.Mail
{
    public class MailAdapter
    {

        private MailServerInfo _ServerInfo;

        public MailAdapter(int port=25)
        {
            _ServerInfo = new MailServerInfo(port);
        }

        public MailAdapter(MailServerInfo serverInfo)
        {
            _ServerInfo = serverInfo;
        }

        ~MailAdapter()
        {

        }

        public static MailAdapter LoadFromSetting()
        {
            //MailServerInfo serverInfo = new MailServerInfo(MailSetting.Default.SmtpServer, MailSetting.Default.SmtpPort);
            //serverInfo.Username = MailSetting.Default.UserName;
            //serverInfo.Password = MailSetting.Default.Password;

            MailServerInfo serverInfo = MailSetting.Default.SmtpServerConn;
            //FileLogManager.Default.InforIt(serverInfo.ToString());
            return new MailAdapter(serverInfo);
        }

        private bool IsMail(string mailAddress)
        {
            bool FoundMatch = false;
            try
            {
                FoundMatch = Regex.IsMatch(mailAddress.ToUpper(), "\\A[A-Z0-9._%-]+@[A-Z0-9._%-]+\\.[A-Z]{2,4}\\z");
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
            }
            return FoundMatch;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="context">邮件内容</param>
        /// <param name="absoluteFilePaths">附件的绝对路径，多个用";"号隔开</param>
        /// <param name="recMails">邮件接收地址，多个用";"号隔开</param>
        public void SendMail(string title,string context,string recMails,string absoluteFilePaths=null)
        {
            MailInfo mail = new MailInfo();
            mail.Notify_content = context;
            mail.Notify_title = title;
            mail.From_email = ConfigurationManager.AppSettings["MailFromName"];
            if (!string.IsNullOrEmpty(absoluteFilePaths))
            {
                mail.Attach_file_path = absoluteFilePaths;
            }

            if (!string.IsNullOrEmpty(recMails))
            {
                SendMail(mail, recMails);
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendingMail">已经初始化的Mail</param>
        /// <param name="recMails">邮件接收地址，多个用";"号隔开</param>
        public void SendMail(MailInfo sendingMail, string recMails)
        {
            if (string.IsNullOrEmpty(recMails))
            {
                sendingMail.ReturnMessage = "No Mail Rec Addrs";
                return;
            }
            //FileLogManager.Default.DebugIt(string.Format("Before send mail id={0},to={1}", sendingMail.Event_notify_id, sendingMail.Email));
            MailMessage message = new MailMessage();

            if (!IsMail(sendingMail.From_email))
            {
                if (!string.IsNullOrEmpty(MailSetting.Default.DefaultSenderMailAddress) && IsMail(MailSetting.Default.DefaultSenderMailAddress))
                {
                    message.From = new MailAddress(MailSetting.Default.DefaultSenderMailAddress);
                }
                else
                {
                    sendingMail.ReturnMessage = "Please set the 'DefaultSenderEMailAddress' value in the configuration";
                    sendingMail.Status = MailStatus.DefaultSenderAddressInvalid;
                    return;
                }
            }
            else
            {
                message.From = new MailAddress(sendingMail.From_email);
            }

            string[] rm = recMails.Split(';');
            foreach (var r in rm)
            {
                if (IsMail(r))
                {
                    message.To.Add(r);
                }
            }
            
            message.Subject = sendingMail.Notify_title;
            message.Body = sendingMail.Notify_content;

            //添加附件
            //message.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath("~/images/butt.gif")));

            message.IsBodyHtml = sendingMail.Notify_content.StartsWith("<body>", StringComparison.CurrentCultureIgnoreCase) && sendingMail.Notify_content.EndsWith("</body>", StringComparison.CurrentCultureIgnoreCase);

            //FileLogManager.Default.DebugIt(sendingMail.Notify_content);
            //FileLogManager.Default.DebugIt(string.Format("mail is html: {0}", message.IsBodyHtml));

            #region Add file attach to message 添加附件

            if (!string.IsNullOrEmpty(sendingMail.Attach_file_path))
            {
                try
                {
                    foreach (string path in sendingMail.Attachments)
                        message.Attachments.Add(new Attachment(path));
                }
                catch (Exception ex)
                {
                    sendingMail.ReturnMessage = "Attachment error :" + ex.Message;
                    //FileLogManager.Default.DebugIt(ex.ToString());
                    sendingMail.Status = MailStatus.AttachmentError;
                    return;
                }
            }

            #endregion

            SmtpClient smtp = new SmtpClient(_ServerInfo.ServerName, _ServerInfo.Port);
            smtp.EnableSsl = _ServerInfo.NeedSSL;

            if (smtp.Port != 25)
            {
                smtp.EnableSsl = true;
            }

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp.DeliveryMethod = _ServerInfo.DeliveryMethod;
            if (smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                smtp.PickupDirectoryLocation = _ServerInfo.DeliveryLoacl;

            if (smtp.DeliveryMethod == SmtpDeliveryMethod.PickupDirectoryFromIis)
            {
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = CredentialCache.DefaultNetworkCredentials;
            }

            if (!string.IsNullOrEmpty(_ServerInfo.Username))
            {
                System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential();
                networkCredential.UserName = _ServerInfo.Username;
                networkCredential.Password = _ServerInfo.Password;
                smtp.Credentials = networkCredential;
            }
            try
            {
                //smtp.Send(message);

                #region 异步发送, 会进入回调函数SendCompletedCallback，来判断发送是否成功
                smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
               // string userState = "测试";
                smtp.SendAsync(message, message.Subject);
                #endregion


                sendingMail.ReturnMessage = " Mail was send to server successfully";
            }

            catch (Exception ex)
            {
                sendingMail.ReturnMessage = "Send mail error " + ex.ToString();
                //FileLogManager.Default.ErrorIt("Send mail error " + ex.ToString());
                sendingMail.Status = MailStatus.UnKnowError;
            }
            sendingMail.Status = MailStatus.SendSuccess;
        }

        void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)  //邮件发送被取消
            {

            }
            if (e.Error != null)   //邮件发送失败
            {

            }
            else   //发送成功
            {

            }

        } 
    }
}

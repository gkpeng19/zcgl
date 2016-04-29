using System.Net.Mail;
using System.Text;
using System.Configuration;
using NM.Lib;

namespace NM.Mail
{
    public class MailServerInfo
    {
        /// <summary>
        /// 使用默认Web.config文件的邮件发送配置
        /// </summary>
        /// <param name="port"></param>
        public MailServerInfo(int port=25)
        {
            ServerName = ConfigurationManager.AppSettings["MailServerName"];
            Username = ConfigurationManager.AppSettings["MailUserName"];
            Password = newTemp.DecryptString(ConfigurationManager.AppSettings["MailPsw"]);
            Port = port;
        }

        /// <summary>
        /// 自定义邮件发送参数
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public MailServerInfo(string serverName, int port, string user, string password)
        {
            ServerName = serverName;
            Port = port;
            Username = user;
            Password = password;
        }


        #region  Connet the server


        private string _ServerName;
        private int _Port = 25;

        public string ServerName
        {
            get { return _ServerName; }
            set { _ServerName = value; }
        }

        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private string _Username;

        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private bool _NeedSSL = false;

        public bool NeedSSL
        {
            get { return _NeedSSL; }
            set { _NeedSSL = value; }
        }

        private SmtpDeliveryMethod _DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

        public SmtpDeliveryMethod DeliveryMethod
        {
            get { return _DeliveryMethod; }
            set { _DeliveryMethod = value; }
        }

        private string _DeliveryLoacl;

        public string DeliveryLoacl
        {
            get { return _DeliveryLoacl; }
            set { _DeliveryLoacl = value; }
        }

        public string Display()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ServerName))
            {
                sb.Append("ServerName :");
                sb.AppendLine(ServerName);
            }
            sb.Append("  Port:");
            sb.AppendLine(Port.ToString());
            sb.Append("SmtpDeliveryMethod :");
            sb.AppendLine(DeliveryMethod.ToString());
            if (!string.IsNullOrEmpty(Username))
            {
                sb.Append("UserName :");
                sb.AppendLine(Username);
            }

            if (!string.IsNullOrEmpty(Password))
            {
                sb.Append("Password :");
                sb.AppendLine(Password);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return string.Format("ServerName: {0},Port: {1}", ServerName, Port);
        }
        #endregion
    }
}

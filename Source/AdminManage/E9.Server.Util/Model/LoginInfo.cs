
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NM.Log;
using NM.Util;

namespace NM.Model
{
    public enum LoginStatus
    {
        Failed = 0,
        Successed = 1,
        Exited = 2
    }

    public class LoginInfo : TJson
    {
        public LoginInfo()
        {
            LoginTime = DateTime.Now;
            LogoutTime = DateTime.Now;
            Permission = new List<string>();
            ClientIP = "";
            ClientName = "";           
        }

        public int ID { get; set; }
        public EAP_User User { get; set; }
        public List<string> Permission { get; set; }
        public DateTime LoginTime { get; set; }
        public string ClientIP { get; set; }
        public string ClientName { get; set; }
        public string ServerIP { get; set; }
        public string ServerName { get; set; }
        public LogLevel LogLevel { get; set; }
        public int LoginPort { get; set; }
         
        public DateTime LogoutTime { get; set; }
        public int SessionID { get; set; }
        public bool IsDebug
        {
            get
            {
                return LogLevel >= LogLevel.Debug;
            }
            set
            {
                LogLevel = value ? LogLevel.Debug : LogLevel.Hint;
            }
        }

        public string Message { get; set; }
        public LoginStatus Status { get; set; }



        public static LoginInfo SystemUser()
        {
            return new LoginInfo() { User = new EAP_User() {UserID=1,UserName="Admin" } };
        }

        //#if SILVERLIGHT
        //        [IgnoreDataMemberAttribute]
        //#endif
    }
}

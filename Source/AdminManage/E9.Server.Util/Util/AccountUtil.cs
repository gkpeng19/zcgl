using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.OP;
using NM.Model;
using System.Web;
using System.Web.Security;

namespace NM.Util
{
    public static class AccountUtil
    {
        public static LoginInfo Login { get; private set; }

        public static LoginInfo LogIn(string sUserName, string sPassword, string clientIP, string clientName, int port)
        {
            AccountOP op = new AccountOP(null);
            Login = op.Login(sUserName, sPassword, clientIP, clientName, port);

            return Login;
        }
        public static LoginInfo LoginBykey(string sUserName, string sPassword, string clientIP, string clientName, int port,string KeyID)
        {
            AccountOP op = new AccountOP(null);
            Login = op.LoginBykey(KeyID);

            return Login;
        }
        public static void LogOut(LoginInfo login)
        {
            if (login == null)
                return;

            AccountOP op = new AccountOP(null);
            op.Logout(login);

            FormsAuthentication.SignOut();

            HttpCookie cookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                cookie.Path = HttpContext.Current.Request.ApplicationPath;
                cookie.Value = "";
            }
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.End();
        }

        public static void LogOut()
        {
            LogOut(Login);
        }
    }
}

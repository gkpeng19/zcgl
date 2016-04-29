using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.Security;
using System.Web;
using NM.Model;

namespace NM.Util
{
    public class E9BaseLoginPage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            if (Request.QueryString["RETURNURL"] == null)
            {
                Response.Redirect("Login.aspx?ReturnURL=" + DefaultRedirectPage, true);
            }
            base.OnLoad(e);
        }

        protected virtual void Authenticate(string sUserName, string sPassword, bool bSignAuto)
        {
            try
            {
                string clientIP = HttpContext.Current.Request.UserHostAddress;
                //string clientName = string.Format("{0},{1},{2}",
                //     HttpContext.Current.Request.ServerVariables["REMOTE_HOST"],
                //     HttpContext.Current.Request.ServerVariables["LOGON_USER"],
                //     HttpContext.Current.Request.ServerVariables["SERVER_NAME"]
                //     );// HttpContext.Current.Request.UserHostName;
                string clientName = HttpContext.Current.Request.ServerVariables["LOGON_USER"];

                int loginPort = HttpContext.Current.Request.Url.Port;

                LoginIn(sUserName, sPassword, clientIP, clientName, loginPort);

                SetPersistentCookie(sUserName, bSignAuto);
                FormsAuthentication.RedirectFromLoginPage(sUserName, false);
                //Response.Redirect(FormsAuthentication.GetRedirectUrl(sUserName, false));
            }
            catch (ApplicationException ex)
            {
                HandleLoginError(ex);
            }
        }

        protected virtual void LoginIn(string sUserName, string sPassword, string clientIP, string clientName, int port)
        {
            LoginInfo login = AccountUtil.LogIn(sUserName, sPassword, clientIP, clientName, port);

            if (login.Status == LoginStatus.Failed)
            {
                throw new ApplicationException(login.Message);
            }
            else
            {
                //AccountPrincipal principal = new AccountPrincipal(login.User);
                //Context.User = principal;
            }
        }

        protected virtual void HandleLoginError(Exception ex)
        {
            throw ex;
        }

        public void LogOut()
        {
            AccountUtil.LogOut(AccountUtil.Login);
        }

        protected void SetPersistentCookie(string sUser, bool persistent)
        {
            FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, sUser,
                                                                        DateTime.Now, DateTime.Now.AddMonths(3),
                                                                        persistent, sUser,
                                                                        Request.ApplicationPath.ToLower());

            string encTicket = FormsAuthentication.Encrypt(t);

            HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName,
                                          encTicket);

            if (persistent)
                c.Expires = DateTime.Now.AddMonths(3);
            c.Path = Request.ApplicationPath;

            Response.Cookies.Add(c);
        }

        public virtual string DefaultRedirectPage
        {
            get { return WebUtil.GetAbsUrl(""); }
        }

        private string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

    }
}

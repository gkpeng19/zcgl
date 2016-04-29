using NM.Model;
using NM.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GIS.Portal
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string UserName = this.username.Value;
                string Password = this.password.Value;
                string clientip = getIPAddress();
                LoginInfo login = AccountUtil.LogIn(UserName, Password, null, "GisPortal", 0);

                if (login.Status == NM.Model.LoginStatus.Successed)
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(UserName, false);

                    Session["loginuser"] = login;
                    Session.Add("USERNAME", UserName);
                    string mm = Request.QueryString["ReturnUrl"];
                    if (string.IsNullOrEmpty(mm))
                    {
                        mm = "/Default.aspx";
                    }
                    Response.Redirect(mm);
                }
                else 
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script type='text/javascript'>alert('用户名或密码错误，请重试！');</script>");
                }
            }
        }

        public static string getIPAddress()
        {
            string result = String.Empty;
            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            // 如果使用代理，获取真实IP 
            if (result != null && result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
            {
                result = null;
            }
            else if (result != null)
            {
                if (result.IndexOf(",") != -1)
                {
                    //有“,”，估计多个代理。取第一个不是内网的IP。 
                    result = result.Replace(" ", "").Replace("'", "");
                    string[] temparyip = result.Split(",;".ToCharArray());
                    for (int i = 0; i < temparyip.Length; i++)
                    {
                        if (IsIPAddress(temparyip[i])
                            && temparyip[i].Substring(0, 3) != "10."
                            && temparyip[i].Substring(0, 7) != "192.168"
                            && temparyip[i].Substring(0, 7) != "172.16.")
                        {
                            return temparyip[i];    //找到不是内网的地址 
                        }
                    }
                }
                else if (IsIPAddress(result)) //代理即是IP格式 
                {
                    return result;
                }
                else
                {
                    result = null;    //代理中的内容 非IP，取IP 
                }
            }
            if (null == result || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (result == null || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.UserHostAddress;
            }

            return result;
        }

        /// <summary>

        /// 判断是否是IP地址格式 0.0.0.0

        /// </summary>

        /// <param name="str1">待判断的IP地址</param>

        /// <returns>true or false</returns>

        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15)
            {
                return false;
            }

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
    }
}
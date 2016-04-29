using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;
using App.Models.Sys;
using System.Web.Configuration;
using App.Common;
namespace App.Admin
{
    public static class LoginUserManage
    {
        /// <summary>
        ///  验证用户状态是否改变
        /// </summary>
        /// <param name="sessId">sessionid</param>
        /// <param name="name">session</param>
        /// <returns></returns>
        public static bool IsChange(String sessId, String name)
        {
            Boolean bResult = false;
            NameValueCollection loginUsers = HttpContext.Current.Application["__loginUsers"] as NameValueCollection;
            if (loginUsers != null)
            {
                String oldSessId = loginUsers.GetValues(name)[0];
                if (!String.IsNullOrEmpty(oldSessId) && !sessId.Equals(oldSessId))
                {
                    bResult = true;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 验证是否有单机限制
        /// </summary>
        /// <param name="account">用户信息</param>
        /// <returns>验证结果：true有限制，false没有</returns>
        public static bool ValidateRelogin(AccountModel account)
        {
            bool bResult = false;
            if (account != null)
            {
                App.Models.Sys.siteconfig siteConfig = new App.BLL.SysConfigBLL().loadConfig(Utils.GetXmlMapPath("Configpath"));
                if (siteConfig.issinglelogin == 1)
                {
                    if (IsChange(HttpContext.Current.Session.SessionID, account.Id))
                    {
                        //同一帐号已经在其他机子登录
                        bResult = true;
                        WritePage("同一帐号已经在其他机子登录！需要重新登录", "<a href='/Account'>Login</a>");
                    }
                }
            }
            return bResult;
        }
        /// <summary>
        /// 验证是否已经登录
        /// </summary>
        /// <param name="account">account</param>
        /// <returns>验证结果：true已经登录，false没有登录</returns>
        public static bool ValidateIsLogined(AccountModel account)
        {

            if (account == null)
            {
                WritePage("您没有登录，或已经跳时！", "/Account");
            }

            return true;
        }
        /// <summary>
        /// 失效（错误）窗口提示
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="url">URL地址，可选参数，为空则只弹出对话框，而不刷新页面</param>
        public static void WritePage(string message, string url)
        {
            if (url == "")
            {
                HttpContext.Current.Response.Write(message);
                HttpContext.Current.Response.End();
            }
            else
            {
                HttpContext.Current.Response.Write(message + "去登录:<a href='" + url + "'>Login</a>，后再刷新窗口");
                HttpContext.Current.Response.End();
            }
        }
    }
}
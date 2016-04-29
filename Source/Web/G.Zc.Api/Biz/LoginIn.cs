using G.Zc.Entity.Eap;
using G.Util.Tool;
using GOMFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace G.Zc.Api.Biz
{
    public class LoginIn
    {
        public static CommonResult Login(string username, string password, string clientIp = null, string clientName = null, int nametype = 0, int port = 0)
        {
            CommonResult result = new CommonResult() { ResultID = 0 };
            if (string.IsNullOrEmpty(username))
            {
                result.Message = "登录失败，用户名输入为空。";
            }
            if (string.IsNullOrEmpty(password))
            {
                result.Message = "登录失败，密码输入为空。";
            }

            EapOracleListProcEntity pe = new EapOracleListProcEntity("usp_GetUserByUserName2");
            pe["UserName"] = username;
            pe["nametype"] = nametype;
            EAP_User user = null;
            var users = pe.Execute<EAP_User>();
            if (users.Data.Count == 0)
            {
                result.Message = "登录失败，用户不存在。";
            }
            else
            {
                user = users.Data[0];
                if (nametype == 0 && password != DESEncrypt.Decrypt(user.Password))
                {
                    result.Message = "登录失败，密码不正确。";
                }
                else if (user.IsLock)
                {
                    result.Message = "该用户名已经已经停止使用。";
                }
                else
                {
                    result.ResultID = 1;
                    result.Tag = user;
                }
            }

            #region 记录登录日志

            #region get client pc info

            string serverIP, serverName;
            DateTime myNow = DateTime.Now;
            serverName = Dns.GetHostEntry("localhost").HostName;

            System.Net.IPAddress[] addressList = Dns.GetHostEntry(serverName).AddressList;
            if (addressList.Length > 0)
            {
                int _k = addressList.Length - 1;
                serverIP = addressList[_k].ToString();
            }

            else
                serverIP = addressList[0].ToString();
            serverIP = "1";

            #endregion

            APP_Login info = new APP_Login();
            info["USERNAME"] = username;
            info["SERVERIP"] = serverIP;
            info["SERVERNAME"] = string.IsNullOrEmpty(serverName) ? "No get." : serverName;
            info["CLIENTIP"] = clientIp;
            info["CLIENTNAME"] = string.IsNullOrEmpty(clientName) ? "No get." : clientName;
            info["LOGINPORT"] = port;
            info["LOGINTIME"] = DateTime.Now;
            info["LOGMESSAGE"] = result.Message;
            info["STATUS"] = result.ResultID;

            info.Save();

            #endregion

            return result;
        }
    }
}
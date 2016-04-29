using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using NM.Model;
using System.Runtime.Serialization;
#if SILVERLIGHT
#else
using System.Web;
using System.Threading;
using System.Configuration;
using NM.Lib;
using NM.OP;
using System.Data.SqlClient;
#endif

namespace NM.Util
{
    public interface IMsgSendArgs
    {
        [IgnoreDataMember]
        Int32 IsUsing_G { get; }

        [IgnoreDataMember]
        Int32 IsSend_Rec_G { get; }

        [IgnoreDataMember]
        Int32 IsSend_Sell_G { get; }

        [IgnoreDataMember]
        Int32 IsSend_DaySum_G { get; }

        [IgnoreDataMember]
        String ReceiveTels_G { get; }

        [IgnoreDataMember]
        String BillCode_G { get; set; }

        [IgnoreDataMember]
        Decimal BillAmount_G { get; set; }

        /// <summary>
        /// 收货金额-采购退货金额
        /// </summary>
        [IgnoreDataMember]
        Decimal RecAmount_G { get; set; }

        /// <summary>
        /// 销售额-销售退货金额
        /// </summary>
        [IgnoreDataMember]
        Decimal SellAmount_G { get; set; }
    }

#if SILVERLIGHT
#else
    public interface ISendMsg
    {
        event Action<IMsgSendArgs, MsgSendType> SendActioned;
    }

    public enum MsgSendType
    {
        None,
        Rec,
        Sell,
        DaySum
    }
    public class SendUserInfo
    {

        public int orgid { get; set; }
        public string userid { get; set; }
        public string username { get; set; }
        public int isLog { get ; set;   }

        public SendUserInfo()
        {
            isLog = 1;
        }

    }

    public class MsgSend
    {
        static bool IsUsingMsg
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["IsUsingMsg"]);
                }
                catch { return false; }
            }
        }
        static string strReg = newTemp.DecryptString(ConfigurationManager.AppSettings["MsgReg"]);
        static string strPwd = newTemp.DecryptString(ConfigurationManager.AppSettings["MsgPwd"]);
        static string strSourceAdd = ConfigurationManager.AppSettings["MsgSourceAdd"];
        static string url = ConfigurationManager.AppSettings["MsgUrl"];

        static string balanceUrl = ConfigurationManager.AppSettings["BalanceUrl"];
        static string msgextraInfo = ConfigurationManager.AppSettings["MsgextraInfo"];

        static string strReg1 = newTemp.DecryptString(ConfigurationManager.AppSettings["MsgReg1"]);
        static string strPwd1 = newTemp.DecryptString(ConfigurationManager.AppSettings["MsgPwd1"]);
        static string strSourceAdd1 = ConfigurationManager.AppSettings["MsgSourceAdd1"];
        static string url1 = ConfigurationManager.AppSettings["MsgUrl1"];


        
        public static void Send(IMsgSendArgs args, MsgSendType type)
        {
            Send(args, type,args.ReceiveTels_G);
        }

        static string GetStrReg(int channelID)
        { 
            string _key="MsgReg";
            if (channelID>0)
            {
               _key=_key+channelID.ToString();
            }
            return  newTemp.DecryptString(ConfigurationManager.AppSettings[_key]);
        
        }
        public static void Send(IMsgSendArgs args, MsgSendType type, string strPhone)
        {
            if (!IsUsingMsg)
            {
                return;
            }

            if (string.IsNullOrEmpty(strPhone))
            {
                return;
            }

            if (!IsUsing(args,type))
            {
                return;
            }
            string _msg = GetMsg(args, type) + msgextraInfo;
            //短信内容
            string strContent = HttpUtility.UrlEncode(_msg, Encoding.UTF8);

            //要发送的内容
            string strSend = "reg=" + strReg + "&pwd=" + strPwd + "&sourceadd=" + strSourceAdd +
                             "&phone=" + strPhone + "&content=" + strContent;

            //发送
            HttpSend.postSend(url, strSend);
        }

        public static bool DirectSend(string Msginfo, string strPhone,SendUserInfo userinfo,int channelID=0)
        {
            bool blsendok = false;
           
            if (string.IsNullOrEmpty(strPhone))
            {
                return false;
            }
            if (string.IsNullOrEmpty(Msginfo))
            {
                return false;
            }
            string _msg = Msginfo + msgextraInfo;

            int _percount = 40;
            int i = _msg.Length / _percount;
            int j = _msg.Length % _percount;
            if (j > 0)
            {
                i = i + 1;
            }
            string[] _ps = strPhone.Split(',');
            int _p = _ps.Length;

            int psCount = _p * i;
            //短信内容
            string strContent = HttpUtility.UrlEncode(_msg, Encoding.UTF8);

            //要发送的内容
            string strSend = "";
            string newurl = url;
            if (channelID == 0)
            {
                strSend = "reg=" + strReg + "&pwd=" + strPwd + "&sourceadd=" + strSourceAdd +
                                  "&phone=" + strPhone + "&content=" + strContent;
            }
            else
            {
                strSend = "reg=" + strReg1 + "&pwd=" + strPwd1 + "&sourceadd=" + strSourceAdd1 +
                                                  "&phone=" + strPhone + "&content=" + strContent;
                newurl = url1;
            }
            string r = string.Empty;
            DateTime beginSendTime = DateTime.Now;
            int success = 0;
            try
            {
                //发送
                r = HttpSend.postSend(newurl, strSend);
            }
            catch { }
            if (!string.IsNullOrEmpty(r) && r.Split('&')[0].Split('=')[1].Equals("0"))
            {
                blsendok= true;
                success = psCount;
            }
            
            DateTime endSendTime = DateTime.Now;
            //记录日志
            if ((userinfo != null) && (userinfo.isLog == 1))
            {
                try
                {
                    #region 维护短信发送记录

                    DataProvider.GetEAP_Provider().ExecuteNonQuery("insert into EAP_MsgRecord(orgid,senddate,enddate,telcount,successcount,failcount,domain,notetxt,Phones,senduser) values(@orgid,@senddate,@enddate,@telcount,@successcount,@failcount,@domain,@notetxt,@phones,@Senduser)",
                        new SqlParameter("@orgid", userinfo.orgid), new SqlParameter("@senddate", beginSendTime),
                        new SqlParameter("@enddate", endSendTime), new SqlParameter("@telcount", psCount),
                        new SqlParameter("@successcount", success), new SqlParameter("@failcount", psCount - success),
                        new SqlParameter("@domain", channelID.ToString()), new SqlParameter("@notetxt", _msg),
                         new SqlParameter("@phones", strPhone), new SqlParameter("@Senduser", userinfo.username) 
                        );

                    #endregion
                }
                catch { }
            }
            //

            return blsendok;
        }

        public static int BalanceQuery()
        {
            //要发送的内容
            string strSend = "reg=" + strReg + "&pwd=" + strPwd;
            //发送
            string strResult = HttpSend.postSend(balanceUrl, strSend);
            if (!string.IsNullOrEmpty(strResult))
            {
                try
                {
                    string c = strResult.Split('&')[1].Split('=')[1];
                    if (!string.IsNullOrEmpty(c))
                    {
                        return Convert.ToInt32(c);
                    }
                }
                catch { }
            }
            return 0;
        }

        static string GetMsg(IMsgSendArgs args, MsgSendType type)
        {
            string msg = string.Empty;
            switch (type)
            {
                case MsgSendType.Rec:
                    msg = string.Format("收货--收货单号:{0},金额:{1},时间:{2}", args.BillCode_G, args.BillAmount_G.ToString(), DateTime.Now.ToString());
                    break;
                case MsgSendType.Sell:
                    msg = string.Format("销售--出库单号:{0},金额:{1},时间:{2}", args.BillCode_G, args.BillAmount_G.ToString(), DateTime.Now.ToString());
                    break;
                case MsgSendType.DaySum:
                    msg = string.Format("日结--今日收货金额:{0},销售金额:{1},时间:{2}", args.RecAmount_G.ToString(), args.SellAmount_G.ToString(), DateTime.Now.ToShortDateString());
                    break;
            }
            return msg;
        }

        static bool IsUsing(IMsgSendArgs args,MsgSendType type)
        {
            if (args.IsUsing_G == 0)
            {
                return false;
            }
            switch (type)
            {
                case MsgSendType.Rec:
                    if (args.IsSend_Rec_G == 1)
                    {
                        return true;
                    }
                    break;
                case MsgSendType.Sell:
                    if (args.IsSend_Sell_G == 1)
                    {
                        return true;
                    }
                    break;
                case MsgSendType.DaySum:
                    if (args.IsSend_DaySum_G == 1)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }

    /// <summary>
    /// HttpSend 的摘要说明
    /// </summary>
    public class HttpSend
    {
        public HttpSend()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// post方法
        /// </summary>
        /// <param name="url">服务器URL</param>
        /// <param name="param">要发送的参数字符串</param>
        /// <returns>服务器返回字符串</returns>
        public static string postSend(string url, string param)
        {
            byte[] postBytes = Encoding.ASCII.GetBytes(param);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            req.ContentLength = postBytes.Length;

            try
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }

                using (WebResponse res = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                    {
                        string strResult = sr.ReadToEnd();
                        return strResult;
                    }
                }

            }
            catch (WebException ex)
            {
                return "无法连接到服务器\r\n错误信息：" + ex.Message;
            }

        }
        /// <summary>
        /// get方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string getSend(string url, string param)
        {
            string address = url + "?" + param;
            Uri uri = new Uri(address);
            WebRequest webReq = WebRequest.Create(uri);

            try
            {
                using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse())
                {
                    using (Stream respStream = webResp.GetResponseStream())
                    {
                        using (StreamReader objReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding("UTF-8")))
                        {
                            string strRes = objReader.ReadToEnd();
                            return strRes;
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                return "无法连接到服务器/r/n错误信息：" + ex.Message;
            }
        }
    }

#endif
}

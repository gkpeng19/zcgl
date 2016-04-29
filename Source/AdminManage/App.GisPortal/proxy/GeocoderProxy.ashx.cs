using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GIS.Portal.proxy
{
    /// <summary>
    /// Summary description for GeocoderProxy
    /// </summary>
    public class GeocoderProxy : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string value = context.Request.Params["value"];
            string decodeStr = HttpUtility.UrlDecode(value);
            //string decodeStr = Encoding.GetEncoding("GB2312").GetString(Encoding.Default.GetBytes(value));

            string url = "http://10.246.0.81:10000/service/AddrCode/cmd?commandType=0&username=syllhjxxjx&password=syllhjxxjx123&AddrName=" +
                decodeStr + "&IsAccurate=false&AddrType=AllType&Start=0&Nums=10";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            req.Method = context.Request.HttpMethod;
            req.ServicePoint.Expect100Continue = false;
            req.Referer = context.Request.Headers["referer"];
            req.Method = "GET";
            using (System.Net.WebResponse wr = req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
                {
                    context.Response.Write(sr.ReadToEnd());
                    sr.Close();
                }
            }
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
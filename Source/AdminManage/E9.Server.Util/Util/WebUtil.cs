using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NM.Util
{
    public class WebUtil
    {
        public const string DEFAULT_APP_PAGE = "Default.aspx";

        static public string CombineUrls(string baseUrl, string additionalUrl)
        {
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            }
            if (additionalUrl.StartsWith("/"))
            {
                additionalUrl = additionalUrl.Substring(1);
            }
            return baseUrl + "/" + additionalUrl;
        }

        static public string GetAbsUrl(string relUrl)
        {
            if (string.IsNullOrEmpty(relUrl))
            {
                relUrl = DEFAULT_APP_PAGE;
            }
            return CombineUrls(HttpContext.Current.Request.ApplicationPath, relUrl);
        }
    }
}

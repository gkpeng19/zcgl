using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Util;
using System.Runtime.Serialization;
using System.IO;
#if SILVERLIGHT
#else
using System.Web;
using System.Web.Configuration;
#endif
namespace NM.Model
{
    public class ClientContext : TJson
    {
        public string ServerUrl { get; set; }
        public string MachineName { get; set; }
        public int ProxyPort { get; set; }
        public Uri ServerUri
        {
            get
            {
                var u = new Uri(ServerUrl, UriKind.Absolute);
                return u;
            }
        }

#if SILVERLIGHT
#else
        public static ClientContext GetClientContext(HttpContext c)
        {
            ClientContext r = new ClientContext();
            var s = WebConfigurationManager.AppSettings["ProxyUri"];
            if (!string.IsNullOrEmpty(s))
            {
                r.ServerUrl = s;
            }
            else
            {
                r.ServerUrl = c.Request.Url.AbsoluteUri;//.Replace("default.aspx", "");
            }
            r.MachineName = c.Server.MachineName;
            return r;
        }
#endif
    }

    public class UploadFileArgs : TJson
    {
        public int SuccessID { get; set; }
        [IgnoreDataMember]
        public Stream Stream { get; set; }
        public string ErrorMsg { get; set; }
    }
}

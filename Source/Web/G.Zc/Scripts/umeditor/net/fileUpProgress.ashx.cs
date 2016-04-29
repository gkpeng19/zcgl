using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Scripts.umeditor.net
{
    public class fileUpProgress : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            var progresskey = context.Request["progresskey"];
            var progress = HttpContext.Current.Cache[progresskey];
            if (progress == null)
            {
                progress = 0;
            }
            if (int.Parse(progress.ToString()) == 100)
            {
                HttpContext.Current.Cache.Remove(progresskey);
            }
            context.Response.Write(progress);
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
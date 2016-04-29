using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using NM.Log;
using NM.OP;
using NM.Service;
using NM.Util;
using System.Net.Mail;
using System.Net;
using NM.Mail;

namespace NM.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DownLoadFileHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
             var fileName = context.Request["FileName"];
            fileName = HttpUtility.UrlDecode(fileName);
            var fullFileName=string.Empty;
            //if (fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase) || fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) || fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    fullFileName = context.Server.MapPath("~/") + "ClientBin/UpLoad/" + fileName;
            //}
            //else
            //{
                fullFileName = context.Server.MapPath("~/") + "data/" + fileName;
            //}
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment;  filename=" +
                   HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            if(File.Exists(fullFileName))
            {
                 context.Response.WriteFile(fullFileName);
            }
            else
                  context.Response.Write("文件不存在");
            context.Response.End();
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}

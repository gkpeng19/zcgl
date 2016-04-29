using G.Util.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebSite.Scripts.umeditor.net
{
    public class fileUp : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            string progresskey = context.Request["progresskey"];

            int[,] thumbs = null;

            //上传配置
            string pathbase = "upload/"; //保存路径

            //文件允许格式
            string[] filetypes = null;
            string callback = null;
            if (context.Request["iseditor"] == null)//从百度编辑器上传
            {
                string editorId = context.Request["editorid"];
                filetypes = new string[] { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
                pathbase = pathbase + "images/";

                callback = context.Request["callback"];

                thumbs = new int[,] { { 260, 0 }, { 420, 0 }, { 480, 0 }, { 580, 0 } };
            }
            else
            {
                string filter = context.Request["filter"];
                if (filter != null && filter.Length > 0)
                {
                    if (filter.Equals("image"))
                    {
                        filetypes = new string[] { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
                        pathbase = pathbase + "images/";
                    }
                    else if (filter.Equals("video"))
                    {
                        filetypes = new string[] { ".mp4", ".flv" };
                        pathbase = pathbase + "videos/";
                    }
                    else if (filter.Equals("text"))
                    {
                        filetypes = new string[] { ".html", ".htm" };
                        pathbase = pathbase + "text/";
                    }
                    else if (filter.Equals("application"))
                    {
                        filetypes = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".html", ".htm" };
                        pathbase = pathbase + "documents/";
                    }
                }

                string thumbstr = context.Request["thumbs"];
                if (thumbstr != null && thumbstr.Length > 0)
                {
                    var ts = thumbstr.Split(',');
                    thumbs = new int[ts.Length, 2];
                    for (var i = 0; i < ts.Length; ++i)
                    {
                        var tstr = ts[i].Split('*');
                        thumbs[i, 0] = int.Parse(tstr[0]);
                        if (tstr.Length > 1)
                        {
                            thumbs[i, 1] = int.Parse(tstr[1]);
                        }
                        else
                        {
                            thumbs[i, 1] = 0;
                        }
                    }
                }
            }

            int size = 0;                     //文件大小限制,单位mb
            string maxSize = context.Request["maxSize"];
            if (maxSize != null && maxSize.Length > 0)
            {
                int.TryParse(maxSize, out size);
            }
            if (size == 0)
            {
                size = 10;
            }

            //上传图片
            Hashtable info;
            UMeditorUploader up = new UMeditorUploader();
            info = up.upFile(context, pathbase, filetypes, size, progresskey); //获取上传状态
            var path = info["path"];
            var savepath = pathbase + path;
            info["path"] = savepath;
            string json = BuildJson(info);

            #region 生成缩略图

            if (thumbs != null)
            {
                Thread thread = new Thread((ctx) =>
                {
                    var server = ((HttpContext)ctx).Server;
                    for (var i = 0; i < thumbs.GetLength(0); ++i)
                    {
                        try
                        {
                            ImageUtil.MakeThumbnail(server.MapPath("~/" + savepath),
                            server.MapPath("~/" + pathbase + "size" + thumbs[i, 0] + "/" + path), thumbs[i, 0], thumbs[i, 1]);
                        }
                        catch { }
                    }
                });
                thread.Start(context);
            }

            #endregion

            context.Response.ContentType = "text/html";
            if (callback != null)
            {
                context.Response.Write(String.Format("<script>{0}(JSON.parse(\"{1}\"));</script>", callback, json));
            }
            else
            {
                var callbackurl = ConfigurationManager.AppSettings["uploadcallbackurl"];
                context.Response.Redirect(callbackurl + "?d=" + json);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string BuildJson(Hashtable info)
        {
            List<string> fields = new List<string>();
            string[] keys = new string[] { "originalName", "name", "url", "path", "size", "state", "type" };
            for (int i = 0; i < keys.Length; i++)
            {
                fields.Add(String.Format("\"{0}\": \"{1}\"", keys[i], info[keys[i]]));
            }
            return "{" + String.Join(",", fields) + "}";
        }
    }
}
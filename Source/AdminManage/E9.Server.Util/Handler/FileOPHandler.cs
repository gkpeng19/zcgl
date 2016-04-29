using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;
using NM.Model;

namespace NM.Handler
{
    public class FileOPHandler : IHttpHandler
    {
        /// <summary>
        /// 1:上传  2:下载 3:删除
        /// </summary>
        int opType = 0;
        string fileName = string.Empty;
        string tempExtension = "_temp";
        UploadFileArgs args = new UploadFileArgs();

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                opType = Convert.ToInt32(context.Request["op"]);
                fileName = context.Request["fileName"];
            }
            catch
            {
                args.SuccessID = 0;
                args.ErrorMsg = "文件操作参数错误";
                context.Response.Write(args.ToJson());
                return;
            }
            string spath = string.Empty;//加扩展名后的路径
            //正确的路径
            string dpath = HostingEnvironment.ApplicationPhysicalPath + @"\Data\" + fileName;
            spath = dpath + tempExtension;


            switch (opType)
            {
                case 1:
                    {
                        FileUpload(context, spath, dpath);
                        break;
                    }
                case 2:
                    {
                        FileDownload(context, dpath);
                        break;
                    }
                case 3:
                    {
                        FileDelete(context, dpath);
                        break;
                    }
            }
        }

        void FileUpload(HttpContext context, string spath, string dpath)
        {
            if (context.Request.InputStream.Length <= 0)
            {
                args.ErrorMsg = "上传文件为空!";
                args.SuccessID = 0;
            }
            else
            {
                try
                {
                    SaveFile(context.Request.InputStream, spath);
                    File.Move(spath, dpath);
                    args.SuccessID = 1;
                }
                catch (Exception e)
                {
                    args.SuccessID = 0;
                    args.ErrorMsg = e.Message;
                }
            }
            context.Response.Write(args.ToJson());
        }

        void FileDownload(HttpContext context, string path)
        {
            if (File.Exists(path))
            {
                context.Response.WriteFile(path);
            }
        }

        void FileDelete(HttpContext context, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                args.SuccessID = 1;
                args.ErrorMsg = "文件删除成功!";
            }
            else
            {
                args.SuccessID = 0;
                args.ErrorMsg= "文件不存在!";
            }
            context.Response.Write(args.ToJson());
        }

        void SaveFile(Stream stream, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                stream.CopyTo(fs);
                //byte[] bits = new byte[4096];
                //int byteRead = 0;
                //while ((byteRead = stream.Read(bits, 0, bits.Length)) != 0)
                //{
                //    fs.Write(bits, 0, bits.Length);
                //}
            }
        }
    }
}

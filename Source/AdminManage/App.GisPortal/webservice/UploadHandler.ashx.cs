using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace GIS.Portal.webservice
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private const string UPLOAD_FOLDER = "~/uploads/";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";

            string resultVal = ReturnVal.Failed.ToString();
            try
            {
                HttpFileCollection fileColl = context.Request.Files;
                for (int i = 0; i < fileColl.Count; i++)
                {
                    HttpPostedFile postedFiles = fileColl[i];
                    if (postedFiles != null)
                    {
                        if (postedFiles.InputStream.Length != 0)
                        {
                            string originalFileName = System.IO.Path.GetFileName(postedFiles.FileName);
                            string ext = System.IO.Path.GetExtension(originalFileName);
                            string newFileName = string.Format("{0}{1}", Guid.NewGuid(), ext);
                            string path = context.Server.MapPath(UPLOAD_FOLDER);
                            if (!System.IO.Directory.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path);
                            }
                            string file = "uploads" + Path.DirectorySeparatorChar + newFileName;
                            string fileAbsPath = context.Server.MapPath(UPLOAD_FOLDER) + newFileName;
                            postedFiles.SaveAs(fileAbsPath);
                            //将上传的文件信息保存到数据库
                            resultVal = file;
                        }
                        else
                        {
                            resultVal = ReturnVal.FileEmpty.ToString();
                        }
                    }
                    else
                    {
                        resultVal = ReturnVal.NotSelected.ToString();
                    }
                }

            }
            catch (Exception)
            {
                resultVal = ReturnVal.Failed.ToString();
            }
            finally
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                context.Response.Charset = "utf-8";
                context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                context.Response.Write(serializer.Serialize(resultVal));
                context.Response.End();
            }
        }
        private enum ReturnVal : int
        {
            /// <summary>
            /// 不能上传 0 K大小的文件
            /// </summary>
            FileEmpty = -2,
            /// <summary>
            /// 未选中文件
            /// </summary>
            NotSelected = -1,
            /// <summary>
            /// 上传失败
            /// </summary>
            Failed = 0,
            /// <summary>
            /// 上传成功
            /// </summary>
            Succeed = 1
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
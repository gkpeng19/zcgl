﻿using System;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Hosting;

namespace NM.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UploadPictureHandler : IHttpHandler
    {

        private HttpContext _httpContext;
        private string _tempExtension = "_temp";
        private string _fileName;
        private string _path;
        private string _parameters;
        private bool _lastChunk;
        private bool _firstChunk;
        private long _startByte;

        StreamWriter _debugFileStreamWriter;
        TextWriterTraceListener _debugListener;

        /// <summary>
        /// Start method
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            _httpContext = context;

            if (context.Request.InputStream.Length == 0)
                throw new ArgumentException("No file input");

            try
            {
                //StartDebugListener();

                GetQueryStringParameters();

                string uploadFolder = GetUploadFolder();
                string tempFileName = _fileName + _tempExtension;

                string tempPath = GetTempFilePath(tempFileName);
                string targetPath = GetTargetFilePath(_fileName);

                //Is it the first chunk? Prepare by deleting any existing files with the same name
                if (_firstChunk)
                {
                    Debug.WriteLine("First chunk arrived at webservice");

                    //Delete temp file               
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    //Delete target file                
                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                }

                //Write the file
                Debug.WriteLine(string.Format("Write data to disk FOLDER: {0}", uploadFolder));

                using (FileStream fs = File.Open(tempPath, FileMode.Append))
                {
                    SaveFile(context.Request.InputStream, fs);
                    fs.Close();
                }

                Debug.WriteLine("Write data to disk SUCCESS");

                //Is it the last chunk? Then finish up...
                if (_lastChunk)
                {
                    Debug.WriteLine("Last chunk arrived");

                    //Rename file to original file
                    File.Move(tempPath, targetPath);

                    //Finish stuff....
                    FinishedFileUpload(_fileName, _parameters);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

                throw;
            }
            finally
            {
                //StopDebugListener();
            }

        }

        /// <summary>
        /// Get the querystring parameters
        /// </summary>
        private void GetQueryStringParameters()
        {
            _fileName = _httpContext.Request.QueryString["file"];
            _path = _httpContext.Request.QueryString["serverPath"];
            _parameters = _httpContext.Request.QueryString["param"];
            _lastChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["last"]) ? true : bool.Parse(_httpContext.Request.QueryString["last"]);
            _firstChunk = string.IsNullOrEmpty(_httpContext.Request.QueryString["first"]) ? true : bool.Parse(_httpContext.Request.QueryString["first"]);
            _startByte = string.IsNullOrEmpty(_httpContext.Request.QueryString["offset"]) ? 0 : long.Parse(_httpContext.Request.QueryString["offset"]); ;
        }

        /// <summary>
        /// Save the contents of the Stream to a file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fs"></param>
        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Do your own stuff here when the file is finished uploading
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        protected virtual void FinishedFileUpload(string fileName, string parameters)
        {
        }

        protected virtual string GetUploadFolder()
        {
            var s = @HostingEnvironment.ApplicationPhysicalPath + "\\Data\\" + _path;

            if (!Directory.Exists(s))
                Directory.CreateDirectory(s);
            return s;
        }

        protected string GetTempFilePath(string fileName)
        {
            return Path.Combine(@HostingEnvironment.ApplicationPhysicalPath, Path.Combine(GetUploadFolder(), fileName));
        }

        protected string GetTargetFilePath(string fileName)
        {
            return Path.Combine(@HostingEnvironment.ApplicationPhysicalPath, Path.Combine(GetUploadFolder(), fileName));
        }


        /// <summary>
        /// Write debug output to a textfile in debug mode
        /// </summary>
        [Conditional("DEBUG")]
        private void StartDebugListener()
        {
            try
            {
                _debugFileStreamWriter = System.IO.File.AppendText("debug.txt");
                _debugListener = new TextWriterTraceListener(_debugFileStreamWriter);
                Debug.Listeners.Add(_debugListener);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clean up the debug listener
        /// </summary>
        [Conditional("DEBUG")]
        private void StopDebugListener()
        {
            try
            {
                Debug.Flush();
                _debugFileStreamWriter.Close();
                Debug.Listeners.Remove(_debugListener);
            }
            catch
            {
            }
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

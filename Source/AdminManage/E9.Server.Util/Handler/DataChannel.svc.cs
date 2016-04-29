using System;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using System.Web;
using NM.Log;
using NM.OP;
using NM.Service;
using NM.Util;

namespace NM.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataChannel" in code, svc and config file together.
    [AspNetCompatibilityRequirementsAttribute(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DataChannel : IDataChannel
    {
        DataResponse result;
        DataRequest request = null;
        StringBuilder sbDebug = new StringBuilder();
        DataProvider _DataSource;
        public string ProcessRequest(string query)
        {
            request = TJson.Parse<DataRequest>(query);
            result = new DataResponse();
            LogManager.OnLogEvent += new LogEvent(LogManager_OnLogEvent);
            ServiceContext serviceContext = null;
            try
            {
                SetCulture(request);
                result.ServiceName = request.ServiceName;
                serviceContext = new ServiceContext(request, result, DataSource, HttpContext.Current);
                DoProcess(serviceContext);

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
            return result.ToJson();
        }


        protected DataProvider DataSource
        {
            get
            {
                if (null == _DataSource)
                    _DataSource = DataProvider.GetProvider(request.DataConnection);

                return _DataSource;
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
    }
}

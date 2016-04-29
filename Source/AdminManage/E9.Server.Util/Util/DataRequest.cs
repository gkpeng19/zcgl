using System;
using System.Collections.Generic;
using System.Text;
using NM.Model;
using System.Threading;


namespace NM.Util
{
    public enum ReturnCode
    {
        None,
        Success,
        Fail,
        Error,
        Ignore
    }

    public enum HttpWebMethod
    {
        GET,
        POST
    }

    public class DataRequest : TJson
    {
        public DataRequest()
        {
#if SILVERLIGHT
            ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
            ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
#endif
        }

        public HttpWebMethod Method { get; set; }
        public LoginInfo LogIn { get; set; }
        public string ServiceName { get; set; }
        public string Category { get; set; }
        public string DataConnection { get; set; }
        private List<string> _Keys;
        public string ClientCulture { get; set; }
        public string ClientUICulture { get; set; }

        public List<string> Keys
        {
            get
            {
                if (_Keys == null)
                    _Keys = new List<string>();
                return _Keys;
            }
            set { _Keys = value; }
        }

        private List<string> _Values;
        public List<string> Values
        {
            get
            {
                if (_Values == null)
                    _Values = new List<string>();
                return _Values;
            }
            set { _Values = value; }
        }

        public string this[string key]
        {
            get
            {
                int index = Keys.IndexOf(key);
                return index >= 0 ? Values[index] : "";
            }
        }

        public string this[int index]
        {
            get
            {
                return this["P" + index];
            }
        }

        public void Add(string key, string value)
        {
            Keys.Add(key);
            Values.Add(value);
        }

        public string Parameters
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < Keys.Count; index++)
                {
                    sb.AppendFormat("{0} ={1};", Keys[index], Values[index]);
                }
                return sb.ToString();
            }
        }
    }

    public class DataResponse : TJson
    {
        public DataResponse()
        {
            ReturnTime = DateTime.Now;
            SubMessages = new List<DataResponse>();
            Result = ReturnCode.None;
        }

        public DataResponse(string actionName)
            : this()
        {
            ServiceName = actionName;
        }

        public DataResponse(string actionName, ReturnCode result, string value, string message)
            : this(actionName)
        {
            Result = result;
            Value = value;
            Message = message;
        }

        public ReturnCode Result { get; set; }
        public DateTime ReturnTime { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ServiceName { get; set; }
        public string DebugInfo { get; set; }
        public List<DataResponse> SubMessages;

        public DataResponse AddMessage(string actionName, ReturnCode result, string value, string message)
        {
            DataResponse mr = new DataResponse(actionName, result, value, message);
            SubMessages.Add(mr);
            return mr;
        }
    }
}

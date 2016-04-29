
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-22 16:56:59  </CreateDate>
 /// <summary>  
///  LogItem.cs
 /// <summary>  
/// <Update>2007-6-24 14:35:36</Update> 
/// <remarks> </remarks>

using System;
using System.Diagnostics;
using System.Text;

namespace NM.Log
{    
    public class LogItem
    {
        private static string LogString = "{0,-3}|{1,-5}|{2}| {3}";
        public LogItem()
        {
        }

        public LogItem(int SessionID, string message)
        {
            _SessionID = SessionID;
            _Message = message;
            StackTrace st = new StackTrace(new StackFrame(1));
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < st.FrameCount; index++)
            {
                

                StackFrame sf = st.GetFrame(index);
                
//                sb.Append(sf.GetFileName());
                sb.Append(sf.GetMethod());
                //sb.Append(sf.GetFileLineNumber());
            }
            CallPath = sb.ToString();
        }

        public LogItem(int SessionID, string message, LogLevel LogLevel)
            : this(SessionID, message)
        {
            _LogLevel = LogLevel;
        }

        private int _ID;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private LogLevel _LogLevel;//日值等级

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel
        {
            get
            {
                return _LogLevel;
            }
            set
            { _LogLevel = value; }
        }

        private DateTime _Time = DateTime.Now;

        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        private int _SessionID;

        public int SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public string CallPath { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return string.Format(LogString, new object[] { _SessionID, LogLevel, Time, Message });
        }

        public void Parse(string logtext)
        {
            if (!string.IsNullOrEmpty(logtext))
            {
                string[] strs = logtext.Split("|".ToCharArray());
                _SessionID = Int32.Parse(strs[0]);
                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), strs[1], false);
                Time = DateTime.Parse(strs[2]);
                Message = strs[3];
            }
        }
    }
}


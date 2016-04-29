using System;

namespace NM.Log
{
    public delegate void LogEvent(LogEventArgs e);

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs()
        {
        }

        public LogEventArgs(LogItem log)
        {
            Log = log;
        }

        private LogItem _Log;

        public LogItem Log
        {
            get { return _Log; }
            set { _Log = value; }
        }
    }
}

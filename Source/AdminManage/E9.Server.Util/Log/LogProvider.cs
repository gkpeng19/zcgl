using System;
using System.Collections.Generic;

namespace NM.Log
{
    public class LogProviderCollection : List<LogProvider>
    {
        public new void Add(LogProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is LogProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }
            base.Add(provider);
        }

        public void LogIt(LogItem log)
        {
            foreach (ILogWriter w in this)
                w.LogIt(log);
        }

        public void Flush()
        {
            foreach (ILogWriter write in this)
            {
                write.Flush();
            }
        }
    }


    public abstract class LogProvider : IlogReader, ILogWriter
    {

        ~LogProvider()
        {
            Flush();
        }

        private bool _Enable = true;

        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        private LogLevel _LogLevel = LogLevel.Error;

        public LogLevel LogLevel
        {
            get { return _LogLevel; }
            set
            {
                _LogLevel = value;
            }
        }

        private int _BatchWrite = 6;
        public int BatchWrite
        {
            get { return _BatchWrite; }
            set
            {
                _BatchWrite = value;
            }
        }

        private List<LogItem> _List;

        internal List<LogItem> List
        {
            get
            {
                if (_List == null)
                    _List = new List<LogItem>();
                return _List;
            }
        }

        private void CatchLogItem(LogItem _Log)
        {
            List.Add(_Log);
            if (List.Count >= BatchWrite)
            {
                Flush();
            }
        }

        public bool LogIt(LogItem log)
        {
            if (MathLogLevel(log.LogLevel))
            {
                CatchLogItem(log);
            }
            return true;
        }

        public bool Flush()
        {
            if (List.Count > 0)
            {
                //写记录
                Save(_List);
                //清空当前
                List.Clear();
            }
            return true;
        }
        private bool MathLogLevel(LogLevel logLevel)
        {
            return logLevel <= LogLevel;
        }

        #region IlogReader Members


        public abstract List<LogItem> Load(DateTime time);

        #endregion

        #region ILogWriter Members

        private bool Save(List<LogItem> _Logs)
        {
            return DoSave(_Logs);
        }

        protected abstract bool DoSave(List<LogItem> _Logs);

        #endregion
    }
}

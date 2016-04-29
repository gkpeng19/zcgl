/// <summary>
/// Purpose: Logging 
/// Created By : SHuangfu
/// Created On: 6/2/2007
/// 
/// Purpose :
/// Modified By :
/// Modified On :
/// </summary>

using System;
using System.Diagnostics;

namespace NM.Log
{
    public class LogManager
    {
        //    public static readonly LogManager Default = new LogManager();

        public LogManager()
        {
        }

        public static event LogEvent OnLogEvent;

        private static LogProviderCollection  Providers=new LogProviderCollection(); 


        public static void AddWriter(LogProvider writer)
        {
            Providers.Add(writer);
        }

        public static void Reset()
        {
            Providers = null;
        }

        public static void LogIt(LogItem log)
        {
            if (Debugger.IsAttached)
                Trace.WriteLine(log.ToString());
            if (OnLogEvent != null)
                OnLogEvent(new LogEventArgs(log));
            Providers.LogIt(log);
        }
           
        public static void Flush()
        {
            Providers.Flush();
        }

        public static void HintIt(int SessionID, string message)
        {
            LogIt(new LogItem(SessionID, message, LogLevel.Hint));
        }

        public static void HintIt(string message)
        {
            HintIt(0, message);
        }

        public static void InforIt(int SessionID, string message)
        {
            LogIt(new LogItem(SessionID, message, LogLevel.Infor));
        }

        public static void InforIt(string message)
        {
            InforIt(0, message);
        }

        public static void WarningIt(int SessionID, string message)
        {
            LogIt(new LogItem(SessionID, message, LogLevel.Warning));
        }

        public static void WarningIt(string message)
        {
            WarningIt(0, message);
        }

        public static void DebugIt(int SessionID, string message)
        {
            LogIt(new LogItem(SessionID, message, LogLevel.Debug));
        }

        public static void DebugIt(string message)
        {
            DebugIt(0, message);
        }

        public static void ErrorIt(int SessionID, string message)
        {
            LogIt(new LogItem(SessionID, message, LogLevel.Error));
        }

        public static void ErrorIt(string message)
        {
            ErrorIt(0, message);
        }

        public static void LogException(Exception ex)
        {
            ErrorIt(ex.Message);
            DebugIt(ex.ToString());
        }  
    }
}

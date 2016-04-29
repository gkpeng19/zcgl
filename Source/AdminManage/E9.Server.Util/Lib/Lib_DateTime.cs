
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-15 17:24:19  </CreateDate>
 /// <summary>  
///  Lib_DateTime.cs
 /// <summary>  
/// <Update>2007-6-15 17:30:47</Update> 
/// <remarks> </remarks>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NM.Util
{
    public static class Lib_DateTime
    {

        public static int Seconds(DateTime t1, DateTime t2)
        {
            TimeSpan spen = t1 - t2;
            return Convert.ToInt32(spen.TotalSeconds);
        }

        public static int Seconds(TimeSpan spen)
        {
            return Convert.ToInt32(spen.TotalSeconds);
        } 

        public static void SetSysDateTime(DateTime dt)
        {
            SystemTime sysTime = new SystemTime();

            sysTime.wYear = Convert.ToUInt16(dt.Year);
            sysTime.wMonth = Convert.ToUInt16(dt.Month);
            //处置北京时间 
            int nBeijingHour = dt.Hour - 8;
            if (nBeijingHour <= 0)
            {
                nBeijingHour += 24;
                sysTime.wDay = Convert.ToUInt16(dt.Day - 1);
                sysTime.wDayOfWeek = Convert.ToUInt16(dt.DayOfWeek - 1);
            }
            else
            {
                sysTime.wDay = Convert.ToUInt16(dt.Day);
                sysTime.wDayOfWeek = Convert.ToUInt16(dt.DayOfWeek);
            }
            sysTime.wHour = Convert.ToUInt16(nBeijingHour);
            sysTime.wMinute = Convert.ToUInt16(dt.Minute);
            sysTime.wSecond = Convert.ToUInt16(dt.Second);
            sysTime.wMiliseconds = Convert.ToUInt16(dt.Millisecond);
            Win32.SetSystemTime(ref sysTime);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }

    public class Win32
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetSystemTime(ref SystemTime sysTime);
        [DllImport("Kernel32.dll")]
        public static extern void GetSystemTime(ref SystemTime sysTime);
    }

}


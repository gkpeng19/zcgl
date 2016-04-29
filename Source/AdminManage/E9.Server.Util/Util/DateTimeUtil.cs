﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace NM.Util
{
    public class DateTimeUtil
    {
        static ChineseLunisolarCalendar Calendar = new ChineseLunisolarCalendar();
        static int[] ChinaCalendarInfo = { 
0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,0x16554,0x056a0,0x09ad0,0x055d2,
0x04ae0,0x0a5b6,0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,0x095b0,0x14977,
0x04970,0x0a4b0,0x0b4b5,0x06a50,0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,
0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,0x186e3,0x092e0,0x1c8d7,0x0c950,
0x0d4a0,0x1d8a6,0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,0x0a950,0x0b557,
 0x06ca0,0x0b550,0x15355,0x04da0,0x0a5b0,0x14573,0x052b0,0x0a9a8,0x0e950,0x06aa0,
0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,0x0f263,0x0d950,0x05b57,0x056a0,
0x096d0,0x04dd5,0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,0x0b6a0,0x195a6,
0x095b0,0x049b0,0x0a974,0x0a4b0,0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,
0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,0x055c0,0x0ab60,0x096d5,0x092e0,
0x0c960,0x0d954,0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,0x092d0,0x0cab5,
0x0a950,0x0b4a0,0x0baa4,0x0ad50,0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,
0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,0x0a4e0,0x0d260,0x0ea65,0x0d530,
0x05aa0,0x076a3,0x096d0,0x04bd7,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,0x0d520,0x0dd45,
0x0b5a0,0x056d0,0x055b2,0x049b0,0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0,
0x14b63};

        /// <summary>
        /// 由农历日期得到阳历日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetDateFromLunarDate(int year, int month, int day)
        {
            if (year < 1902 || year > 2100)
                throw new Exception("只支持1902～2100期间的农历年");
            if (month < 1 || month > 12)
                throw new Exception("表示月份的数字必须在1～12之间");

            if (day < 1 || day > Calendar.GetDaysInMonth(year, month))
                throw new Exception("农历日期输入有误");

            bool IsLeapMonth = month == DoubleMonth(year) ? true : false;

            int num1 = 0, num2 = 0;
            int leapMonth = Calendar.GetLeapMonth(year);

            if (((leapMonth == month + 1) && IsLeapMonth) || (leapMonth > 0 && leapMonth <= month))
                num2 = month;
            else
                num2 = month - 1;

            while (num2 > 0)
            {
                num1 += Calendar.GetDaysInMonth(year, num2--);
            }

            DateTime dt = GetLunarNewYearDate(year);
            return dt.AddDays(num1 + day - 1);
        }

        /// <summary>   
        /// 获取指定年份春节当日（正月初一）的阳历日期   
        /// </summary>   
        /// <param name="year">指定的年份</param>   
        public static DateTime GetLunarNewYearDate(int year)
        {
            DateTime dt = new DateTime(year, 1, 1);
            int cnYear = Calendar.GetYear(dt);
            int cnMonth = Calendar.GetMonth(dt);

            int num1 = 0;
            int num2 = Calendar.IsLeapYear(cnYear) ? 13 : 12;

            while (num2 >= cnMonth)
            {
                num1 += Calendar.GetDaysInMonth(cnYear, num2--);
            }

            num1 = num1 - Calendar.GetDayOfMonth(dt) + 1;
            return dt.AddDays(num1);
        }

        ///<summary>
        ///返回农历年闰月月份1-12 , 没闰返回0
        ///</summary>
        ///<param name="year">农历年</param>
        ///<returns></returns>
        public static int DoubleMonth(int year)
        {
            return (ChinaCalendarInfo[year - 1900] & 0xf);
        }

    }
}

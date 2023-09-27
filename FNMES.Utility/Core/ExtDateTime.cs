using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Core
{
    public static class ExtDateTime
    {
        /// <summary>
        /// 获取格式化字符串，不带时分秒。格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒。格式："yyyy-MM-dd"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToDateString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            return ToDateString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 获取格式化字符串，不带年月日，格式："HH:mm:ss"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToTimeString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            return ToTimeString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToMillisecondString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            return ToMillisecondString(dateTime.Value);
        }

        /// <summary>
        /// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            return string.Format("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime dateTime, bool isRemoveSecond = false)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
            result.AppendFormat(" {0}时{1}分", dateTime.Hour, dateTime.Minute);

            if (isRemoveSecond == false)
            {
                result.AppendFormat("{0}秒", dateTime.Second);
            }

            return result.ToString();
        }

        /// <summary>
        /// 获取格式化字符串，带时分秒，格式："yyyy年MM月dd日 HH时mm分"
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="isRemoveSecond">是否移除秒</param>
        public static string ToChineseDateTimeString(this DateTime? dateTime, bool isRemoveSecond = false)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            return ToChineseDateTimeString(dateTime.Value);
        }

        /// <summary>
        ///  返回指定日期起始时间。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime StartDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                throw new ArgumentNullException();
            }

            DateTime nonNullableDateTime = dateTime.Value;
            return new DateTime(nonNullableDateTime.Year, nonNullableDateTime.Month, nonNullableDateTime.Day, 0, 0, 0);
        }
        /// <summary>
        ///  返回指定日期结束时间。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime EndDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                throw new ArgumentNullException();
            }

            DateTime nonNullableDateTime = dateTime.Value;
            return new DateTime(nonNullableDateTime.Year, nonNullableDateTime.Month, nonNullableDateTime.Day, 23, 59, 59);
        }


        #region 获取时间戳
        /// <summary> 
        /// 获取时间戳 
        /// </summary>  
        public static string GetTimeStamp(DateTime dateTime)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64(dateTime.Subtract(dtStart).TotalMilliseconds).ToString();
        }
        #endregion

        #region 根据时间戳获取时间
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public static DateTime TimeStampToDateTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return dtStart.AddMilliseconds(Convert.ToInt64(timeStamp));
        }
        #endregion

        #region 本周开始时间
        /// <summary>
        /// 本周开始时间
        /// </summary>
        public static DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now;
            int day = Convert.ToInt32(now.DayOfWeek.ToString("d"));
            return now.AddDays(1 - day).Date;
        }
        #endregion

        #region 本周结束时间
        /// <summary>
        /// 本周结束时间
        /// </summary>
        public static DateTime GetCurrentWeekEnd()
        {
            return GetCurrentWeekStart().AddDays(7).AddSeconds(-1);
        }
        #endregion

        #region 本月开始时间
        /// <summary>
        /// 本月开始时间
        /// </summary>
        public static DateTime GetCurrentMonthStart()
        {
            DateTime now = DateTime.Now;
            return now.AddDays(1 - now.Day).Date;
        }
        #endregion

        #region 本月结束时间
        /// <summary>
        /// 本月结束时间
        /// </summary>
        public static DateTime GetCurrentMonthEnd()
        {
            return GetCurrentWeekStart().AddMonths(1).AddSeconds(-1);
        }
        #endregion

        #region 本季度开始时间
        /// <summary>
        /// 本季度开始时间
        /// </summary>
        public static DateTime GetCurrentQuarterStart()
        {
            DateTime now = DateTime.Now;
            return now.AddMonths(0 - (now.Month - 1) % 3).AddDays(1 - now.Day).Date;
        }
        #endregion

        #region 本季度结束时间
        /// <summary>
        /// 本季度结束时间
        /// </summary>
        public static DateTime GetCurrentQuarterthEnd()
        {
            return GetCurrentWeekStart().AddMonths(3).AddSeconds(-1);
        }
        #endregion

        #region 本年开始时间
        /// <summary>
        /// 本年开始时间
        /// </summary>
        public static DateTime GetCurrentYearStart()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }
        #endregion

        #region 本年结束时间
        /// <summary>
        /// 本年结束时间
        /// </summary>
        public static DateTime GetCurrentYearEnd()
        {
            return new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
        }
        #endregion

    }
}

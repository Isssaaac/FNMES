using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FNMES.Utility.Core
{
    /// <summary>
    /// ChineseDateTime
    /// 一日有十二时辰，一时辰有四刻，一刻有三盏茶，一盏茶有两柱香
    /// 一柱香有五分，一分有六弹指，一弹指有十刹那，一刹那为一念
    /// </summary>
    public class ChineseDateTime
    {
        #region ====== 内部常量 ======
        private readonly ChineseLunisolarCalendar _chineseDateTime;
        private readonly DateTime _dateTime;
        private readonly int _serialMonth;

        private static readonly string[] _chineseNumber = { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        private static readonly string[] _chineseMonth =
        {
            "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"
        };
        private static readonly string[] _chineseDay =
        {
            "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
            "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
            "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
        };
        private static readonly string[] _chineseWeek =
        {
            "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"
        };

        private static readonly string[] _celestialStem = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static readonly string[] _terrestrialBranch = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private static readonly string[] _chineseZodiac = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        private static readonly string[] _solarTerm =
        {
            "小寒", "大寒", "立春", "雨水", "惊蛰", "春分",
            "清明", "谷雨", "立夏", "小满", "芒种", "夏至",
            "小暑", "大暑", "立秋", "处暑", "白露", "秋分",
            "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
        };
        private static readonly int[] _solarTermInfo = {
            0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989,
            308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758
        };
        #endregion

        #region ======= 构建日期 ======

        public ChineseDateTime(DateTime dateTime)
        {
            _chineseDateTime = new ChineseLunisolarCalendar();
            if (dateTime < _chineseDateTime.MinSupportedDateTime || dateTime > _chineseDateTime.MaxSupportedDateTime)
            {
                throw new ArgumentOutOfRangeException(
                    $"参数日期不在有效的范围内：只支持{_chineseDateTime.MinSupportedDateTime.ToShortTimeString()}到{_chineseDateTime.MaxSupportedDateTime}");
            }

            Year = _chineseDateTime.GetYear(dateTime);
            Month = _chineseDateTime.GetMonth(dateTime);
            Day = _chineseDateTime.GetDayOfMonth(dateTime);
            IsLeep = _chineseDateTime.IsLeapMonth(Year, Month);
            _dateTime = dateTime;
            _serialMonth = Month;
            var leepMonth = _chineseDateTime.GetLeapMonth(Year);
            if (leepMonth > 0 && leepMonth <= Month) Month--;
        }

        /// <summary>
        /// 参数为农历的年月日及是否润月
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="isLeap"></param>
        public ChineseDateTime(int year, int month, int day, bool isLeap = false)
            : this(year, month, day, 0, 0, 0, isLeap)
        {

        }

        public ChineseDateTime(int year, int month, int day, int hour, int minute, int second, bool isLeap = false)
            : this(year, month, day, hour, minute, second, 0, isLeap)
        {

        }

        public ChineseDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, bool isLeap = false)
        {
            _chineseDateTime = new ChineseLunisolarCalendar();
            if (year < _chineseDateTime.MinSupportedDateTime.Year || year >= _chineseDateTime.MaxSupportedDateTime.Year)
            {
                throw new ArgumentOutOfRangeException(
                    $"参数年份不在有效的范围内，只支持{_chineseDateTime.MinSupportedDateTime.Year}到{_chineseDateTime.MaxSupportedDateTime.Year - 1}");
            }

            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException($"月份只支持1-12");
            IsLeep = isLeap;
            var leepMonth = _chineseDateTime.GetLeapMonth(year);
            if (leepMonth - 1 != month)
                IsLeep = false;
            _serialMonth = month;
            if (leepMonth > 0 && (month == leepMonth - 1 && isLeap || month > leepMonth - 1))
                _serialMonth = month + 1;

            if (_chineseDateTime.GetDaysInMonth(year, _serialMonth) < day || day < 1)
                throw new ArgumentOutOfRangeException($"指定的月份天数，不在有效的范围内");

            Year = year;
            Month = month;
            Day = day;
            _dateTime = _chineseDateTime.ToDateTime(Year, _serialMonth, Day, hour, minute, second, millisecond);
        }

        public static ChineseDateTime Now => new ChineseDateTime(DateTime.Now);

        #endregion

        #region ====== 年月日润属性 ======
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        /// <summary>
        /// 是否为润月
        /// </summary>
        public bool IsLeep { get; }
        #endregion

        #region ====== 输出常规日期 ======
        /// <summary>
        /// 转换为公历
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return _chineseDateTime.ToDateTime(Year, _serialMonth, Day, _dateTime.Hour,
                _dateTime.Minute,
                _dateTime.Second, _dateTime.Millisecond);
        }

        /// <summary>
        /// 短日期（农历）
        /// </summary>
        /// <returns></returns>
        public string ToShortDateString()
        {
            return $"{Year}-{GetLeap(false)}{Month}-{Day}";
        }

        /// <summary>
        /// 长日期（农历）
        /// </summary>
        /// <returns></returns>
        public string ToLongDateString()
        {
            return $"{Year}年{GetLeap()}{Month}月-{Day}日";
        }

        public new string ToString()
        {
            return $"{Year}-{GetLeap(false)}{Month}-{Day} {_dateTime.Hour}:{_dateTime.Minute}:{_dateTime.Second}";
        }
        #endregion

        #region ====== 输出中文日期及星期 ======
        public string ToChineseString()
        {
            return ToChineseString("yMd");
        }

        public string GetChineseDate()
        {
            var date = new StringBuilder();
            date.Append(GetMonth() + "月");
            date.Append(GetDay() + "");
            date.AppendLine();
            date.Append(GetEraYear() + ChineseZodiac + "年");
            date.AppendLine();
            return date.ToString();
        }

        public string ToChineseString(string format)
        {
            var year = GetYear();
            var month = GetMonth();
            var day = GetDay();

            var date = new StringBuilder();
            foreach (var item in format.ToCharArray())
            {
                switch (item)
                {
                    case 'y':
                        date.Append($"{year}年");
                        break;
                    case 'M':
                        date.Append($"{month}月");
                        break;
                    case 'd':
                        date.Append($"{day}");
                        break;
                    default:
                        date.Append(item);
                        break;
                }
            }
            var def = $"{year}年{month}月{day}";
            var result = date.ToString();
            return string.IsNullOrEmpty(result) ? def : result;
        }

        public string ChineseWeek => _chineseWeek[(int)_dateTime.DayOfWeek];
        #endregion

        #region ====== 输出天干地支生肖 ======

        public string ToChineseEraString()
        {
            return ToChineseEraString("yMdHm");
        }

        public string ToChineseEraString(string format)
        {
            var year = GetEraYear();
            var month = GetEraMonth();
            var day = GetEraDay();
            var hour = GetEraHour();
            var minute = GetEraMinute();

            var date = new StringBuilder();
            foreach (var item in format.ToCharArray())
            {
                switch (item)
                {
                    case 'y':
                        date.Append($"{year}年");
                        break;
                    case 'M':
                        date.Append($"{month}月");
                        break;
                    case 'd':
                        date.Append($"{day}日");
                        break;
                    case 'H':
                        date.Append($"{hour}时");
                        break;
                    case 'm':
                        date.Append($"{minute}刻");
                        break;
                    default:
                        date.Append(item);
                        break;
                }
            }
            var def = $"{year}年{month}月{day}日{hour}时";
            var result = date.ToString();
            return result.IsNullOrEmpty() ? def : result;
        }

        public string ChineseZodiac => _chineseZodiac[(Year - 4) % 12];
        #endregion

        #region ====== 辅助方法(Chinese) ======
        private string GetYear()
        {
            var yearArray = Array.ConvertAll(Year.ToString().ToCharArray(), x => int.Parse(x.ToString()));
            var year = new StringBuilder();
            foreach (var item in yearArray)
                year.Append(_chineseNumber[item]);
            return year.ToString();
        }

        private string GetMonth()
        {
            return $"{GetLeap()}{_chineseMonth[Month - 1]}";
        }

        private string GetDay()
        {
            return _chineseDay[Day - 1];
        }

        private string GetLeap(bool isChinese = true)
        {
            return IsLeep ? isChinese ? "润" : "L" : "";
        }
        #endregion

        #region ====== 输助方法（天干地支）======
        //年采用的头尾法，月采用的是节令法，主流日历基本上都这种结合，如百度的日历  

        private string GetEraYear()
        {
            var sexagenaryYear = _chineseDateTime.GetSexagenaryYear(_dateTime);
            var stemIndex = _chineseDateTime.GetCelestialStem(sexagenaryYear) - 1;
            var branchIndex = _chineseDateTime.GetTerrestrialBranch(sexagenaryYear) - 1;
            return $"{_celestialStem[stemIndex]}{_terrestrialBranch[branchIndex]}";
        }

        private string GetEraMonth()
        {
            #region ====== 节令法 ======
            var solarIndex = SolarTermFunc((x, y) => x <= y, out var dt);
            solarIndex = solarIndex == -1 ? 23 : solarIndex;
            var currentIndex = (int)Math.Floor(solarIndex / (decimal)2);

            //天干         
            var solarMonth = currentIndex == 0 ? 11 : currentIndex - 1; //计算天干序(月份)
            var sexagenaryYear = _chineseDateTime.GetSexagenaryYear(_dateTime);
            var stemYear = _chineseDateTime.GetCelestialStem(sexagenaryYear) - 1;
            if (solarMonth == 0) //立春时，春节前后的不同处理
            {
                var year = _chineseDateTime.GetYear(dt);
                var month = _chineseDateTime.GetMonth(dt);
                stemYear = year == Year && month != 1 ? stemYear + 1 : stemYear;
            }
            if (solarMonth == 11) //立春在春节后，对前一节气春节前后不同处理
            {
                var year = _chineseDateTime.GetYear(dt);
                stemYear = year != Year ? stemYear - 1 : stemYear;
            }
            int stemIndex;
            switch (stemYear)
            {
                case 0:
                case 5:
                    stemIndex = 3;
                    break;
                case 1:
                case 6:
                    stemIndex = 5;
                    break;
                case 2:
                case 7:
                    stemIndex = 7;
                    break;
                case 3:
                case 8:
                    stemIndex = 9;
                    break;
                default:
                    stemIndex = 1;
                    break;
            }
            //天干序
            stemIndex = (stemIndex - 1 + solarMonth) % 10;

            //地支序
            var branchIndex = currentIndex >= 11 ? currentIndex - 11 : currentIndex + 1;

            return $"{_celestialStem[stemIndex]}{_terrestrialBranch[branchIndex]}";

            #endregion

            #region ====== 头尾法 ======
            //这里算法要容易些，原理和节令法一样，只需取农历整年整月即可。未贴上来
            #endregion
        }

        private string GetEraDay()
        {
            var ts = _dateTime - new DateTime(1901, 2, 15);
            var offset = ts.Days;
            var sexagenaryDay = offset % 60;
            return $"{_celestialStem[sexagenaryDay % 10]}{_terrestrialBranch[sexagenaryDay % 12]}";
        }

        private string GetEraHour()
        {
            var hourIndex = (int)Math.Floor((_dateTime.Hour + 1) / (decimal)2);
            hourIndex = hourIndex == 12 ? 0 : hourIndex;
            return _terrestrialBranch[hourIndex];
        }

        private string GetEraMinute()
        {
            var realMinute = (_dateTime.Hour % 2 == 0 ? 60 : 0) + _dateTime.Minute;
            return $"{_chineseNumber[(int)Math.Floor(realMinute / (decimal)30) + 1]}";
        }
        #endregion

        #region ====== 24节气 ======
        /// <summary>
        /// 当前节气，没有则返回空
        /// </summary>
        public string SolarTerm
        {
            get
            {
                var i = SolarTermFunc((x, y) => x == y, out var dt);
                return i == -1 ? "" : _solarTerm[i];
            }
        }

        /// <summary>
        /// 上一个节气
        /// </summary>
        public string SolarTermPrev
        {
            get
            {
                var i = SolarTermFunc((x, y) => x < y, out var dt);
                return i == -1 ? "" : _solarTerm[i];
            }
        }

        /// <summary>
        /// 下一个节气
        /// </summary>
        public string SolarTermNext
        {
            get
            {
                var i = SolarTermFunc((x, y) => x > y, out var dt);
                return i == -1 ? "" : $"{_solarTerm[i]}";
            }
        }

        /// <summary>
        /// 节气计算（当前年），返回指定条件的节气序及日期（公历）
        /// </summary>
        /// <param name="func"></param>
        /// <param name="dateTime"></param>
        /// <returns>-1时即没找到</returns>
        private int SolarTermFunc(Expression<Func<int, int, bool>> func, out DateTime dateTime)
        {
            var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            var year = _dateTime.Year;
            int[] solar = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            var expressionType = func.Body.NodeType;
            if (expressionType != ExpressionType.LessThan && expressionType != ExpressionType.LessThanOrEqual &&
                expressionType != ExpressionType.GreaterThan && expressionType != ExpressionType.GreaterThanOrEqual &&
                expressionType != ExpressionType.Equal)
            {
                throw new NotSupportedException("不受支持的操作符");
            }

            if (expressionType == ExpressionType.LessThan || expressionType == ExpressionType.LessThanOrEqual)
            {
                solar = solar.OrderByDescending(x => x).ToArray();
            }
            foreach (var item in solar)
            {
                var num = 525948.76 * (year - 1900) + _solarTermInfo[item - 1];
                var newDate = baseDateAndTime.AddMinutes(num); //按分钟计算
                if (func.Compile()(newDate.DayOfYear, _dateTime.DayOfYear))
                {
                    dateTime = newDate;
                    return item - 1;
                }
            }
            dateTime = _chineseDateTime.MinSupportedDateTime;
            return -1;
        }
        #endregion
    }
}

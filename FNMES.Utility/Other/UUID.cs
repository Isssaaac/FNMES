using System;

namespace FNMES.Utility.Other
{
    public class UUID
    {
        public static long SnowId
        {
            get
            {
                return SnowFlakeHelper.GetSnowInstance().NextId();
            }
        }

        public static string StrSnowId
        {
            get
            {
                return Convert.ToString(SnowId);
            }
        }


        public static string NewTimeUUID
        {
            get
            {
                return DateTime.Today.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 10);
            }
        }

        public static string NewTimeUUID2
        {
            get
            {
                return DateTime.Today.ToString("yyyyMMdd") + Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
        }

        public static string NewUUID
        {
            get
            {
                return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
        }
    }
}

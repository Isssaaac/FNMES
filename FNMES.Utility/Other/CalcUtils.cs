using System;

namespace FNMES.Utility.Other
{
    public class CalcUtils
    {
        /// <summary>
        /// 计算两个坐标的距离
        /// </summary>
        /// <param name="long1">经度1</param>
        /// <param name="lat1">纬度1</param>
        /// <param name="long2">经度2</param>
        /// <param name="lat2">纬度2</param>
        /// <returns></returns>
        public static double getDistance(double long1, double lat1, double long2, double lat2)
        {
            double a, b, R;
            R = 6371393; // 地球半径  
            lat1 = lat1 * Math.PI / 180.0;
            lat2 = lat2 * Math.PI / 180.0;
            a = lat1 - lat2;
            b = (long1 - long2) * Math.PI / 180.0;
            double d;
            double sa2, sb2;
            sa2 = Math.Sin(a / 2.0);
            sb2 = Math.Sin(b / 2.0);
            d = 2 * R * Math.Asin(Math.Sqrt(sa2 * sa2 + Math.Cos(lat1) * Math.Cos(lat2) * sb2 * sb2));
            return d;
        }

        public static double algorithm(double longitude1, double latitude1, double longitude2, double latitude2)
        {
            double Lat1 = rad(latitude1); // 纬度
            double Lat2 = rad(latitude2);
            double a = Lat1 - Lat2;//两点纬度之差
            double b = rad(longitude1) - rad(longitude2); //经度之差
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(Lat1) * Math.Cos(Lat2) * Math.Pow(Math.Sin(b / 2), 2)));//计算两点距离的公式
            s = s * 6378137.0;//弧长乘地球半径（半径为米）
            s = Math.Round(s * 10000d) / 10000d;//精确距离的数值
            return s;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.00; //角度转换成弧度
        }

        public static double getVectorAngle(double x1, double y1, double x2, double y2)
        {
            return Math.Acos((x1 * x2 + y1 * y2) / (Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2))) * 180.0 / Math.PI;
        }
    }
}

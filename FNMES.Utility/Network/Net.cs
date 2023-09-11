#if !NETFRAMEWORK
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
#endif
using FNMES.Utility.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FNMES.Utility.Network
{
    /// <summary>
    /// 网络操作。
    /// </summary>
    public class Net
    {
        #region Ip(获取Ip)
        /// <summary>
        /// 获取IP。
        /// </summary>
        public static string Ip
        {
            get
            {
                var result = string.Empty;
#if !NETFRAMEWORK
                if (MyHttpContext.httpContext != null)
                {
                    result = GetWebClientIp();
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = GetLanIp();
                }
#else
                if (HttpContext.Current != null)
                {
                    result = GetWebClientIp();
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = GetLanIp();
                }
#endif
                return result;
            }
        }

        /// <summary>
        /// 获取Web客户端IP。
        /// </summary>
        private static string GetWebClientIp()
        {
#if NETFRAMEWORK
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            foreach (var hostAddress in Dns.GetHostAddresses(ip))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
#else
            var ip = MyHttpContext.httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = MyHttpContext.httpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
#endif

        }

        /// <summary>
        /// 获取局域网IP。
        /// </summary>
        private static string GetLanIp()
        {
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取IP地址信息（源：淘宝IP库接口）。
        /// </summary>
        /// <param name="ip">IP</param>
        /// <returns></returns>
        public static string GetAddress(string ip)
        {
            try
            {
#if !NETFRAMEWORK
                string path = MyHttpContext.MapPath("/");
                path = path.Replace("\\", "/");
                path += "data/IPAddressDB/17monipdb.dat";
                EnableFileWatch = true;
                Load(path);
                return string.Join(",", Find(ip));
#else
                string path = HttpContext.Current.Server.MapPath("/");
                path = path.Replace("\\", "/");
                path += "App_Data/IPAddressDB/17monipdb.dat";
                EnableFileWatch = true;
                Load(path);
                return string.Join(",", Find(ip));
#endif
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #endregion


        #region Host(获取主机名)
#if NETFRAMEWORK
        /// <summary>
        /// 获取主机名。
        /// </summary>
        public static string Host
        {
            get
            {
                return HttpContext.Current == null ? Dns.GetHostName() : GetWebClientHostName();
            }
        }

        /// <summary>
        /// 获取Web客户端主机名。
        /// </summary>
        private static string GetWebClientHostName()
        {
            if (!HttpContext.Current.Request.IsLocal)
                return string.Empty;
            var ip = GetWebClientIp();
            var result = Dns.GetHostEntry(IPAddress.Parse(ip)).HostName;
            if (result == "localhost.localdomain")
                result = Dns.GetHostName();
            return result;
        }
#endif
        #endregion


        #region 获取mac地址
        /// <summary>
        /// 返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        /// </summary>
        /// <returns></returns>
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }
        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }
        #endregion

        #region Browser(获取浏览器信息)
        /// <summary>
        /// 获取浏览器信息
        /// </summary>
        public static string Browser
        {
            get
            {
#if !NETFRAMEWORK
                HttpContext context = MyHttpContext.httpContext;
                return context.Request.Headers[HeaderNames.UserAgent].ToString();
#else
                if (HttpContext.Current == null)
                    return string.Empty;
                var browser = HttpContext.Current.Request.Browser;
                return string.Format("{0} {1}", browser.Browser, browser.Version);
#endif
            }
        }
        #endregion

        #region 淘宝IP地址库接口模型
        /// <summary>
        /// 淘宝IP地址库接口模型。
        /// http://ip.taobao.com/instructions.php
        /// </summary>
        internal class TaoBaoIpEnitiy
        {
            /// <summary>
            /// 响应结果。
            /// </summary>
            public int code { get; set; }
            /// <summary>
            ///  响应数据。
            /// </summary>
            public IpDataEnitiy data { get; set; }

            public string GetAddress()
            {
                if (this.data == null)
                {
                    return string.Empty;
                }
                return string.Format("{0} {1} {2}", this.data.country, this.data.region, this.data.city);
            }
        }

        internal class IpDataEnitiy
        {
            /// <summary>
            /// 国家
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 国家ID
            /// </summary>
            public string country_id { get; set; }
            /// <summary>
            /// 地区
            /// </summary>
            public string area { get; set; }
            /// <summary>
            /// 地区ID
            /// </summary>
            public string area_id { get; set; }
            /// <summary>
            /// 省份
            /// </summary>
            public string region { get; set; }
            /// <summary>
            /// 省份ID
            /// </summary>
            public string region_id { get; set; }
            /// <summary>
            /// 城市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 城市ID
            /// </summary>
            public string city_id { get; set; }
            /// <summary>
            /// 地区
            /// </summary>
            public string county { get; set; }
            /// <summary>
            /// 地区ID
            /// </summary>
            public string county_id { get; set; }
            /// <summary>
            /// 运营商
            /// </summary>
            public string isp { get; set; }
            /// <summary>
            /// 运营商ID
            /// </summary>
            public string isp_id { get; set; }
            /// <summary>
            /// IP
            /// </summary>
            public string ip { get; set; }
        }
        #endregion

        #region IP地址本地数据查询
        public static bool EnableFileWatch = false;

        private static int offset;
        private static uint[] index = new uint[256];
        private static byte[] dataBuffer;
        private static byte[] indexBuffer;
        private static long lastModifyTime = 0L;
        private static string ipFile;
        private static readonly object @lock = new object();

        public static void Load(string filename)
        {
            ipFile = new FileInfo(filename).FullName;
            Load();
            if (EnableFileWatch)
            {
                Watch();
            }
        }

        public static string[] Find(string ip)
        {
            lock (@lock)
            {
                var ips = ip.Split('.');
                var ip_prefix_value = int.Parse(ips[0]);
                long ip2long_value = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]),
                    byte.Parse(ips[3]));
                var start = index[ip_prefix_value];
                var max_comp_len = offset - 1028;
                long index_offset = -1;
                var index_length = -1;
                byte b = 0;
                for (start = start * 8 + 1024; start < max_comp_len; start += 8)
                {
                    if (
                        BytesToLong(indexBuffer[start + 0], indexBuffer[start + 1], indexBuffer[start + 2],
                            indexBuffer[start + 3]) >= ip2long_value)
                    {
                        index_offset = BytesToLong(b, indexBuffer[start + 6], indexBuffer[start + 5],
                            indexBuffer[start + 4]);
                        index_length = 0xFF & indexBuffer[start + 7];
                        break;
                    }
                }
                var areaBytes = new byte[index_length];
                Array.Copy(dataBuffer, offset + (int)index_offset - 1024, areaBytes, 0, index_length);
                return Encoding.UTF8.GetString(areaBytes).Split('\t');
            }
        }

        private static void Watch()
        {
            var file = new FileInfo(ipFile);
            if (file.DirectoryName == null) return;
            var watcher = new FileSystemWatcher(file.DirectoryName, file.Name) { NotifyFilter = NotifyFilters.LastWrite };
            watcher.Changed += (s, e) =>
            {
                var time = File.GetLastWriteTime(ipFile).Ticks;
                if (time > lastModifyTime)
                {
                    Load();
                }
            };
            watcher.EnableRaisingEvents = true;
        }

        private static void Load()
        {
            lock (@lock)
            {
                var file = new FileInfo(ipFile);
                lastModifyTime = file.LastWriteTime.Ticks;
                try
                {
                    dataBuffer = new byte[file.Length];
                    using (var fin = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        fin.Read(dataBuffer, 0, dataBuffer.Length);
                    }

                    var indexLength = BytesToLong(dataBuffer[0], dataBuffer[1], dataBuffer[2], dataBuffer[3]);
                    indexBuffer = new byte[indexLength];
                    Array.Copy(dataBuffer, 4, indexBuffer, 0, indexLength);
                    offset = (int)indexLength;

                    for (var loop = 0; loop < 256; loop++)
                    {
                        index[loop] = BytesToLong(indexBuffer[loop * 4 + 3], indexBuffer[loop * 4 + 2],
                            indexBuffer[loop * 4 + 1],
                            indexBuffer[loop * 4]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;
        }
        #endregion
    }
}

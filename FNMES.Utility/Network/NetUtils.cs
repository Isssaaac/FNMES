using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{
    public static class NetUtils
    {
        public static bool ping(this string strIpAddress)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(strIpAddress, 200);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

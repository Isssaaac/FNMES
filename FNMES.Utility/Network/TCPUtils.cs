using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{
    public class TCPUtils
    {
        private TcpClient tcpClient;
        private string ipAddress = "127.0.0.1";
        private int port = 502;
        private int connectTimeout = 1000;

        private bool connected = false;

        private NetworkStream stream;

        /// <summary>
        /// 判断是否已经连接
        /// </summary>
        public bool Connected
        {
            get
            {
                if (tcpClient == null)
                {
                    return false;
                }
                return connected;
            }
        }

        public string IPAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return connectTimeout;
            }
            set
            {
                connectTimeout = value;
            }
        }

        public TCPUtils() { }
        public TCPUtils(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public void Connect()
        {
            tcpClient = new TcpClient();
            IAsyncResult asyncResult = tcpClient.BeginConnect(ipAddress, port, null, null);
            if (!asyncResult.AsyncWaitHandle.WaitOne(connectTimeout))
            {
                throw new Exception("connection timed out");
            }
            tcpClient.EndConnect(asyncResult);
            stream = tcpClient.GetStream();
            stream.ReadTimeout = connectTimeout;
            connected = true;
        }

        public void Connect(string ipAddress, int port)
        {
            tcpClient = new TcpClient();
            IAsyncResult asyncResult = tcpClient.BeginConnect(ipAddress, port, null, null);
            if (!asyncResult.AsyncWaitHandle.WaitOne(connectTimeout))
            {
                throw new Exception("connection timed out");
            }
            tcpClient.EndConnect(asyncResult);
            stream = tcpClient.GetStream();
            stream.ReadTimeout = connectTimeout;
            connected = true;
        }

        public byte[] Send(byte[] sendData)
        {
            if (tcpClient == null)
            {
                throw new Exception("connection error");
            }
            if (!tcpClient.Client.Connected)
            {
                throw new Exception("connection error");
            }
            //连接成功
            stream.Write(sendData, 0, sendData.Length);
            byte[] revData = new byte[256];
            int num = stream.Read(revData, 0, revData.Length);
            return revData.Take(num).ToArray();
        }

        public void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
            connected = false;
        }

        ~TCPUtils()
        {
            if (tcpClient != null)
            {
                if (stream != null)
                {
                    stream.Close();
                }
                tcpClient.Close();
            }
        }

        public bool Available(int timeout)
        {
            Ping ping = new Ping();
            IPAddress address = System.Net.IPAddress.Parse(ipAddress);
            string s = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            PingReply pingReply = ping.Send(address, timeout, bytes);
            if (pingReply.Status == IPStatus.Success)
            {
                return true;
            }
            return false;
        }
    }
}

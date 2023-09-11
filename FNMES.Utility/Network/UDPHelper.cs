using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{
    public class UDPHelper
    {
        private bool isStarted = false;
        private bool stop = false;
        private string message;
        private Socket server;
        public delegate void OnMessageReceive(byte[] msg);
        public event OnMessageReceive OnMessageReceived;
        private int port;
        public UDPHelper(int port)
        {
            this.port = port;
        }
        public UDPHelper(string port)
        {
            this.port = Convert.ToInt32(port);
        }
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            isStarted = true;
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                server.Bind(new IPEndPoint(IPAddress.Any, port));//绑定端口号和IP
                new Thread(ReceiveMsg).Start();
            }
            catch
            {
                Stop();
                isStarted = false;
            }
        }


        private void ReceiveMsg()
        {
            while (!stop)
            {
                try
                {
                    //用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);
                    int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                    OnMessageReceived?.Invoke(buffer.Take(length).ToArray());
                }
                catch (Exception ex)
                {

                }
            }
        }


        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            try
            {
                if (!isStarted)
                {
                    return;
                }
                stop = true;
                server.Close();
            }
            catch { }
        }


        public void SendMsg(string ip, int port, byte[] bytes)
        {
            if (!isStarted)
            {
                return;
            }
            message = string.Empty;
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            server.SendTo(bytes, point);
        }

        public void SendMsg(string ip, int port, List<byte> bytes)
        {
            SendMsg(ip, port, bytes.ToArray());
        }

        public void SendMsg(string ip, int port, string message, Encoding encoding)
        {
            if (!isStarted)
            {
                return;
            }
            message = string.Empty;
            EndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            server.SendTo(encoding.GetBytes(message), point);
        }

        public void SendMsg(string ip, int port, string msg)
        {
            SendMsg(ip, port, msg, Encoding.UTF8);
        }
    }
}

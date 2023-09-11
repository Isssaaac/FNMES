using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EasyModbus
{
	internal class TCPHandler
	{
		public delegate void DataChanged(object networkConnectionParameter);

		public delegate void NumberOfClientsChanged();

		internal class Client
		{
			private readonly TcpClient tcpClient;

			private readonly byte[] buffer;

			public long Ticks
			{
				get;
				set;
			}

			public TcpClient TcpClient => tcpClient;

			public byte[] Buffer => buffer;

			public NetworkStream NetworkStream => tcpClient.GetStream();

			public Client(TcpClient tcpClient)
			{
				this.tcpClient = tcpClient;
				int receiveBufferSize = tcpClient.ReceiveBufferSize;
				buffer = new byte[receiveBufferSize];
			}
		}

		private TcpListener server = null;

		private List<Client> tcpClientLastRequestList = new List<Client>();

		public string ipAddress = null;

		public int NumberOfConnectedClients
		{
			get;
			set;
		}

		public event DataChanged dataChanged;

		public event NumberOfClientsChanged numberOfClientsChanged;

		public TCPHandler(int port)
		{
			IPAddress any = IPAddress.Any;
			server = new TcpListener(any, port);
			server.Start();
			server.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
		}

		public TCPHandler(string ipAddress, int port)
		{
			this.ipAddress = ipAddress;
			IPAddress any = IPAddress.Any;
			server = new TcpListener(any, port);
			server.Start();
			server.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
		}

		private void AcceptTcpClientCallback(IAsyncResult asyncResult)
		{
			TcpClient tcpClient = new TcpClient();
			try
			{
				tcpClient = server.EndAcceptTcpClient(asyncResult);
				tcpClient.ReceiveTimeout = 4000;
				if (ipAddress != null)
				{
					string text = tcpClient.Client.RemoteEndPoint.ToString();
					text = text.Split(':')[0];
					if (text != ipAddress)
					{
						tcpClient.Client.Disconnect(reuseSocket: false);
						return;
					}
				}
			}
			catch (Exception)
			{
			}
			try
			{
				server.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
				Client client = new Client(tcpClient);
				NetworkStream networkStream = client.NetworkStream;
				networkStream.ReadTimeout = 4000;
				networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
			}
			catch (Exception)
			{
			}
		}

		private int GetAndCleanNumberOfConnectedClients(Client client)
		{
			lock (this)
			{
				bool flag = false;
				foreach (Client tcpClientLastRequest in tcpClientLastRequestList)
				{
					if (client.Equals(tcpClientLastRequest))
					{
						flag = true;
					}
				}
				try
				{
					tcpClientLastRequestList.RemoveAll((Client c) => checked(DateTime.Now.Ticks - c.Ticks) > 40000000);
				}
				catch (Exception)
				{
				}
				if (!flag)
				{
					tcpClientLastRequestList.Add(client);
				}
				return tcpClientLastRequestList.Count;
			}
		}

		private void ReadCallback(IAsyncResult asyncResult)
		{
			NetworkConnectionParameter networkConnectionParameter = default(NetworkConnectionParameter);
			Client client = asyncResult.AsyncState as Client;
			client.Ticks = DateTime.Now.Ticks;
			NumberOfConnectedClients = GetAndCleanNumberOfConnectedClients(client);
			if (this.numberOfClientsChanged != null)
			{
				this.numberOfClientsChanged();
			}
			if (client == null)
			{
				return;
			}
			NetworkStream networkStream = null;
			int num;
			try
			{
				networkStream = client.NetworkStream;
				num = networkStream.EndRead(asyncResult);
			}
			catch (Exception)
			{
				return;
			}
			if (num != 0)
			{
				byte[] array = new byte[num];
				Buffer.BlockCopy(client.Buffer, 0, array, 0, num);
				networkConnectionParameter.bytes = array;
				networkConnectionParameter.stream = networkStream;
				if (this.dataChanged != null)
				{
					this.dataChanged(networkConnectionParameter);
				}
				try
				{
					networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, ReadCallback, client);
				}
				catch (Exception)
				{
				}
			}
		}

		public void Disconnect()
		{
			try
			{
				foreach (Client tcpClientLastRequest in tcpClientLastRequestList)
				{
					tcpClientLastRequest.NetworkStream.Close(0);
				}
			}
			catch (Exception)
			{
			}
			server.Stop();
		}
	}
}

using EasyModbus.Exceptions;
using System;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EasyModbus
{
	public class ModbusClient
	{
		public enum RegisterOrder
		{
			LowHigh,
			HighLow
		}

		public delegate void ReceiveDataChangedHandler(object sender);

		public delegate void SendDataChangedHandler(object sender);

		public delegate void ConnectedChangedHandler(object sender);

		private bool debug = false;

		private TcpClient tcpClient;

		private string ipAddress = "127.0.0.1";

		private int port = 502;

		private uint transactionIdentifierInternal = 0u;

		private byte[] transactionIdentifier = new byte[2];

		private byte[] protocolIdentifier = new byte[2];

		private byte[] crc = new byte[2];

		private byte[] length = new byte[2];

		private byte unitIdentifier = 1;

		private byte functionCode;

		private byte[] startingAddress = new byte[2];

		private byte[] quantity = new byte[2];

		private bool udpFlag = false;

		private int portOut;

		private int baudRate = 9600;

		private int connectTimeout = 1000;

		public byte[] receiveData;

		public byte[] sendData;

		private SerialPort serialport;

		private Parity parity = Parity.Even;

		private StopBits stopBits = StopBits.One;

		private bool connected = false;

		private int countRetries = 0;

		private NetworkStream stream;

		private bool dataReceived = false;

		private bool receiveActive = false;

		private byte[] readBuffer = new byte[256];

		private int bytesToRead = 0;

		private int akjjjctualPositionToRead = 0;

		private DateTime dateTimeLastRead;

		public int NumberOfRetries
		{
			get;
			set;
		} = 3;


		public bool Connected
		{
			get
			{
				if (serialport != null)
				{
					return serialport.IsOpen;
				}
				if (udpFlag & (tcpClient != null))
				{
					return true;
				}
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

		public bool UDPFlag
		{
			get
			{
				return udpFlag;
			}
			set
			{
				udpFlag = value;
			}
		}

		public byte UnitIdentifier
		{
			get
			{
				return unitIdentifier;
			}
			set
			{
				unitIdentifier = value;
			}
		}

		public int Baudrate
		{
			get
			{
				return baudRate;
			}
			set
			{
				baudRate = value;
			}
		}

		public Parity Parity
		{
			get
			{
				if (serialport != null)
				{
					return parity;
				}
				return Parity.Even;
			}
			set
			{
				if (serialport != null)
				{
					parity = value;
				}
			}
		}

		public StopBits StopBits
		{
			get
			{
				if (serialport != null)
				{
					return stopBits;
				}
				return StopBits.One;
			}
			set
			{
				if (serialport != null)
				{
					stopBits = value;
				}
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

		public string SerialPort
		{
			get
			{
				return serialport.PortName;
			}
			set
			{
				if (value == null)
				{
					serialport = null;
					return;
				}
				if (serialport != null)
				{
					serialport.Close();
				}
				serialport = new SerialPort();
				serialport.PortName = value;
				serialport.BaudRate = baudRate;
				serialport.Parity = parity;
				serialport.StopBits = stopBits;
				serialport.WriteTimeout = 10000;
				serialport.ReadTimeout = connectTimeout;
				serialport.DataReceived += DataReceivedHandler;
			}
		}

		public string LogFileFilename
		{
			get
			{
				return StoreLogData.Instance.Filename;
			}
			set
			{
				StoreLogData.Instance.Filename = value;
				if (StoreLogData.Instance.Filename != null)
				{
					debug = true;
				}
				else
				{
					debug = false;
				}
			}
		}

		public event ReceiveDataChangedHandler ReceiveDataChanged;

		public event SendDataChangedHandler SendDataChanged;

		public event ConnectedChangedHandler ConnectedChanged;

		public ModbusClient(string ipAddress, int port)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-TCP, IPAddress: " + ipAddress + ", Port: " + port, DateTime.Now);
			}
			Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
			Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
			Console.WriteLine();
			this.ipAddress = ipAddress;
			this.port = port;
		}

		public ModbusClient(string serialPort)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-RTU, COM-Port: " + serialPort, DateTime.Now);
			}
			Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
			Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
			Console.WriteLine();
			serialport = new SerialPort();
			serialport.PortName = serialPort;
			serialport.BaudRate = baudRate;
			serialport.Parity = parity;
			serialport.StopBits = stopBits;
			serialport.WriteTimeout = 10000;
			serialport.ReadTimeout = connectTimeout;
			serialport.DataReceived += DataReceivedHandler;
		}

		public ModbusClient()
		{
			if (debug)
			{
				StoreLogData.Instance.Store("EasyModbus library initialized for Modbus-TCP", DateTime.Now);
			}
			Console.WriteLine("EasyModbus Client Library Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
			Console.WriteLine("Copyright (c) Stefan Rossmann Engineering Solutions");
			Console.WriteLine();
		}

		public void Connect()
		{
			if (serialport != null)
			{
				if (!serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("Open Serial port " + serialport.PortName, DateTime.Now);
					}
					serialport.BaudRate = baudRate;
					serialport.Parity = parity;
					serialport.StopBits = stopBits;
					serialport.WriteTimeout = 10000;
					serialport.ReadTimeout = connectTimeout;
					serialport.Open();
					connected = true;
				}
				if (this.ConnectedChanged != null)
				{
					try
					{
						this.ConnectedChanged(this);
					}
					catch
					{
					}
				}
				return;
			}
			if (!udpFlag)
			{
				if (debug)
				{
					StoreLogData.Instance.Store("Open TCP-Socket, IP-Address: " + ipAddress + ", Port: " + port, DateTime.Now);
				}
				tcpClient = new TcpClient();
				IAsyncResult asyncResult = tcpClient.BeginConnect(ipAddress, port, null, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(connectTimeout))
				{
					throw new ConnectionException("connection timed out");
				}
				tcpClient.EndConnect(asyncResult);
				stream = tcpClient.GetStream();
				stream.ReadTimeout = connectTimeout;
				connected = true;
			}
			else
			{
				tcpClient = new TcpClient();
				connected = true;
			}
			if (this.ConnectedChanged != null)
			{
				try
				{
					this.ConnectedChanged(this);
				}
				catch
				{
				}
			}
		}

		public void Connect(string ipAddress, int port)
		{
			if (!udpFlag)
			{
				if (debug)
				{
					StoreLogData.Instance.Store("Open TCP-Socket, IP-Address: " + ipAddress + ", Port: " + port, DateTime.Now);
				}
				tcpClient = new TcpClient();
				IAsyncResult asyncResult = tcpClient.BeginConnect(ipAddress, port, null, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(connectTimeout))
				{
					throw new ConnectionException("connection timed out");
				}
				tcpClient.EndConnect(asyncResult);
				stream = tcpClient.GetStream();
				stream.ReadTimeout = connectTimeout;
				connected = true;
			}
			else
			{
				tcpClient = new TcpClient();
				connected = true;
			}
			if (this.ConnectedChanged != null)
			{
				this.ConnectedChanged(this);
			}
		}

		public static float ConvertRegistersToFloat(int[] registers)
		{
			if (registers.Length != 2)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '2'");
			}
			int value = registers[1];
			int value2 = registers[0];
			byte[] bytes = BitConverter.GetBytes(value);
			byte[] bytes2 = BitConverter.GetBytes(value2);
			byte[] value3 = new byte[4]
			{
				bytes2[0],
				bytes2[1],
				bytes[0],
				bytes[1]
			};
			return BitConverter.ToSingle(value3, 0);
		}

		public static float ConvertRegistersToFloat(int[] registers, RegisterOrder registerOrder)
		{
			int[] registers2 = new int[2]
			{
				registers[0],
				registers[1]
			};
			if (registerOrder == RegisterOrder.HighLow)
			{
				registers2 = new int[2]
				{
					registers[1],
					registers[0]
				};
			}
			return ConvertRegistersToFloat(registers2);
		}

		public static int ConvertRegistersToInt(int[] registers)
		{
			if (registers.Length != 2)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '2'");
			}
			int value = registers[1];
			int value2 = registers[0];
			byte[] bytes = BitConverter.GetBytes(value);
			byte[] bytes2 = BitConverter.GetBytes(value2);
			byte[] value3 = new byte[4]
			{
				bytes2[0],
				bytes2[1],
				bytes[0],
				bytes[1]
			};
			return BitConverter.ToInt32(value3, 0);
		}

		public static int ConvertRegistersToInt(int[] registers, RegisterOrder registerOrder)
		{
			int[] registers2 = new int[2]
			{
				registers[0],
				registers[1]
			};
			if (registerOrder == RegisterOrder.HighLow)
			{
				registers2 = new int[2]
				{
					registers[1],
					registers[0]
				};
			}
			return ConvertRegistersToInt(registers2);
		}

		public static long ConvertRegistersToLong(int[] registers)
		{
			if (registers.Length != 4)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '4'");
			}
			int value = registers[3];
			int value2 = registers[2];
			int value3 = registers[1];
			int value4 = registers[0];
			byte[] bytes = BitConverter.GetBytes(value);
			byte[] bytes2 = BitConverter.GetBytes(value2);
			byte[] bytes3 = BitConverter.GetBytes(value3);
			byte[] bytes4 = BitConverter.GetBytes(value4);
			byte[] value5 = new byte[8]
			{
				bytes4[0],
				bytes4[1],
				bytes3[0],
				bytes3[1],
				bytes2[0],
				bytes2[1],
				bytes[0],
				bytes[1]
			};
			return BitConverter.ToInt64(value5, 0);
		}

		public static long ConvertRegistersToLong(int[] registers, RegisterOrder registerOrder)
		{
			if (registers.Length != 4)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '4'");
			}
			int[] registers2 = new int[4]
			{
				registers[0],
				registers[1],
				registers[2],
				registers[3]
			};
			if (registerOrder == RegisterOrder.HighLow)
			{
				registers2 = new int[4]
				{
					registers[3],
					registers[2],
					registers[1],
					registers[0]
				};
			}
			return ConvertRegistersToLong(registers2);
		}

		public static double ConvertRegistersToDouble(int[] registers)
		{
			if (registers.Length != 4)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '4'");
			}
			int value = registers[3];
			int value2 = registers[2];
			int value3 = registers[1];
			int value4 = registers[0];
			byte[] bytes = BitConverter.GetBytes(value);
			byte[] bytes2 = BitConverter.GetBytes(value2);
			byte[] bytes3 = BitConverter.GetBytes(value3);
			byte[] bytes4 = BitConverter.GetBytes(value4);
			byte[] value5 = new byte[8]
			{
				bytes4[0],
				bytes4[1],
				bytes3[0],
				bytes3[1],
				bytes2[0],
				bytes2[1],
				bytes[0],
				bytes[1]
			};
			return BitConverter.ToDouble(value5, 0);
		}

		public static double ConvertRegistersToDouble(int[] registers, RegisterOrder registerOrder)
		{
			if (registers.Length != 4)
			{
				throw new ArgumentException("Input Array length invalid - Array langth must be '4'");
			}
			int[] registers2 = new int[4]
			{
				registers[0],
				registers[1],
				registers[2],
				registers[3]
			};
			if (registerOrder == RegisterOrder.HighLow)
			{
				registers2 = new int[4]
				{
					registers[3],
					registers[2],
					registers[1],
					registers[0]
				};
			}
			return ConvertRegistersToDouble(registers2);
		}

		public static int[] ConvertFloatToRegisters(float floatValue)
		{
			byte[] bytes = BitConverter.GetBytes(floatValue);
			byte[] value = new byte[4]
			{
				bytes[2],
				bytes[3],
				0,
				0
			};
			byte[] value2 = new byte[4]
			{
				bytes[0],
				bytes[1],
				0,
				0
			};
			return new int[2]
			{
				BitConverter.ToInt32(value2, 0),
				BitConverter.ToInt32(value, 0)
			};
		}

		public static int[] ConvertFloatToRegisters(float floatValue, RegisterOrder registerOrder)
		{
			int[] array = ConvertFloatToRegisters(floatValue);
			int[] result = array;
			if (registerOrder == RegisterOrder.HighLow)
			{
				result = new int[2]
				{
					array[1],
					array[0]
				};
			}
			return result;
		}

		public static int[] ConvertIntToRegisters(int intValue)
		{
			byte[] bytes = BitConverter.GetBytes(intValue);
			byte[] value = new byte[4]
			{
				bytes[2],
				bytes[3],
				0,
				0
			};
			byte[] value2 = new byte[4]
			{
				bytes[0],
				bytes[1],
				0,
				0
			};
			return new int[2]
			{
				BitConverter.ToInt32(value2, 0),
				BitConverter.ToInt32(value, 0)
			};
		}

		public static int[] ConvertIntToRegisters(int intValue, RegisterOrder registerOrder)
		{
			int[] array = ConvertIntToRegisters(intValue);
			int[] result = array;
			if (registerOrder == RegisterOrder.HighLow)
			{
				result = new int[2]
				{
					array[1],
					array[0]
				};
			}
			return result;
		}

		public static int[] ConvertLongToRegisters(long longValue)
		{
			byte[] bytes = BitConverter.GetBytes(longValue);
			byte[] value = new byte[4]
			{
				bytes[6],
				bytes[7],
				0,
				0
			};
			byte[] value2 = new byte[4]
			{
				bytes[4],
				bytes[5],
				0,
				0
			};
			byte[] value3 = new byte[4]
			{
				bytes[2],
				bytes[3],
				0,
				0
			};
			byte[] value4 = new byte[4]
			{
				bytes[0],
				bytes[1],
				0,
				0
			};
			return new int[4]
			{
				BitConverter.ToInt32(value4, 0),
				BitConverter.ToInt32(value3, 0),
				BitConverter.ToInt32(value2, 0),
				BitConverter.ToInt32(value, 0)
			};
		}

		public static int[] ConvertLongToRegisters(long longValue, RegisterOrder registerOrder)
		{
			int[] array = ConvertLongToRegisters(longValue);
			int[] result = array;
			if (registerOrder == RegisterOrder.HighLow)
			{
				result = new int[4]
				{
					array[3],
					array[2],
					array[1],
					array[0]
				};
			}
			return result;
		}

		public static int[] ConvertDoubleToRegisters(double doubleValue)
		{
			byte[] bytes = BitConverter.GetBytes(doubleValue);
			byte[] value = new byte[4]
			{
				bytes[6],
				bytes[7],
				0,
				0
			};
			byte[] value2 = new byte[4]
			{
				bytes[4],
				bytes[5],
				0,
				0
			};
			byte[] value3 = new byte[4]
			{
				bytes[2],
				bytes[3],
				0,
				0
			};
			byte[] value4 = new byte[4]
			{
				bytes[0],
				bytes[1],
				0,
				0
			};
			return new int[4]
			{
				BitConverter.ToInt32(value4, 0),
				BitConverter.ToInt32(value3, 0),
				BitConverter.ToInt32(value2, 0),
				BitConverter.ToInt32(value, 0)
			};
		}

		public static int[] ConvertDoubleToRegisters(double doubleValue, RegisterOrder registerOrder)
		{
			int[] array = ConvertDoubleToRegisters(doubleValue);
			int[] result = array;
			if (registerOrder == RegisterOrder.HighLow)
			{
				result = new int[4]
				{
					array[3],
					array[2],
					array[1],
					array[0]
				};
			}
			return result;
		}

		public static string ConvertRegistersToString(int[] registers, int offset, int stringLength)
		{
			byte[] array = new byte[stringLength];
			byte[] array2 = new byte[2];
			checked
			{
				for (int i = 0; i < unchecked(stringLength / 2); i++)
				{
					array2 = BitConverter.GetBytes(registers[offset + i]);
					array[i * 2] = array2[0];
					array[i * 2 + 1] = array2[1];
				}
				return Encoding.Default.GetString(array);
			}
		}

		public static int[] ConvertStringToRegisters(string stringToConvert)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(stringToConvert);
			checked
			{
				int[] array = new int[unchecked(stringToConvert.Length / 2) + unchecked(stringToConvert.Length % 2)];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = bytes[i * 2];
					if (i * 2 + 1 < bytes.Length)
					{
						array[i] |= bytes[i * 2 + 1] << 8;
					}
				}
				return array;
			}
		}

		public static ushort calculateCRC(byte[] data, ushort numberOfBytes, int startByte)
		{
			byte[] array = new byte[256]
			{
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64,
				1,
				192,
				128,
				65,
				1,
				192,
				128,
				65,
				0,
				193,
				129,
				64
			};
			byte[] array2 = new byte[256]
			{
				0,
				192,
				193,
				1,
				195,
				3,
				2,
				194,
				198,
				6,
				7,
				199,
				5,
				197,
				196,
				4,
				204,
				12,
				13,
				205,
				15,
				207,
				206,
				14,
				10,
				202,
				203,
				11,
				201,
				9,
				8,
				200,
				216,
				24,
				25,
				217,
				27,
				219,
				218,
				26,
				30,
				222,
				223,
				31,
				221,
				29,
				28,
				220,
				20,
				212,
				213,
				21,
				215,
				23,
				22,
				214,
				210,
				18,
				19,
				211,
				17,
				209,
				208,
				16,
				240,
				48,
				49,
				241,
				51,
				243,
				242,
				50,
				54,
				246,
				247,
				55,
				245,
				53,
				52,
				244,
				60,
				252,
				253,
				61,
				255,
				63,
				62,
				254,
				250,
				58,
				59,
				251,
				57,
				249,
				248,
				56,
				40,
				232,
				233,
				41,
				235,
				43,
				42,
				234,
				238,
				46,
				47,
				239,
				45,
				237,
				236,
				44,
				228,
				36,
				37,
				229,
				39,
				231,
				230,
				38,
				34,
				226,
				227,
				35,
				225,
				33,
				32,
				224,
				160,
				96,
				97,
				161,
				99,
				163,
				162,
				98,
				102,
				166,
				167,
				103,
				165,
				101,
				100,
				164,
				108,
				172,
				173,
				109,
				175,
				111,
				110,
				174,
				170,
				106,
				107,
				171,
				105,
				169,
				168,
				104,
				120,
				184,
				185,
				121,
				187,
				123,
				122,
				186,
				190,
				126,
				127,
				191,
				125,
				189,
				188,
				124,
				180,
				116,
				117,
				181,
				119,
				183,
				182,
				118,
				114,
				178,
				179,
				115,
				177,
				113,
				112,
				176,
				80,
				144,
				145,
				81,
				147,
				83,
				82,
				146,
				150,
				86,
				87,
				151,
				85,
				149,
				148,
				84,
				156,
				92,
				93,
				157,
				95,
				159,
				158,
				94,
				90,
				154,
				155,
				91,
				153,
				89,
				88,
				152,
				136,
				72,
				73,
				137,
				75,
				139,
				138,
				74,
				78,
				142,
				143,
				79,
				141,
				77,
				76,
				140,
				68,
				132,
				133,
				69,
				135,
				71,
				70,
				134,
				130,
				66,
				67,
				131,
				65,
				129,
				128,
				64
			};
			ushort num = numberOfBytes;
			byte b = byte.MaxValue;
			byte b2 = byte.MaxValue;
			int num2 = 0;
			checked
			{
				while (num > 0)
				{
					num = (ushort)(unchecked((uint)num) - 1u);
					if (num2 + startByte < data.Length)
					{
						int num3 = b2 ^ data[num2 + startByte];
						b2 = (byte)(b ^ array[num3]);
						b = array2[num3];
					}
					num2++;
				}
				return (ushort)((b << 8) | b2);
			}
		}

		private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
		{
			serialport.DataReceived -= DataReceivedHandler;
			receiveActive = true;
			SerialPort serialPort = (SerialPort)sender;
			if (bytesToRead == 0)
			{
				serialPort.DiscardInBuffer();
				receiveActive = false;
				serialport.DataReceived += DataReceivedHandler;
				return;
			}
			readBuffer = new byte[256];
			int num = 0;
			int num2 = 0;
			DateTime now = DateTime.Now;
			checked
			{
				do
				{
					try
					{
						now = DateTime.Now;
						while (serialPort.BytesToRead == 0)
						{
							Thread.Sleep(10);
							if (DateTime.Now.Ticks - now.Ticks > 20000000)
							{
								break;
							}
						}
						num = serialPort.BytesToRead;
						byte[] array = new byte[num];
						serialPort.Read(array, 0, num);
						Array.Copy(array, 0, readBuffer, num2, (num2 + array.Length <= bytesToRead) ? array.Length : (bytesToRead - num2));
						num2 += array.Length;
					}
					catch (Exception)
					{
					}
				}
				while (bytesToRead > num2 && !(DetectValidModbusFrame(readBuffer, (num2 < readBuffer.Length) ? num2 : readBuffer.Length) | (bytesToRead <= num2)) && DateTime.Now.Ticks - now.Ticks < 20000000);
				receiveData = new byte[num2];
				Array.Copy(readBuffer, 0, receiveData, 0, (num2 < readBuffer.Length) ? num2 : readBuffer.Length);
				if (debug)
				{
					StoreLogData.Instance.Store("Received Serial-Data: " + BitConverter.ToString(readBuffer), DateTime.Now);
				}
				bytesToRead = 0;
				dataReceived = true;
				receiveActive = false;
				serialport.DataReceived += DataReceivedHandler;
				if (this.ReceiveDataChanged != null)
				{
					this.ReceiveDataChanged(this);
				}
			}
		}

		public static bool DetectValidModbusFrame(byte[] readBuffer, int length)
		{
			if (length < 6)
			{
				return false;
			}
			if ((readBuffer[0] < 1) | (readBuffer[0] > 247))
			{
				return false;
			}
			byte[] array = new byte[2];
			checked
			{
				array = BitConverter.GetBytes(calculateCRC(readBuffer, (ushort)(length - 2), 0));
				if ((array[0] != readBuffer[length - 2]) | (array[1] != readBuffer[length - 1]))
				{
					return false;
				}
				return true;
			}
		}

		public bool[] ReadDiscreteInputs(int startingAddress, int quantity)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC2 (Read Discrete Inputs from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				if ((startingAddress > 65535) | (quantity > 2000))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ArgumentException Throwed", DateTime.Now);
					}
					throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 2;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				this.quantity = BitConverter.GetBytes(quantity);
				byte[] array = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					this.quantity[1],
					this.quantity[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				array[12] = crc[0];
				array[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					if (unchecked(quantity % 8) == 0)
					{
						bytesToRead = 5 + unchecked(quantity / 8);
					}
					else
					{
						bytesToRead = 6 + unchecked(quantity / 8);
					}
					serialport.Write(array, 6, 8);
					if (debug)
					{
						byte[] array2 = new byte[8];
						Array.Copy(array, 6, array2, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b = array[6];
					}
					if (b != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[2100];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 130) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 130) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 130) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 130) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport != null)
				{
					crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(unchecked((int)array[8]) + 3), 6));
					if (((crc[0] != array[unchecked((int)array[8]) + 9]) | (crc[1] != array[unchecked((int)array[8]) + 10])) & dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new CRCCheckFailedException("Response CRC check failed");
						}
						countRetries++;
						return ReadDiscreteInputs(startingAddress, quantity);
					}
					if (!dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new TimeoutException("No Response from Modbus Slave");
						}
						countRetries++;
						return ReadDiscreteInputs(startingAddress, quantity);
					}
				}
				bool[] array4 = new bool[quantity];
				for (int i = 0; i < quantity; i++)
				{
					int num2 = array[9 + unchecked(i / 8)];
					unchecked
					{
						int num3 = Convert.ToInt32(Math.Pow(2.0, i % 8));
						array4[i] = Convert.ToBoolean((num2 & num3) / num3);
					}
				}
				return array4;
			}
		}

		public bool[] ReadCoils(int startingAddress, int quantity)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC1 (Read Coils from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				if ((startingAddress > 65535) | (quantity > 2000))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ArgumentException Throwed", DateTime.Now);
					}
					throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 1;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				this.quantity = BitConverter.GetBytes(quantity);
				byte[] array = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					this.quantity[1],
					this.quantity[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				array[12] = crc[0];
				array[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					if (unchecked(quantity % 8) == 0)
					{
						bytesToRead = 5 + unchecked(quantity / 8);
					}
					else
					{
						bytesToRead = 6 + unchecked(quantity / 8);
					}
					serialport.Write(array, 6, 8);
					if (debug)
					{
						byte[] array2 = new byte[8];
						Array.Copy(array, 6, array2, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b = array[6];
					}
					if (b != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send MocbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[2100];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 129) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 129) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 129) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 129) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport != null)
				{
					crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(unchecked((int)array[8]) + 3), 6));
					if (((crc[0] != array[unchecked((int)array[8]) + 9]) | (crc[1] != array[unchecked((int)array[8]) + 10])) & dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new CRCCheckFailedException("Response CRC check failed");
						}
						countRetries++;
						return ReadCoils(startingAddress, quantity);
					}
					if (!dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new TimeoutException("No Response from Modbus Slave");
						}
						countRetries++;
						return ReadCoils(startingAddress, quantity);
					}
				}
				bool[] array4 = new bool[quantity];
				for (int i = 0; i < quantity; i++)
				{
					int num2 = array[9 + unchecked(i / 8)];
					unchecked
					{
						int num3 = Convert.ToInt32(Math.Pow(2.0, i % 8));
						array4[i] = Convert.ToBoolean((num2 & num3) / num3);
					}
				}
				return array4;
			}
		}

		public int[] ReadHoldingRegisters(int startingAddress, int quantity)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC3 (Read Holding Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				if ((startingAddress > 65535) | (quantity > 125))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ArgumentException Throwed", DateTime.Now);
					}
					throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 3;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				this.quantity = BitConverter.GetBytes(quantity);
				byte[] array = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					this.quantity[1],
					this.quantity[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				array[12] = crc[0];
				array[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 5 + 2 * quantity;
					serialport.Write(array, 6, 8);
					if (debug)
					{
						byte[] array2 = new byte[8];
						Array.Copy(array, 6, array2, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b = array[6];
					}
					if (b != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[256];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 131) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 131) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 131) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 131) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport != null)
				{
					crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(unchecked((int)array[8]) + 3), 6));
					if (((crc[0] != array[unchecked((int)array[8]) + 9]) | (crc[1] != array[unchecked((int)array[8]) + 10])) & dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new CRCCheckFailedException("Response CRC check failed");
						}
						countRetries++;
						return ReadHoldingRegisters(startingAddress, quantity);
					}
					if (!dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new TimeoutException("No Response from Modbus Slave");
						}
						countRetries++;
						return ReadHoldingRegisters(startingAddress, quantity);
					}
				}
				int[] array4 = new int[quantity];
				for (int i = 0; i < quantity; i++)
				{
					byte b2 = array[9 + i * 2];
					byte b3 = array[9 + i * 2 + 1];
					array[9 + i * 2] = b3;
					array[9 + i * 2 + 1] = b2;
					array4[i] = BitConverter.ToInt16(array, 9 + i * 2);
				}
				return array4;
			}
		}

		public int[] ReadInputRegisters(int startingAddress, int quantity)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC4 (Read Input Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				if ((startingAddress > 65535) | (quantity > 125))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ArgumentException Throwed", DateTime.Now);
					}
					throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 4;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				this.quantity = BitConverter.GetBytes(quantity);
				byte[] array = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					this.quantity[1],
					this.quantity[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				array[12] = crc[0];
				array[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 5 + 2 * quantity;
					serialport.Write(array, 6, 8);
					if (debug)
					{
						byte[] array2 = new byte[8];
						Array.Copy(array, 6, array2, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b = array[6];
					}
					if (b != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[2100];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 132) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 132) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 132) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 132) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport != null)
				{
					crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(unchecked((int)array[8]) + 3), 6));
					if (((crc[0] != array[unchecked((int)array[8]) + 9]) | (crc[1] != array[unchecked((int)array[8]) + 10])) & dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new CRCCheckFailedException("Response CRC check failed");
						}
						countRetries++;
						return ReadInputRegisters(startingAddress, quantity);
					}
					if (!dataReceived)
					{
						if (debug)
						{
							StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
						}
						if (NumberOfRetries <= countRetries)
						{
							countRetries = 0;
							throw new TimeoutException("No Response from Modbus Slave");
						}
						countRetries++;
						return ReadInputRegisters(startingAddress, quantity);
					}
				}
				int[] array4 = new int[quantity];
				for (int i = 0; i < quantity; i++)
				{
					byte b2 = array[9 + i * 2];
					byte b3 = array[9 + i * 2 + 1];
					array[9 + i * 2] = b3;
					array[9 + i * 2 + 1] = b2;
					array4[i] = BitConverter.ToInt16(array, 9 + i * 2);
				}
				return array4;
			}
		}

		public void WriteSingleCoil(int startingAddress, bool value)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC5 (Write single coil to Master device), StartingAddress: " + startingAddress + ", Value: " + value.ToString(), DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				byte[] array = new byte[2];
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 5;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				array = ((!value) ? BitConverter.GetBytes(0) : BitConverter.GetBytes(65280));
				byte[] array2 = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					array[1],
					array[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array2, 6, 6));
				array2[12] = crc[0];
				array2[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 8;
					serialport.Write(array2, 6, 8);
					if (debug)
					{
						byte[] array3 = new byte[8];
						Array.Copy(array2, 6, array3, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array3), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array2, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array2 = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array2 = new byte[2100];
						Array.Copy(readBuffer, 0, array2, 6, readBuffer.Length);
						b = array2[6];
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array2, array2.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array2 = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array2, 0, array2.Length - 2);
						if (debug)
						{
							byte[] array4 = new byte[array2.Length - 2];
							Array.Copy(array2, 0, array4, 0, array2.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array4), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array2.Length - 2];
							Array.Copy(array2, 0, sendData, 0, array2.Length - 2);
							this.SendDataChanged(this);
						}
						array2 = new byte[2100];
						int num = stream.Read(array2, 0, array2.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array2, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array2[7] == 133) & (array2[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array2[7] == 133) & (array2[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array2[7] == 133) & (array2[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array2[7] == 133) & (array2[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport == null)
				{
					return;
				}
				crc = BitConverter.GetBytes(calculateCRC(array2, 6, 6));
				if (((crc[0] != array2[12]) | (crc[1] != array2[13])) & dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new CRCCheckFailedException("Response CRC check failed");
					}
					countRetries++;
					WriteSingleCoil(startingAddress, value);
				}
				else if (!dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new TimeoutException("No Response from Modbus Slave");
					}
					countRetries++;
					WriteSingleCoil(startingAddress, value);
				}
			}
		}

		public void WriteSingleRegister(int startingAddress, int value)
		{
			if (debug)
			{
				StoreLogData.Instance.Store("FC6 (Write single register to Master device), StartingAddress: " + startingAddress + ", Value: " + value, DateTime.Now);
			}
			checked
			{
				transactionIdentifierInternal++;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				byte[] array = new byte[2];
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(6);
				functionCode = 6;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				array = BitConverter.GetBytes(value);
				byte[] array2 = new byte[14]
				{
					transactionIdentifier[1],
					transactionIdentifier[0],
					protocolIdentifier[1],
					protocolIdentifier[0],
					length[1],
					length[0],
					unitIdentifier,
					functionCode,
					this.startingAddress[1],
					this.startingAddress[0],
					array[1],
					array[0],
					crc[0],
					crc[1]
				};
				crc = BitConverter.GetBytes(calculateCRC(array2, 6, 6));
				array2[12] = crc[0];
				array2[13] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 8;
					serialport.Write(array2, 6, 8);
					if (debug)
					{
						byte[] array3 = new byte[8];
						Array.Copy(array2, 6, array3, 0, 8);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array3), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[8];
						Array.Copy(array2, 6, sendData, 0, 8);
						this.SendDataChanged(this);
					}
					array2 = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b = byte.MaxValue;
					while ((b != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array2 = new byte[2100];
						Array.Copy(readBuffer, 0, array2, 6, readBuffer.Length);
						b = array2[6];
					}
					if (b != unitIdentifier)
					{
						array2 = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array2, array2.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array2 = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array2, 0, array2.Length - 2);
						if (debug)
						{
							byte[] array4 = new byte[array2.Length - 2];
							Array.Copy(array2, 0, array4, 0, array2.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array4), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array2.Length - 2];
							Array.Copy(array2, 0, sendData, 0, array2.Length - 2);
							this.SendDataChanged(this);
						}
						array2 = new byte[2100];
						int num = stream.Read(array2, 0, array2.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array2, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array2[7] == 134) & (array2[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array2[7] == 134) & (array2[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array2[7] == 134) & (array2[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array2[7] == 134) & (array2[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport == null)
				{
					return;
				}
				crc = BitConverter.GetBytes(calculateCRC(array2, 6, 6));
				if (((crc[0] != array2[12]) | (crc[1] != array2[13])) & dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new CRCCheckFailedException("Response CRC check failed");
					}
					countRetries++;
					WriteSingleRegister(startingAddress, value);
				}
				else if (!dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new TimeoutException("No Response from Modbus Slave");
					}
					countRetries++;
					WriteSingleRegister(startingAddress, value);
				}
			}
		}

		public void WriteMultipleCoils(int startingAddress, bool[] values)
		{
			string text = "";
			checked
			{
				for (int i = 0; i < values.Length; i++)
				{
					text = text + values[i].ToString() + " ";
				}
				if (debug)
				{
					StoreLogData.Instance.Store("FC15 (Write multiple coils to Master device), StartingAddress: " + startingAddress + ", Values: " + text, DateTime.Now);
				}
				transactionIdentifierInternal++;
				byte b = (byte)((unchecked(values.Length % 8) != 0) ? (unchecked(values.Length / 8) + 1) : unchecked(values.Length / 8));
				byte[] bytes = BitConverter.GetBytes(values.Length);
				byte b2 = 0;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(7 + unchecked((int)b));
				functionCode = 15;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				byte[] array = new byte[16 + ((unchecked(values.Length % 8) != 0) ? unchecked(values.Length / 8) : (unchecked(values.Length / 8) - 1))];
				array[0] = transactionIdentifier[1];
				array[1] = transactionIdentifier[0];
				array[2] = protocolIdentifier[1];
				array[3] = protocolIdentifier[0];
				array[4] = length[1];
				array[5] = length[0];
				array[6] = unitIdentifier;
				array[7] = functionCode;
				array[8] = this.startingAddress[1];
				array[9] = this.startingAddress[0];
				array[10] = bytes[1];
				array[11] = bytes[0];
				array[12] = b;
				for (int j = 0; j < values.Length; j++)
				{
					byte b3;
					unchecked
					{
						if (j % 8 == 0)
						{
							b2 = 0;
						}
						b3 = (byte)(values[j] ? 1 : 0);
					}
					b2 = (byte)((b3 << unchecked(j % 8)) | b2);
					array[13 + unchecked(j / 8)] = b2;
				}
				crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(array.Length - 8), 6));
				array[array.Length - 2] = crc[0];
				array[array.Length - 1] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 8;
					serialport.Write(array, 6, array.Length - 6);
					if (debug)
					{
						byte[] array2 = new byte[array.Length - 6];
						Array.Copy(array, 6, array2, 0, array.Length - 6);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[array.Length - 6];
						Array.Copy(array, 6, sendData, 0, array.Length - 6);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b4 = byte.MaxValue;
					while ((b4 != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b4 = array[6];
					}
					if (b4 != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[2100];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 143) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 143) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 143) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 143) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport == null)
				{
					return;
				}
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				if (((crc[0] != array[12]) | (crc[1] != array[13])) & dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new CRCCheckFailedException("Response CRC check failed");
					}
					countRetries++;
					WriteMultipleCoils(startingAddress, values);
				}
				else if (!dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new TimeoutException("No Response from Modbus Slave");
					}
					countRetries++;
					WriteMultipleCoils(startingAddress, values);
				}
			}
		}

		public void WriteMultipleRegisters(int startingAddress, int[] values)
		{
			string text = "";
			checked
			{
				for (int i = 0; i < values.Length; i++)
				{
					text = text + values[i] + " ";
				}
				if (debug)
				{
					StoreLogData.Instance.Store("FC16 (Write multiple Registers to Server device), StartingAddress: " + startingAddress + ", Values: " + text, DateTime.Now);
				}
				transactionIdentifierInternal++;
				byte b = (byte)(values.Length * 2);
				byte[] bytes = BitConverter.GetBytes(values.Length);
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(7 + values.Length * 2);
				functionCode = 16;
				this.startingAddress = BitConverter.GetBytes(startingAddress);
				byte[] array = new byte[15 + values.Length * 2];
				array[0] = transactionIdentifier[1];
				array[1] = transactionIdentifier[0];
				array[2] = protocolIdentifier[1];
				array[3] = protocolIdentifier[0];
				array[4] = length[1];
				array[5] = length[0];
				array[6] = unitIdentifier;
				array[7] = functionCode;
				array[8] = this.startingAddress[1];
				array[9] = this.startingAddress[0];
				array[10] = bytes[1];
				array[11] = bytes[0];
				array[12] = b;
				for (int j = 0; j < values.Length; j++)
				{
					byte[] bytes2 = BitConverter.GetBytes(values[j]);
					array[13 + j * 2] = bytes2[1];
					array[14 + j * 2] = bytes2[0];
				}
				crc = BitConverter.GetBytes(calculateCRC(array, (ushort)(array.Length - 8), 6));
				array[array.Length - 2] = crc[0];
				array[array.Length - 1] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 8;
					serialport.Write(array, 6, array.Length - 6);
					if (debug)
					{
						byte[] array2 = new byte[array.Length - 6];
						Array.Copy(array, 6, array2, 0, array.Length - 6);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array2), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[array.Length - 6];
						Array.Copy(array, 6, sendData, 0, array.Length - 6);
						this.SendDataChanged(this);
					}
					array = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b2 = byte.MaxValue;
					while ((b2 != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array = new byte[2100];
						Array.Copy(readBuffer, 0, array, 6, readBuffer.Length);
						b2 = array[6];
					}
					if (b2 != unitIdentifier)
					{
						array = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array, array.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array, 0, array.Length - 2);
						if (debug)
						{
							byte[] array3 = new byte[array.Length - 2];
							Array.Copy(array, 0, array3, 0, array.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array3), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array.Length - 2];
							Array.Copy(array, 0, sendData, 0, array.Length - 2);
							this.SendDataChanged(this);
						}
						array = new byte[2100];
						int num = stream.Read(array, 0, array.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array[7] == 144) & (array[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array[7] == 144) & (array[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array[7] == 144) & (array[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array[7] == 144) & (array[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				if (serialport == null)
				{
					return;
				}
				crc = BitConverter.GetBytes(calculateCRC(array, 6, 6));
				if (((crc[0] != array[12]) | (crc[1] != array[13])) & dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("CRCCheckFailedException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new CRCCheckFailedException("Response CRC check failed");
					}
					countRetries++;
					WriteMultipleRegisters(startingAddress, values);
				}
				else if (!dataReceived)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("TimeoutException Throwed", DateTime.Now);
					}
					if (NumberOfRetries <= countRetries)
					{
						countRetries = 0;
						throw new TimeoutException("No Response from Modbus Slave");
					}
					countRetries++;
					WriteMultipleRegisters(startingAddress, values);
				}
			}
		}

		public int[] ReadWriteMultipleRegisters(int startingAddressRead, int quantityRead, int startingAddressWrite, int[] values)
		{
			string text = "";
			checked
			{
				for (int i = 0; i < values.Length; i++)
				{
					text = text + values[i] + " ";
				}
				if (debug)
				{
					StoreLogData.Instance.Store("FC23 (Read and Write multiple Registers to Server device), StartingAddress Read: " + startingAddressRead + ", Quantity Read: " + quantityRead + ", startingAddressWrite: " + startingAddressWrite + ", Values: " + text, DateTime.Now);
				}
				transactionIdentifierInternal++;
				byte[] array = new byte[2];
				byte[] array2 = new byte[2];
				byte[] array3 = new byte[2];
				byte[] array4 = new byte[2];
				byte b = 0;
				if (serialport != null && !serialport.IsOpen)
				{
					if (debug)
					{
						StoreLogData.Instance.Store("SerialPortNotOpenedException Throwed", DateTime.Now);
					}
					throw new SerialPortNotOpenedException("serial port not opened");
				}
				if ((tcpClient == null) & !udpFlag & (serialport == null))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ConnectionException Throwed", DateTime.Now);
					}
					throw new ConnectionException("connection error");
				}
				if ((startingAddressRead > 65535) | (quantityRead > 125) | (startingAddressWrite > 65535) | (values.Length > 121))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ArgumentException Throwed", DateTime.Now);
					}
					throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
				}
				transactionIdentifier = BitConverter.GetBytes(transactionIdentifierInternal);
				protocolIdentifier = BitConverter.GetBytes(0);
				length = BitConverter.GetBytes(11 + values.Length * 2);
				functionCode = 23;
				array = BitConverter.GetBytes(startingAddressRead);
				array2 = BitConverter.GetBytes(quantityRead);
				array3 = BitConverter.GetBytes(startingAddressWrite);
				array4 = BitConverter.GetBytes(values.Length);
				b = Convert.ToByte(values.Length * 2);
				byte[] array5 = new byte[19 + values.Length * 2];
				array5[0] = transactionIdentifier[1];
				array5[1] = transactionIdentifier[0];
				array5[2] = protocolIdentifier[1];
				array5[3] = protocolIdentifier[0];
				array5[4] = length[1];
				array5[5] = length[0];
				array5[6] = unitIdentifier;
				array5[7] = functionCode;
				array5[8] = array[1];
				array5[9] = array[0];
				array5[10] = array2[1];
				array5[11] = array2[0];
				array5[12] = array3[1];
				array5[13] = array3[0];
				array5[14] = array4[1];
				array5[15] = array4[0];
				array5[16] = b;
				for (int j = 0; j < values.Length; j++)
				{
					byte[] bytes = BitConverter.GetBytes(values[j]);
					array5[17 + j * 2] = bytes[1];
					array5[18 + j * 2] = bytes[0];
				}
				crc = BitConverter.GetBytes(calculateCRC(array5, (ushort)(array5.Length - 8), 6));
				array5[array5.Length - 2] = crc[0];
				array5[array5.Length - 1] = crc[1];
				if (serialport != null)
				{
					dataReceived = false;
					bytesToRead = 5 + 2 * quantityRead;
					serialport.Write(array5, 6, array5.Length - 6);
					if (debug)
					{
						byte[] array6 = new byte[array5.Length - 6];
						Array.Copy(array5, 6, array6, 0, array5.Length - 6);
						if (debug)
						{
							StoreLogData.Instance.Store("Send Serial-Data: " + BitConverter.ToString(array6), DateTime.Now);
						}
					}
					if (this.SendDataChanged != null)
					{
						sendData = new byte[array5.Length - 6];
						Array.Copy(array5, 6, sendData, 0, array5.Length - 6);
						this.SendDataChanged(this);
					}
					array5 = new byte[2100];
					readBuffer = new byte[256];
					DateTime now = DateTime.Now;
					byte b2 = byte.MaxValue;
					while ((b2 != unitIdentifier) & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
					{
						while (!dataReceived & (DateTime.Now.Ticks - now.Ticks <= 10000L * unchecked((long)connectTimeout)))
						{
							Thread.Sleep(1);
						}
						array5 = new byte[2100];
						Array.Copy(readBuffer, 0, array5, 6, readBuffer.Length);
						b2 = array5[6];
					}
					if (b2 != unitIdentifier)
					{
						array5 = new byte[2100];
					}
					else
					{
						countRetries = 0;
					}
				}
				else if (tcpClient.Client.Connected | udpFlag)
				{
					if (udpFlag)
					{
						UdpClient udpClient = new UdpClient();
						IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
						udpClient.Send(array5, array5.Length - 2, endPoint);
						portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
						udpClient.Client.ReceiveTimeout = 5000;
						endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
						array5 = udpClient.Receive(ref endPoint);
					}
					else
					{
						stream.Write(array5, 0, array5.Length - 2);
						if (debug)
						{
							byte[] array7 = new byte[array5.Length - 2];
							Array.Copy(array5, 0, array7, 0, array5.Length - 2);
							if (debug)
							{
								StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(array7), DateTime.Now);
							}
						}
						if (this.SendDataChanged != null)
						{
							sendData = new byte[array5.Length - 2];
							Array.Copy(array5, 0, sendData, 0, array5.Length - 2);
							this.SendDataChanged(this);
						}
						array5 = new byte[2100];
						int num = stream.Read(array5, 0, array5.Length);
						if (this.ReceiveDataChanged != null)
						{
							receiveData = new byte[num];
							Array.Copy(array5, 0, receiveData, 0, num);
							if (debug)
							{
								StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), DateTime.Now);
							}
							this.ReceiveDataChanged(this);
						}
					}
				}
				if ((array5[7] == 151) & (array5[8] == 1))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", DateTime.Now);
					}
					throw new FunctionCodeNotSupportedException("Function code not supported by master");
				}
				if ((array5[7] == 151) & (array5[8] == 2))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", DateTime.Now);
					}
					throw new StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
				}
				if ((array5[7] == 151) & (array5[8] == 3))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("QuantityInvalidException Throwed", DateTime.Now);
					}
					throw new QuantityInvalidException("quantity invalid");
				}
				if ((array5[7] == 151) & (array5[8] == 4))
				{
					if (debug)
					{
						StoreLogData.Instance.Store("ModbusException Throwed", DateTime.Now);
					}
					throw new ModbusException("error reading");
				}
				int[] array8 = new int[quantityRead];
				for (int k = 0; k < quantityRead; k++)
				{
					byte b3 = array5[9 + k * 2];
					byte b4 = array5[9 + k * 2 + 1];
					array5[9 + k * 2] = b4;
					array5[9 + k * 2 + 1] = b3;
					array8[k] = BitConverter.ToInt16(array5, 9 + k * 2);
				}
				return array8;
			}
		}

		public void Disconnect()
		{
			if (debug)
			{
				StoreLogData.Instance.Store("Disconnect", DateTime.Now);
			}
			if (serialport != null)
			{
				if (serialport.IsOpen & !receiveActive)
				{
					serialport.Close();
				}
				if (this.ConnectedChanged != null)
				{
					this.ConnectedChanged(this);
				}
				return;
			}
			if (stream != null)
			{
				stream.Close();
			}
			if (tcpClient != null)
			{
				tcpClient.Close();
			}
			connected = false;
			if (this.ConnectedChanged != null)
			{
				this.ConnectedChanged(this);
			}
		}

		~ModbusClient()
		{
			if (debug)
			{
				StoreLogData.Instance.Store("Destructor called - automatically disconnect", DateTime.Now);
			}
			if (serialport != null)
			{
				if (serialport.IsOpen)
				{
					serialport.Close();
				}
			}
			else
			{
				if ((tcpClient != null) & !udpFlag)
				{
					if (stream != null)
					{
						stream.Close();
					}
					tcpClient.Close();
				}
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

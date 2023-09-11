using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
	public class ConnectionException : ModbusException
	{
		public ConnectionException()
		{
		}

		public ConnectionException(string message)
			: base(message)
		{
		}

		public ConnectionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ConnectionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

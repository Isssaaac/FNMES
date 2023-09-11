using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
	public class SerialPortNotOpenedException : ModbusException
	{
		public SerialPortNotOpenedException()
		{
		}

		public SerialPortNotOpenedException(string message)
			: base(message)
		{
		}

		public SerialPortNotOpenedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SerialPortNotOpenedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

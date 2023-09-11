using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
	public class QuantityInvalidException : ModbusException
	{
		public QuantityInvalidException()
		{
		}

		public QuantityInvalidException(string message)
			: base(message)
		{
		}

		public QuantityInvalidException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected QuantityInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

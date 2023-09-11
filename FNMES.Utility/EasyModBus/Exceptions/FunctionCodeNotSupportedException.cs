using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
	public class FunctionCodeNotSupportedException : ModbusException
	{
		public FunctionCodeNotSupportedException()
		{
		}

		public FunctionCodeNotSupportedException(string message)
			: base(message)
		{
		}

		public FunctionCodeNotSupportedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected FunctionCodeNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

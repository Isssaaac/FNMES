using System;
using System.Runtime.Serialization;

namespace EasyModbus.Exceptions
{
	public class CRCCheckFailedException : ModbusException
	{
		public CRCCheckFailedException()
		{
		}

		public CRCCheckFailedException(string message)
			: base(message)
		{
		}

		public CRCCheckFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected CRCCheckFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

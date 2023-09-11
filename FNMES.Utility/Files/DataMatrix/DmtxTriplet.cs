using System;

namespace DataMatrix.net
{
	// Token: 0x02000007 RID: 7
	internal struct DmtxTriplet
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000033C4 File Offset: 0x000015C4
		internal byte[] Value
		{
			get
			{
				byte[] result;
				if ((result = this._value) == null)
				{
					result = (this._value = new byte[3]);
				}
				return result;
			}
		}

		// Token: 0x04000036 RID: 54
		private byte[] _value;
	}
}

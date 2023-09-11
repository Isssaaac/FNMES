using System;

namespace DataMatrix.net
{
	// Token: 0x02000008 RID: 8
	internal struct DmtxQuadruplet
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000033F0 File Offset: 0x000015F0
		internal byte[] Value
		{
			get
			{
				byte[] result;
				if ((result = this._value) == null)
				{
					result = (this._value = new byte[4]);
				}
				return result;
			}
		}

		// Token: 0x04000037 RID: 55
		private byte[] _value;
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000019 RID: 25
	internal class DmtxChannelGroup
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00005164 File Offset: 0x00003364
		internal DmtxChannel[] Channels
		{
			get
			{
				if (this._channels == null)
				{
					this._channels = new DmtxChannel[6];
					for (int i = 0; i < 6; i++)
					{
						this._channels[i] = new DmtxChannel();
					}
				}
				return this._channels;
			}
		}

		// Token: 0x04000101 RID: 257
		private DmtxChannel[] _channels;
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x0200001E RID: 30
	internal class DmtxImage
	{
		// Token: 0x06000155 RID: 341 RVA: 0x0000CB98 File Offset: 0x0000AD98
		internal DmtxImage(byte[] pxl, int width, int height, DmtxPackOrder pack)
		{
			this.BitsPerChannel = new int[4];
			this.ChannelStart = new int[4];
			if (pxl == null || width < 1 || height < 1)
			{
				throw new ArgumentException("Cannot create image of size null");
			}
			this.Pxl = pxl;
			this.Width = width;
			this.Height = height;
			this.PixelPacking = pack;
			this.BitsPerPixel = DmtxCommon.GetBitsPerPixel(pack);
			this.BytesPerPixel = this.BitsPerPixel / 8;
			this._rowPadBytes = 0;
			this.RowSizeBytes = this.Width * this.BytesPerPixel + this._rowPadBytes;
			this.ImageFlip = DmtxFlip.DmtxFlipNone;
			this.ChannelCount = 0;
			if (pack <= DmtxPackOrder.DmtxPack8bppK)
			{
				if (pack == DmtxPackOrder.DmtxPackCustom)
				{
					return;
				}
				if (pack == DmtxPackOrder.DmtxPack1bppK)
				{
					throw new ArgumentException("Cannot create image: not supported pack order!");
				}
				if (pack == DmtxPackOrder.DmtxPack8bppK)
				{
					this.SetChannel(0, 8);
					return;
				}
			}
			else
			{
				switch (pack)
				{
				case DmtxPackOrder.DmtxPack16bppRGB:
				case DmtxPackOrder.DmtxPack16bppBGR:
				case DmtxPackOrder.DmtxPack16bppYCbCr:
					this.SetChannel(0, 5);
					this.SetChannel(5, 5);
					this.SetChannel(10, 5);
					return;
				case DmtxPackOrder.DmtxPack16bppRGBX:
				case DmtxPackOrder.DmtxPack16bppBGRX:
					this.SetChannel(0, 5);
					this.SetChannel(5, 5);
					this.SetChannel(10, 5);
					return;
				case DmtxPackOrder.DmtxPack16bppXRGB:
				case DmtxPackOrder.DmtxPack16bppXBGR:
					this.SetChannel(1, 5);
					this.SetChannel(6, 5);
					this.SetChannel(11, 5);
					return;
				default:
					switch (pack)
					{
					case DmtxPackOrder.DmtxPack24bppRGB:
					case DmtxPackOrder.DmtxPack24bppBGR:
					case DmtxPackOrder.DmtxPack24bppYCbCr:
						break;
					default:
						switch (pack)
						{
						case DmtxPackOrder.DmtxPack32bppRGBX:
						case DmtxPackOrder.DmtxPack32bppBGRX:
							break;
						case DmtxPackOrder.DmtxPack32bppXRGB:
						case DmtxPackOrder.DmtxPack32bppXBGR:
							this.SetChannel(8, 8);
							this.SetChannel(16, 8);
							this.SetChannel(24, 8);
							return;
						case DmtxPackOrder.DmtxPack32bppCMYK:
							this.SetChannel(0, 8);
							this.SetChannel(8, 8);
							this.SetChannel(16, 8);
							this.SetChannel(24, 8);
							return;
						default:
							goto IL_220;
						}
						break;
					}
					this.SetChannel(0, 8);
					this.SetChannel(8, 8);
					this.SetChannel(16, 8);
					return;
				}
			}
			IL_220:
			throw new ArgumentException("Cannot create image: Invalid Pack Order");
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000CDD4 File Offset: 0x0000AFD4
		internal bool SetChannel(int channelStart, int bitsPerChannel)
		{
			bool result;
			if (this.ChannelCount >= 4)
			{
				result = false;
			}
			else
			{
				this.BitsPerChannel[this.ChannelCount] = bitsPerChannel;
				this.ChannelStart[this.ChannelCount] = channelStart;
				this.ChannelCount++;
				result = true;
			}
			return result;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000CE24 File Offset: 0x0000B024
		internal int GetByteOffset(int x, int y)
		{
			if (this.ImageFlip == DmtxFlip.DmtxFlipX)
			{
				throw new ArgumentException("DmtxFlipX is not an option!");
			}
			int result;
			if (!this.ContainsInt(0, x, y))
			{
				result = DmtxConstants.DmtxUndefined;
			}
			else if (this.ImageFlip == DmtxFlip.DmtxFlipY)
			{
				result = y * this.RowSizeBytes + x * this.BytesPerPixel;
			}
			else
			{
				result = (this.Height - y - 1) * this.RowSizeBytes + x * this.BytesPerPixel;
			}
			return result;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		internal bool GetPixelValue(int x, int y, int channel, ref int value)
		{
			if (channel >= this.ChannelCount)
			{
				throw new ArgumentException("Channel greater than channel count!");
			}
			int byteOffset = this.GetByteOffset(x, y);
			bool result;
			if (byteOffset == DmtxConstants.DmtxUndefined)
			{
				result = false;
			}
			else
			{
				int num = this.BitsPerChannel[channel];
				if (num != 1)
				{
					if (num != 5)
					{
						if (num == 8)
						{
							if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
							{
								throw new Exception("Error getting pixel value");
							}
							value = (int)this.Pxl[byteOffset + channel];
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000CF48 File Offset: 0x0000B148
		internal bool SetPixelValue(int x, int y, int channel, byte value)
		{
			if (channel >= this.ChannelCount)
			{
				throw new ArgumentException("Channel greater than channel count!");
			}
			int byteOffset = this.GetByteOffset(x, y);
			bool result;
			if (byteOffset == DmtxConstants.DmtxUndefined)
			{
				result = false;
			}
			else
			{
				int num = this.BitsPerChannel[channel];
				if (num != 1)
				{
					if (num != 5)
					{
						if (num == 8)
						{
							if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
							{
								throw new Exception("Error getting pixel value");
							}
							this.Pxl[byteOffset + channel] = value;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000CFE4 File Offset: 0x0000B1E4
		internal bool ContainsInt(int margin, int x, int y)
		{
			return x - margin >= 0 && x + margin < this.Width && y - margin >= 0 && y + margin < this.Height;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000D028 File Offset: 0x0000B228
		internal bool ContainsFloat(double x, double y)
		{
			return x >= 0.0 && x < (double)this.Width && y >= 0.0 && y < (double)this.Height;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0000D078 File Offset: 0x0000B278
		// (set) Token: 0x0600015D RID: 349 RVA: 0x0000D08F File Offset: 0x0000B28F
		internal int Width { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600015E RID: 350 RVA: 0x0000D098 File Offset: 0x0000B298
		// (set) Token: 0x0600015F RID: 351 RVA: 0x0000D0AF File Offset: 0x0000B2AF
		internal int Height { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000160 RID: 352 RVA: 0x0000D0B8 File Offset: 0x0000B2B8
		// (set) Token: 0x06000161 RID: 353 RVA: 0x0000D0CF File Offset: 0x0000B2CF
		internal DmtxPackOrder PixelPacking { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000D0D8 File Offset: 0x0000B2D8
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0000D0EF File Offset: 0x0000B2EF
		internal int BitsPerPixel { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000D0F8 File Offset: 0x0000B2F8
		// (set) Token: 0x06000165 RID: 357 RVA: 0x0000D10F File Offset: 0x0000B30F
		internal int BytesPerPixel { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000D118 File Offset: 0x0000B318
		// (set) Token: 0x06000167 RID: 359 RVA: 0x0000D130 File Offset: 0x0000B330
		internal int RowPadBytes
		{
			get
			{
				return this._rowPadBytes;
			}
			set
			{
				this._rowPadBytes = value;
				this.RowSizeBytes = this.Width * (this.BitsPerPixel / 8) + this._rowPadBytes;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000D158 File Offset: 0x0000B358
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000D16F File Offset: 0x0000B36F
		internal int RowSizeBytes { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600016A RID: 362 RVA: 0x0000D178 File Offset: 0x0000B378
		// (set) Token: 0x0600016B RID: 363 RVA: 0x0000D18F File Offset: 0x0000B38F
		internal DmtxFlip ImageFlip { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600016C RID: 364 RVA: 0x0000D198 File Offset: 0x0000B398
		// (set) Token: 0x0600016D RID: 365 RVA: 0x0000D1AF File Offset: 0x0000B3AF
		internal int ChannelCount { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600016E RID: 366 RVA: 0x0000D1B8 File Offset: 0x0000B3B8
		// (set) Token: 0x0600016F RID: 367 RVA: 0x0000D1CF File Offset: 0x0000B3CF
		internal int[] ChannelStart { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000D1D8 File Offset: 0x0000B3D8
		// (set) Token: 0x06000171 RID: 369 RVA: 0x0000D1EF File Offset: 0x0000B3EF
		internal int[] BitsPerChannel { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000172 RID: 370 RVA: 0x0000D1F8 File Offset: 0x0000B3F8
		// (set) Token: 0x06000173 RID: 371 RVA: 0x0000D20F File Offset: 0x0000B40F
		internal byte[] Pxl { get; set; }

		// Token: 0x04000121 RID: 289
		private int _rowPadBytes;
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000016 RID: 22
	internal struct DmtxScanGrid
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00004914 File Offset: 0x00002B14
		internal DmtxScanGrid(DmtxDecode dec)
		{
			int scanGap = dec.ScanGap;
			this._xMin = dec.XMin;
			this._xMax = dec.XMax;
			this._yMin = dec.YMin;
			this._yMax = dec.YMax;
			int num = this._xMax - this._xMin;
			int num2 = this._yMax - this._yMin;
			int num3 = (num > num2) ? num : num2;
			if (num3 < 1)
			{
				throw new ArgumentException("Invalid max extent for Scan Grid: Must be greater than 0");
			}
			int i = 1;
			this._minExtent = i;
			while (i < num3)
			{
				if (i <= scanGap)
				{
					this._minExtent = i;
				}
				i = (i + 1) * 2 - 1;
			}
			this._maxExtent = i;
			this._xOffset = (this._xMin + this._xMax - this._maxExtent) / 2;
			this._yOffset = (this._yMin + this._yMax - this._maxExtent) / 2;
			this._total = 1;
			this._extent = this._maxExtent;
			this._jumpSize = this._extent + 1;
			this._pixelTotal = 2 * this._extent - 1;
			this._startPos = this._extent / 2;
			this._pixelCount = 0;
			this._xCenter = (this._yCenter = this._startPos);
			this.SetDerivedFields();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004A74 File Offset: 0x00002C74
		internal DmtxRange PopGridLocation(ref DmtxPixelLoc loc)
		{
			DmtxRange gridCoordinates;
			do
			{
				gridCoordinates = this.GetGridCoordinates(ref loc);
				this._pixelCount++;
			}
			while (gridCoordinates == DmtxRange.DmtxRangeBad);
			return gridCoordinates;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004AA8 File Offset: 0x00002CA8
		private DmtxRange GetGridCoordinates(ref DmtxPixelLoc locRef)
		{
			if (this._pixelCount >= this._pixelTotal)
			{
				this._pixelCount = 0;
				this._xCenter += this._jumpSize;
			}
			if (this._xCenter > this._maxExtent)
			{
				this._xCenter = this._startPos;
				this._yCenter += this._jumpSize;
			}
			if (this._yCenter > this._maxExtent)
			{
				this._total *= 4;
				this._extent /= 2;
				this.SetDerivedFields();
			}
			DmtxRange result;
			if (this._extent == 0 || this._extent < this._minExtent)
			{
				locRef.X = (locRef.Y = -1);
				result = DmtxRange.DmtxRangeEnd;
			}
			else
			{
				int num = this._pixelCount;
				if (num >= this._pixelTotal)
				{
					throw new Exception("Scangrid is beyong image limits!");
				}
				DmtxPixelLoc dmtxPixelLoc = default(DmtxPixelLoc);
				if (num == this._pixelTotal - 1)
				{
					dmtxPixelLoc.X = this._xCenter;
					dmtxPixelLoc.Y = this._yCenter;
				}
				else
				{
					int num2 = this._pixelTotal / 2;
					int num3 = num2 / 2;
					if (num < num2)
					{
						dmtxPixelLoc.X = this._xCenter + ((num < num3) ? (num - num3) : (num2 - num));
						dmtxPixelLoc.Y = this._yCenter;
					}
					else
					{
						num -= num2;
						dmtxPixelLoc.X = this._xCenter;
						dmtxPixelLoc.Y = this._yCenter + ((num < num3) ? (num - num3) : (num2 - num));
					}
				}
				dmtxPixelLoc.X += this._xOffset;
				dmtxPixelLoc.Y += this._yOffset;
				locRef.X = dmtxPixelLoc.X;
				locRef.Y = dmtxPixelLoc.Y;
				if (dmtxPixelLoc.X < this._xMin || dmtxPixelLoc.X > this._xMax || dmtxPixelLoc.Y < this._yMin || dmtxPixelLoc.Y > this._yMax)
				{
					result = DmtxRange.DmtxRangeBad;
				}
				else
				{
					result = DmtxRange.DmtxRangeGood;
				}
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004D0C File Offset: 0x00002F0C
		private void SetDerivedFields()
		{
			this._jumpSize = this._extent + 1;
			this._pixelTotal = 2 * this._extent - 1;
			this._startPos = this._extent / 2;
			this._pixelCount = 0;
			this._xCenter = (this._yCenter = this._startPos);
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00004D64 File Offset: 0x00002F64
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00004D7C File Offset: 0x00002F7C
		internal int MinExtent
		{
			get
			{
				return this._minExtent;
			}
			set
			{
				this._minExtent = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00004D88 File Offset: 0x00002F88
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00004DA0 File Offset: 0x00002FA0
		internal int MaxExtent
		{
			get
			{
				return this._maxExtent;
			}
			set
			{
				this._maxExtent = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00004DAC File Offset: 0x00002FAC
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00004DC4 File Offset: 0x00002FC4
		internal int XOffset
		{
			get
			{
				return this._xOffset;
			}
			set
			{
				this._xOffset = value;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00004DD0 File Offset: 0x00002FD0
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00004DE8 File Offset: 0x00002FE8
		internal int YOffset
		{
			get
			{
				return this._yOffset;
			}
			set
			{
				this._yOffset = value;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004DF4 File Offset: 0x00002FF4
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00004E0C File Offset: 0x0000300C
		internal int XMin
		{
			get
			{
				return this._xMin;
			}
			set
			{
				this._xMin = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00004E18 File Offset: 0x00003018
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00004E30 File Offset: 0x00003030
		internal int XMax
		{
			get
			{
				return this._xMax;
			}
			set
			{
				this._xMax = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00004E3C File Offset: 0x0000303C
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00004E54 File Offset: 0x00003054
		internal int YMin
		{
			get
			{
				return this._yMin;
			}
			set
			{
				this._yMin = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00004E60 File Offset: 0x00003060
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00004E78 File Offset: 0x00003078
		internal int YMax
		{
			get
			{
				return this._yMax;
			}
			set
			{
				this._yMax = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00004E84 File Offset: 0x00003084
		// (set) Token: 0x06000092 RID: 146 RVA: 0x00004E9C File Offset: 0x0000309C
		internal int Total
		{
			get
			{
				return this._total;
			}
			set
			{
				this._total = value;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00004EA8 File Offset: 0x000030A8
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00004EC0 File Offset: 0x000030C0
		internal int Extent
		{
			get
			{
				return this._extent;
			}
			set
			{
				this._extent = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00004ECC File Offset: 0x000030CC
		// (set) Token: 0x06000096 RID: 150 RVA: 0x00004EE4 File Offset: 0x000030E4
		internal int JumpSize
		{
			get
			{
				return this._jumpSize;
			}
			set
			{
				this._jumpSize = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00004EF0 File Offset: 0x000030F0
		// (set) Token: 0x06000098 RID: 152 RVA: 0x00004F08 File Offset: 0x00003108
		internal int PixelTotal
		{
			get
			{
				return this._pixelTotal;
			}
			set
			{
				this._pixelTotal = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00004F14 File Offset: 0x00003114
		// (set) Token: 0x0600009A RID: 154 RVA: 0x00004F2C File Offset: 0x0000312C
		internal int StartPos
		{
			get
			{
				return this._startPos;
			}
			set
			{
				this._startPos = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00004F38 File Offset: 0x00003138
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00004F50 File Offset: 0x00003150
		internal int PixelCount
		{
			get
			{
				return this._pixelCount;
			}
			set
			{
				this._pixelCount = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00004F5C File Offset: 0x0000315C
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00004F74 File Offset: 0x00003174
		internal int XCenter
		{
			get
			{
				return this._xCenter;
			}
			set
			{
				this._xCenter = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004F80 File Offset: 0x00003180
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00004F98 File Offset: 0x00003198
		internal int YCenter
		{
			get
			{
				return this._yCenter;
			}
			set
			{
				this._yCenter = value;
			}
		}

		// Token: 0x040000E4 RID: 228
		private int _minExtent;

		// Token: 0x040000E5 RID: 229
		private int _maxExtent;

		// Token: 0x040000E6 RID: 230
		private int _xOffset;

		// Token: 0x040000E7 RID: 231
		private int _yOffset;

		// Token: 0x040000E8 RID: 232
		private int _xMin;

		// Token: 0x040000E9 RID: 233
		private int _xMax;

		// Token: 0x040000EA RID: 234
		private int _yMin;

		// Token: 0x040000EB RID: 235
		private int _yMax;

		// Token: 0x040000EC RID: 236
		private int _total;

		// Token: 0x040000ED RID: 237
		private int _extent;

		// Token: 0x040000EE RID: 238
		private int _jumpSize;

		// Token: 0x040000EF RID: 239
		private int _pixelTotal;

		// Token: 0x040000F0 RID: 240
		private int _startPos;

		// Token: 0x040000F1 RID: 241
		private int _pixelCount;

		// Token: 0x040000F2 RID: 242
		private int _xCenter;

		// Token: 0x040000F3 RID: 243
		private int _yCenter;
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000004 RID: 4
	internal struct DmtxBresLine
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000020D0 File Offset: 0x000002D0
		internal DmtxBresLine(DmtxBresLine orig)
		{
			this._error = orig._error;
			this._loc = new DmtxPixelLoc
			{
				X = orig._loc.X,
				Y = orig._loc.Y
			};
			this._loc0 = new DmtxPixelLoc
			{
				X = orig._loc0.X,
				Y = orig._loc0.Y
			};
			this._loc1 = new DmtxPixelLoc
			{
				X = orig._loc1.X,
				Y = orig._loc1.Y
			};
			this._outward = orig._outward;
			this._steep = orig._steep;
			this._travel = orig._travel;
			this._xDelta = orig._xDelta;
			this._xOut = orig._xOut;
			this._xStep = orig._xStep;
			this._yDelta = orig._yDelta;
			this._yOut = orig._yOut;
			this._yStep = orig._yStep;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002208 File Offset: 0x00000408
		internal DmtxBresLine(DmtxPixelLoc loc0, DmtxPixelLoc loc1, DmtxPixelLoc locInside)
		{
			this._loc0 = loc0;
			this._loc1 = loc1;
			this._xStep = ((loc0.X < loc1.X) ? 1 : -1);
			this._yStep = ((loc0.Y < loc1.Y) ? 1 : -1);
			this._xDelta = Math.Abs(loc1.X - loc0.X);
			this._yDelta = Math.Abs(loc1.Y - loc0.Y);
			this._steep = (this._yDelta > this._xDelta);
			if (this._steep)
			{
				DmtxPixelLoc dmtxPixelLoc;
				DmtxPixelLoc dmtxPixelLoc2;
				if (loc0.Y < loc1.Y)
				{
					dmtxPixelLoc = loc0;
					dmtxPixelLoc2 = loc1;
				}
				else
				{
					dmtxPixelLoc = loc1;
					dmtxPixelLoc2 = loc0;
				}
				int num = (dmtxPixelLoc2.X - dmtxPixelLoc.X) * (locInside.Y - dmtxPixelLoc2.Y) - (dmtxPixelLoc2.Y - dmtxPixelLoc.Y) * (locInside.X - dmtxPixelLoc2.X);
				this._xOut = ((num > 0) ? 1 : -1);
				this._yOut = 0;
			}
			else
			{
				DmtxPixelLoc dmtxPixelLoc;
				DmtxPixelLoc dmtxPixelLoc2;
				if (loc0.X > loc1.X)
				{
					dmtxPixelLoc = loc0;
					dmtxPixelLoc2 = loc1;
				}
				else
				{
					dmtxPixelLoc = loc1;
					dmtxPixelLoc2 = loc0;
				}
				int num = (dmtxPixelLoc2.X - dmtxPixelLoc.X) * (locInside.Y - dmtxPixelLoc2.Y) - (dmtxPixelLoc2.Y - dmtxPixelLoc.Y) * (locInside.X - dmtxPixelLoc2.X);
				this._xOut = 0;
				this._yOut = ((num > 0) ? 1 : -1);
			}
			this._loc = loc0;
			this._travel = 0;
			this._outward = 0;
			this._error = (this._steep ? (this._yDelta / 2) : (this._xDelta / 2));
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023DC File Offset: 0x000005DC
		internal bool GetStep(DmtxPixelLoc target, ref int travel, ref int outward)
		{
			if (this._steep)
			{
				travel = ((this._yStep > 0) ? (target.Y - this._loc.Y) : (this._loc.Y - target.Y));
				this.Step(travel, 0);
				outward = ((this._xOut > 0) ? (target.X - this._loc.X) : (this._loc.X - target.X));
				if (this._yOut != 0)
				{
					throw new Exception("Invald yOut value for bresline step!");
				}
			}
			else
			{
				travel = ((this._xStep > 0) ? (target.X - this._loc.X) : (this._loc.X - target.X));
				this.Step(travel, 0);
				outward = ((this._yOut > 0) ? (target.Y - this._loc.Y) : (this._loc.Y - target.Y));
				if (this._xOut != 0)
				{
					throw new Exception("Invald xOut value for bresline step!");
				}
			}
			return true;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000251C File Offset: 0x0000071C
		internal bool Step(int travel, int outward)
		{
			if (Math.Abs(travel) >= 2)
			{
				throw new ArgumentException("Invalid value for 'travel' in BaseLineStep!");
			}
			if (travel > 0)
			{
				this._travel++;
				if (this._steep)
				{
					this._loc = new DmtxPixelLoc
					{
						X = this._loc.X,
						Y = this._loc.Y + this._yStep
					};
					this._error -= this._xDelta;
					if (this._error < 0)
					{
						this._loc = new DmtxPixelLoc
						{
							X = this._loc.X + this._xStep,
							Y = this._loc.Y
						};
						this._error += this._yDelta;
					}
				}
				else
				{
					this._loc = new DmtxPixelLoc
					{
						X = this._loc.X + this._xStep,
						Y = this._loc.Y
					};
					this._error -= this._yDelta;
					if (this._error < 0)
					{
						this._loc = new DmtxPixelLoc
						{
							X = this._loc.X,
							Y = this._loc.Y + this._yStep
						};
						this._error += this._xDelta;
					}
				}
			}
			else if (travel < 0)
			{
				this._travel--;
				if (this._steep)
				{
					this._loc = new DmtxPixelLoc
					{
						X = this._loc.X,
						Y = this._loc.Y - this._yStep
					};
					this._error += this._xDelta;
					if (this.Error >= this.YDelta)
					{
						this._loc = new DmtxPixelLoc
						{
							X = this._loc.X - this._xStep,
							Y = this._loc.Y
						};
						this._error -= this._yDelta;
					}
				}
				else
				{
					this._loc = new DmtxPixelLoc
					{
						X = this._loc.X - this._xStep,
						Y = this._loc.Y
					};
					this._error += this._yDelta;
					if (this._error >= this._xDelta)
					{
						this._loc = new DmtxPixelLoc
						{
							X = this._loc.X,
							Y = this._loc.Y - this._yStep
						};
						this._error -= this._xDelta;
					}
				}
			}
			for (int i = 0; i < outward; i++)
			{
				this._outward++;
				this._loc = new DmtxPixelLoc
				{
					X = this._loc.X + this._xOut,
					Y = this._loc.Y + this._yOut
				};
			}
			return true;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002900 File Offset: 0x00000B00
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002918 File Offset: 0x00000B18
		internal int XStep
		{
			get
			{
				return this._xStep;
			}
			set
			{
				this._xStep = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002924 File Offset: 0x00000B24
		// (set) Token: 0x06000010 RID: 16 RVA: 0x0000293C File Offset: 0x00000B3C
		internal int YStep
		{
			get
			{
				return this._yStep;
			}
			set
			{
				this._yStep = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002948 File Offset: 0x00000B48
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002960 File Offset: 0x00000B60
		internal int XDelta
		{
			get
			{
				return this._xDelta;
			}
			set
			{
				this._xDelta = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000296C File Offset: 0x00000B6C
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002984 File Offset: 0x00000B84
		internal int YDelta
		{
			get
			{
				return this._yDelta;
			}
			set
			{
				this._yDelta = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002990 File Offset: 0x00000B90
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000029A8 File Offset: 0x00000BA8
		internal bool Steep
		{
			get
			{
				return this._steep;
			}
			set
			{
				this._steep = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000029B4 File Offset: 0x00000BB4
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000029CC File Offset: 0x00000BCC
		internal int XOut
		{
			get
			{
				return this._xOut;
			}
			set
			{
				this._xOut = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000029D8 File Offset: 0x00000BD8
		// (set) Token: 0x0600001A RID: 26 RVA: 0x000029F0 File Offset: 0x00000BF0
		internal int YOut
		{
			get
			{
				return this._yOut;
			}
			set
			{
				this._yOut = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000029FC File Offset: 0x00000BFC
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002A14 File Offset: 0x00000C14
		internal int Travel
		{
			get
			{
				return this._travel;
			}
			set
			{
				this._travel = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002A20 File Offset: 0x00000C20
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002A38 File Offset: 0x00000C38
		internal int Outward
		{
			get
			{
				return this._outward;
			}
			set
			{
				this._outward = value;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002A44 File Offset: 0x00000C44
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002A5C File Offset: 0x00000C5C
		internal int Error
		{
			get
			{
				return this._error;
			}
			set
			{
				this._error = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002A68 File Offset: 0x00000C68
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002A80 File Offset: 0x00000C80
		internal DmtxPixelLoc Loc
		{
			get
			{
				return this._loc;
			}
			set
			{
				this._loc = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002A8C File Offset: 0x00000C8C
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002AA4 File Offset: 0x00000CA4
		internal DmtxPixelLoc Loc0
		{
			get
			{
				return this._loc0;
			}
			set
			{
				this._loc0 = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002AB0 File Offset: 0x00000CB0
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002AC8 File Offset: 0x00000CC8
		internal DmtxPixelLoc Loc1
		{
			get
			{
				return this._loc1;
			}
			set
			{
				this._loc1 = value;
			}
		}

		// Token: 0x04000005 RID: 5
		private int _xStep;

		// Token: 0x04000006 RID: 6
		private int _yStep;

		// Token: 0x04000007 RID: 7
		private int _xDelta;

		// Token: 0x04000008 RID: 8
		private int _yDelta;

		// Token: 0x04000009 RID: 9
		private bool _steep;

		// Token: 0x0400000A RID: 10
		private int _xOut;

		// Token: 0x0400000B RID: 11
		private int _yOut;

		// Token: 0x0400000C RID: 12
		private int _travel;

		// Token: 0x0400000D RID: 13
		private int _outward;

		// Token: 0x0400000E RID: 14
		private int _error;

		// Token: 0x0400000F RID: 15
		private DmtxPixelLoc _loc;

		// Token: 0x04000010 RID: 16
		private DmtxPixelLoc _loc0;

		// Token: 0x04000011 RID: 17
		private DmtxPixelLoc _loc1;
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000022 RID: 34
	internal struct DmtxFollow
	{
		// Token: 0x1700008B RID: 139
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000E05C File Offset: 0x0000C25C
		internal int PtrIndex
		{
			set
			{
				this._ptrIndex = value;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000E068 File Offset: 0x0000C268
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000E087 File Offset: 0x0000C287
		internal byte CurrentPtr
		{
			get
			{
				return this.Ptr[this._ptrIndex];
			}
			set
			{
				this.Ptr[this._ptrIndex] = value;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000E098 File Offset: 0x0000C298
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000E0AF File Offset: 0x0000C2AF
		internal byte[] Ptr { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000E0B8 File Offset: 0x0000C2B8
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000E0D7 File Offset: 0x0000C2D7
		internal byte Neighbor
		{
			get
			{
				return this.Ptr[this._ptrIndex];
			}
			set
			{
				this.Ptr[this._ptrIndex] = value;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000E0E8 File Offset: 0x0000C2E8
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000E0FF File Offset: 0x0000C2FF
		internal int Step { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000E108 File Offset: 0x0000C308
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000E11F File Offset: 0x0000C31F
		internal DmtxPixelLoc Loc { get; set; }

		// Token: 0x0400013A RID: 314
		private int _ptrIndex;
	}
}

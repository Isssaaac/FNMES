using System;

namespace DataMatrix.net
{
	// Token: 0x0200001D RID: 29
	internal class DmtxRay2
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600014C RID: 332 RVA: 0x0000CAE8 File Offset: 0x0000ACE8
		// (set) Token: 0x0600014D RID: 333 RVA: 0x0000CB12 File Offset: 0x0000AD12
		internal DmtxVector2 P
		{
			get
			{
				DmtxVector2 result;
				if ((result = this._p) == null)
				{
					result = (this._p = new DmtxVector2());
				}
				return result;
			}
			set
			{
				this._p = value;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600014E RID: 334 RVA: 0x0000CB1C File Offset: 0x0000AD1C
		// (set) Token: 0x0600014F RID: 335 RVA: 0x0000CB46 File Offset: 0x0000AD46
		internal DmtxVector2 V
		{
			get
			{
				DmtxVector2 result;
				if ((result = this._v) == null)
				{
					result = (this._v = new DmtxVector2());
				}
				return result;
			}
			set
			{
				this._v = value;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000150 RID: 336 RVA: 0x0000CB50 File Offset: 0x0000AD50
		// (set) Token: 0x06000151 RID: 337 RVA: 0x0000CB67 File Offset: 0x0000AD67
		internal double TMin { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000152 RID: 338 RVA: 0x0000CB70 File Offset: 0x0000AD70
		// (set) Token: 0x06000153 RID: 339 RVA: 0x0000CB87 File Offset: 0x0000AD87
		internal double TMax { get; set; }

		// Token: 0x0400011D RID: 285
		private DmtxVector2 _p;

		// Token: 0x0400011E RID: 286
		private DmtxVector2 _v;
	}
}

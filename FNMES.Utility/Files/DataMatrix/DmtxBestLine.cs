using System;

namespace DataMatrix.net
{
	// Token: 0x02000025 RID: 37
	internal struct DmtxBestLine
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000F574 File Offset: 0x0000D774
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000F58B File Offset: 0x0000D78B
		internal int Angle { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000F594 File Offset: 0x0000D794
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000F5AB File Offset: 0x0000D7AB
		internal int HOffset { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000F5B4 File Offset: 0x0000D7B4
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000F5CB File Offset: 0x0000D7CB
		internal int Mag { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000F5D4 File Offset: 0x0000D7D4
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000F5EB File Offset: 0x0000D7EB
		internal int StepBeg { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000F5F4 File Offset: 0x0000D7F4
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000F60B File Offset: 0x0000D80B
		internal int StepPos { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000F614 File Offset: 0x0000D814
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000F62B File Offset: 0x0000D82B
		internal int StepNeg { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000F634 File Offset: 0x0000D834
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000F64B File Offset: 0x0000D84B
		internal int DistSq { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000F654 File Offset: 0x0000D854
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000F66B File Offset: 0x0000D86B
		internal double Devn { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000F674 File Offset: 0x0000D874
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000F68B File Offset: 0x0000D88B
		internal DmtxPixelLoc LocBeg { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000F694 File Offset: 0x0000D894
		// (set) Token: 0x060001EB RID: 491 RVA: 0x0000F6AB File Offset: 0x0000D8AB
		internal DmtxPixelLoc LocPos { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000F6B4 File Offset: 0x0000D8B4
		// (set) Token: 0x060001ED RID: 493 RVA: 0x0000F6CB File Offset: 0x0000D8CB
		internal DmtxPixelLoc LocNeg { get; set; }
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000017 RID: 23
	internal class DmtxPointFlow
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004FA4 File Offset: 0x000031A4
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004FBB File Offset: 0x000031BB
		internal int Plane { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00004FC4 File Offset: 0x000031C4
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00004FDB File Offset: 0x000031DB
		internal int Arrive { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004FE4 File Offset: 0x000031E4
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00004FFB File Offset: 0x000031FB
		internal int Depart { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00005004 File Offset: 0x00003204
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x0000501B File Offset: 0x0000321B
		internal int Mag { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00005024 File Offset: 0x00003224
		// (set) Token: 0x060000AA RID: 170 RVA: 0x0000503B File Offset: 0x0000323B
		internal DmtxPixelLoc Loc { get; set; }
	}
}

using System;
using System.Drawing;

namespace DataMatrix.net
{
	// Token: 0x02000020 RID: 32
	public class DmtxImageEncoderOptions
	{
		// Token: 0x06000185 RID: 389 RVA: 0x0000D614 File Offset: 0x0000B814
		public DmtxImageEncoderOptions()
		{
			this.BackColor = Color.White;
			this.ForeColor = Color.Black;
			this.SizeIdx = DmtxSymbolSize.DmtxSymbolSquareAuto;
			this.Scheme = DmtxScheme.DmtxSchemeAscii;
			this.ModuleSize = 5;
			this.MarginSize = 10;
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000186 RID: 390 RVA: 0x0000D664 File Offset: 0x0000B864
		// (set) Token: 0x06000187 RID: 391 RVA: 0x0000D67B File Offset: 0x0000B87B
		public int MarginSize { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0000D684 File Offset: 0x0000B884
		// (set) Token: 0x06000189 RID: 393 RVA: 0x0000D69B File Offset: 0x0000B89B
		public int ModuleSize { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0000D6A4 File Offset: 0x0000B8A4
		// (set) Token: 0x0600018B RID: 395 RVA: 0x0000D6BB File Offset: 0x0000B8BB
		public DmtxScheme Scheme { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600018C RID: 396 RVA: 0x0000D6C4 File Offset: 0x0000B8C4
		// (set) Token: 0x0600018D RID: 397 RVA: 0x0000D6DB File Offset: 0x0000B8DB
		public DmtxSymbolSize SizeIdx { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600018E RID: 398 RVA: 0x0000D6E4 File Offset: 0x0000B8E4
		// (set) Token: 0x0600018F RID: 399 RVA: 0x0000D6FB File Offset: 0x0000B8FB
		public Color ForeColor { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000D704 File Offset: 0x0000B904
		// (set) Token: 0x06000191 RID: 401 RVA: 0x0000D71B File Offset: 0x0000B91B
		public Color BackColor { get; set; }
	}
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000018 RID: 24
	internal class DmtxChannel
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000504C File Offset: 0x0000324C
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00005063 File Offset: 0x00003263
		internal byte[] Input { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000506C File Offset: 0x0000326C
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00005083 File Offset: 0x00003283
		internal DmtxScheme EncScheme { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x0000508C File Offset: 0x0000328C
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000050A3 File Offset: 0x000032A3
		internal DmtxChannelStatus Invalid { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000050AC File Offset: 0x000032AC
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000050C3 File Offset: 0x000032C3
		internal int InputIndex { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000050CC File Offset: 0x000032CC
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x000050E3 File Offset: 0x000032E3
		internal int EncodedLength { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000050EC File Offset: 0x000032EC
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00005103 File Offset: 0x00003303
		internal int CurrentLength { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000510C File Offset: 0x0000330C
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00005123 File Offset: 0x00003323
		internal int FirstCodeWord { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000512C File Offset: 0x0000332C
		internal byte[] EncodedWords
		{
			get
			{
				byte[] result;
				if ((result = this._encodedWords) == null)
				{
					result = (this._encodedWords = new byte[1558]);
				}
				return result;
			}
		}

		// Token: 0x040000F9 RID: 249
		private byte[] _encodedWords;
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace DataMatrix.net
{
	// Token: 0x02000006 RID: 6
	public class DmtxImageDecoder
	{
		// Token: 0x06000071 RID: 113 RVA: 0x00003154 File Offset: 0x00001354
		public List<string> DecodeImage(Bitmap image)
		{
			return this.DecodeImage(image, int.MaxValue, TimeSpan.MaxValue);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003178 File Offset: 0x00001378
		public List<string> DecodeImage(Bitmap image, TimeSpan timeSpan)
		{
			return this.DecodeImage(image, int.MaxValue, timeSpan);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003198 File Offset: 0x00001398
		public List<string> DecodeImageMosaic(Bitmap image)
		{
			return this.DecodeImageMosaic(image, int.MaxValue, TimeSpan.MaxValue);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000031BC File Offset: 0x000013BC
		public List<string> DecodeImageMosaic(Bitmap image, int maxResultCount, TimeSpan timeOut)
		{
			return this.DecodeImage(image, maxResultCount, timeOut, true);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000031D8 File Offset: 0x000013D8
		public List<string> DecodeImageMosaic(Bitmap image, TimeSpan timeSpan)
		{
			return this.DecodeImage(image, int.MaxValue, timeSpan);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000031F8 File Offset: 0x000013F8
		public List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut)
		{
			return this.DecodeImage(image, maxResultCount, timeOut, false);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003214 File Offset: 0x00001414
		private List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut, bool isMosaic)
		{
			List<string> list = new List<string>();
			int num;
			byte[] pxl = this.ImageToByteArray(image, out num);
			DmtxDecode dmtxDecode = new DmtxDecode(new DmtxImage(pxl, image.Width, image.Height, DmtxPackOrder.DmtxPack24bppRGB)
			{
				RowPadBytes = num % 3
			}, 1);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!(stopwatch.Elapsed > timeOut))
			{
				DmtxRegion dmtxRegion = dmtxDecode.RegionFindNext(timeOut);
				if (dmtxRegion != null)
				{
					DmtxMessage dmtxMessage = isMosaic ? dmtxDecode.MosaicRegion(dmtxRegion, -1) : dmtxDecode.MatrixRegion(dmtxRegion, -1);
					string text = Encoding.ASCII.GetString(dmtxMessage.Output, 0, dmtxMessage.Output.Length);
					text = text.Substring(0, text.IndexOf('\0'));
					if (!list.Contains(text))
					{
						list.Add(text);
						if (list.Count >= maxResultCount)
						{
							break;
						}
					}
					continue;
				}
				return list;
			}
			return list;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003330 File Offset: 0x00001530
		private byte[] ImageToByteArray(Bitmap b, out int stride)
		{
			Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
			BitmapData bitmapData = b.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			byte[] result;
			try
			{
				byte[] array = new byte[bitmapData.Stride * b.Height];
				Marshal.Copy(bitmapData.Scan0, array, 0, bitmapData.Stride * b.Height);
				stride = bitmapData.Stride;
				result = array;
			}
			finally
			{
				b.UnlockBits(bitmapData);
			}
			return result;
		}
	}
}

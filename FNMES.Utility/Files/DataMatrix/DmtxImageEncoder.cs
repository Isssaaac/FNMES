using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace DataMatrix.net
{
	// Token: 0x02000021 RID: 33
	public class DmtxImageEncoder
	{
		// Token: 0x06000192 RID: 402 RVA: 0x0000D724 File Offset: 0x0000B924
		public Bitmap EncodeImageMosaic(string val)
		{
			return this.EncodeImageMosaic(val, DmtxImageEncoder.DefaultDotSize);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000D744 File Offset: 0x0000B944
		public Bitmap EncodeImageMosaic(string val, int dotSize)
		{
			return this.EncodeImageMosaic(val, dotSize, DmtxImageEncoder.DefaultMargin);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000D764 File Offset: 0x0000B964
		public Bitmap EncodeImageMosaic(string val, int dotSize, int margin)
		{
			DmtxImageEncoderOptions options = new DmtxImageEncoderOptions
			{
				MarginSize = margin,
				ModuleSize = dotSize
			};
			return this.EncodeImageMosaic(val, options);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000D798 File Offset: 0x0000B998
		public Bitmap EncodeImageMosaic(string val, DmtxImageEncoderOptions options)
		{
			return this.EncodeImage(val, options, true);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000D7B4 File Offset: 0x0000B9B4
		private Bitmap EncodeImage(string val, DmtxImageEncoderOptions options, bool isMosaic)
		{
			DmtxEncode dmtxEncode = new DmtxEncode
			{
				ModuleSize = options.ModuleSize,
				MarginSize = options.MarginSize,
				SizeIdxRequest = options.SizeIdx
			};
			byte[] rawDataAndSetEncoding = DmtxImageEncoder.GetRawDataAndSetEncoding(val, options, dmtxEncode);
			if (isMosaic)
			{
				dmtxEncode.EncodeDataMosaic(rawDataAndSetEncoding);
			}
			else
			{
				dmtxEncode.EncodeDataMatrix(new Color?(options.ForeColor), new Color?(options.BackColor), rawDataAndSetEncoding);
			}
			return DmtxImageEncoder.CopyDataToBitmap(dmtxEncode.Image.Pxl, dmtxEncode.Image.Width, dmtxEncode.Image.Height);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000D85C File Offset: 0x0000BA5C
		private static byte[] GetRawDataAndSetEncoding(string code, DmtxImageEncoderOptions options, DmtxEncode encode)
		{
			byte[] array = Encoding.ASCII.GetBytes(code);
			encode.Scheme = options.Scheme;
			if (options.Scheme == DmtxScheme.DmtxSchemeAsciiGS1)
			{
				List<byte> list = new List<byte>(new byte[]
				{
					232
				});
				list.AddRange(array);
				array = list.ToArray();
				encode.Scheme = DmtxScheme.DmtxSchemeAscii;
			}
			return array;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000D8CC File Offset: 0x0000BACC
		public Bitmap EncodeImage(string val)
		{
			return this.EncodeImage(val, DmtxImageEncoder.DefaultDotSize, DmtxImageEncoder.DefaultMargin);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000D8F0 File Offset: 0x0000BAF0
		public Bitmap EncodeImage(string val, int dotSize)
		{
			return this.EncodeImage(val, dotSize, DmtxImageEncoder.DefaultMargin);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000D910 File Offset: 0x0000BB10
		public Bitmap EncodeImage(string val, int dotSize, int margin)
		{
			DmtxImageEncoderOptions options = new DmtxImageEncoderOptions
			{
				MarginSize = margin,
				ModuleSize = dotSize
			};
			return this.EncodeImage(val, options);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000D944 File Offset: 0x0000BB44
		public Bitmap EncodeImage(string val, DmtxImageEncoderOptions options)
		{
			return this.EncodeImage(val, options, false);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000D960 File Offset: 0x0000BB60
		public string EncodeSvgImage(string val)
		{
			return this.EncodeSvgImage(val, DmtxImageEncoder.DefaultDotSize, DmtxImageEncoder.DefaultMargin, DmtxImageEncoder.DefaultForeColor, DmtxImageEncoder.DefaultBackColor);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000D990 File Offset: 0x0000BB90
		public string EncodeSvgImage(string val, int dotSize)
		{
			return this.EncodeSvgImage(val, dotSize, DmtxImageEncoder.DefaultMargin, DmtxImageEncoder.DefaultForeColor, DmtxImageEncoder.DefaultBackColor);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000D9BC File Offset: 0x0000BBBC
		public string EncodeSvgImage(string val, int dotSize, int margin)
		{
			return this.EncodeSvgImage(val, dotSize, margin, DmtxImageEncoder.DefaultForeColor, DmtxImageEncoder.DefaultBackColor);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000D9E4 File Offset: 0x0000BBE4
		public string EncodeSvgImage(string val, int dotSize, int margin, Color foreColor, Color backColor)
		{
			DmtxImageEncoderOptions options = new DmtxImageEncoderOptions
			{
				ModuleSize = dotSize,
				MarginSize = margin,
				ForeColor = foreColor,
				BackColor = backColor
			};
			return this.EncodeSvgImage(val, options);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000DA28 File Offset: 0x0000BC28
		public bool[,] EncodeRawData(string val)
		{
			return this.EncodeRawData(val, new DmtxImageEncoderOptions());
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000DA48 File Offset: 0x0000BC48
		public bool[,] EncodeRawData(string val, DmtxImageEncoderOptions options)
		{
			DmtxEncode dmtxEncode = new DmtxEncode
			{
				ModuleSize = 1,
				MarginSize = 0,
				SizeIdxRequest = options.SizeIdx,
				Scheme = options.Scheme
			};
			byte[] rawDataAndSetEncoding = DmtxImageEncoder.GetRawDataAndSetEncoding(val, options, dmtxEncode);
			dmtxEncode.EncodeDataMatrixRaw(rawDataAndSetEncoding);
			return dmtxEncode.RawData;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000DAA4 File Offset: 0x0000BCA4
		public string EncodeSvgImage(string val, DmtxImageEncoderOptions options)
		{
			DmtxEncode dmtxEncode = new DmtxEncode
			{
				ModuleSize = options.ModuleSize,
				MarginSize = options.MarginSize,
				SizeIdxRequest = options.SizeIdx,
				Scheme = options.Scheme
			};
			byte[] rawDataAndSetEncoding = DmtxImageEncoder.GetRawDataAndSetEncoding(val, options, dmtxEncode);
			dmtxEncode.EncodeDataMatrix(new Color?(options.ForeColor), new Color?(options.BackColor), rawDataAndSetEncoding);
			return this.EncodeSvgFile(dmtxEncode, "", options.ModuleSize, options.MarginSize, options.ForeColor, options.BackColor);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000DB40 File Offset: 0x0000BD40
		internal static Bitmap CopyDataToBitmap(byte[] data, int width, int height)
		{
			data = DmtxImageEncoder.InsertPaddingBytes(data, width, height, 24);
			Bitmap bitmap = new Bitmap(width, height);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000DB98 File Offset: 0x0000BD98
		private static byte[] InsertPaddingBytes(byte[] data, int width, int height, int bitsPerPixel)
		{
			int num = 4 * ((width * bitsPerPixel + 31) / 32);
			int num2 = num - 3 * width;
			byte[] result;
			if (num2 == 0)
			{
				result = data;
			}
			else
			{
				byte[] array = new byte[num * height];
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						array[i * num + 3 * j] = data[3 * (i * width + j)];
						array[i * num + 3 * j + 1] = data[3 * (i * width + j) + 1];
						array[i * num + 3 * j + 2] = data[3 * (i * width + j) + 2];
					}
					for (int k = 0; k < num2; k++)
					{
						array[i * num + 3 * k] = byte.MaxValue;
						array[i * num + 3 * k + 1] = byte.MaxValue;
						array[i * num + 3 * k + 2] = byte.MaxValue;
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000DCA0 File Offset: 0x0000BEA0
		internal string EncodeSvgFile(DmtxEncode enc, string format, int moduleSize, int margin, Color foreColor, Color backColor)
		{
			bool flag = false;
			string text = null;
			string text2 = "";
			if (DmtxImageEncoder._dotFormatProvider == null)
			{
				DmtxImageEncoder._dotFormatProvider = new NumberFormatInfo
				{
					NumberDecimalSeparator = "."
				};
			}
			if (format == "svg:")
			{
				flag = true;
				text = format.Substring(4);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "dmtx_0001";
			}
			int num = 2 * enc.MarginSize + enc.Region.SymbolCols * enc.ModuleSize;
			int num2 = 2 * enc.MarginSize + enc.Region.SymbolRows * enc.ModuleSize;
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, enc.Region.SizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, enc.Region.SizeIdx);
			if (!flag)
			{
				text2 += string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n<!-- Created with DataMatrix.net (http://datamatrixnet.sourceforge.net/) -->\n<svg\nxmlns:svg=\"http://www.w3.org/2000/svg\"\nxmlns=\"http://www.w3.org/2000/svg\"\nxmlns:xlink=\"http://www.w3.org/1999/xlink\"\nversion=\"1.0\"\nwidth=\"{0}\"\nheight=\"{1}\"\nid=\"svg2\">\n<defs>\n<symbol id=\"{2}\">\n    <desc>Layout:{0}x%{1} Symbol:{3}x{4} Data Matrix</desc>\n", new object[]
				{
					num,
					num2,
					text,
					symbolAttribute,
					symbolAttribute2
				});
			}
			if (backColor != Color.White)
			{
				string text3 = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ", new object[]
				{
					backColor.R.ToString("X2"),
					backColor.G.ToString("X2"),
					backColor.B.ToString("X2"),
					((double)backColor.A / 255.0).ToString("0.##", DmtxImageEncoder._dotFormatProvider)
				});
				text2 += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"0\" {2}/>\n", num, num2, text3);
			}
			for (int i = 0; i < enc.Region.SymbolRows; i++)
			{
				int num3 = enc.Region.SymbolRows - i - 1;
				for (int j = 0; j < enc.Region.SymbolCols; j++)
				{
					int num4 = enc.Message.SymbolModuleStatus(enc.Region.SizeIdx, i, j);
					string text3 = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ", new object[]
					{
						foreColor.R.ToString("X2"),
						foreColor.G.ToString("X2"),
						foreColor.B.ToString("X2"),
						((double)foreColor.A / 255.0).ToString("0.##", DmtxImageEncoder._dotFormatProvider)
					});
					if ((num4 & DmtxConstants.DmtxModuleOn) != 0)
					{
						text2 += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" {4}/>\n", new object[]
						{
							moduleSize,
							moduleSize,
							j * moduleSize + margin,
							num3 * moduleSize + margin,
							text3
						});
					}
				}
			}
			text2 += "  </symbol>\n";
			if (!flag)
			{
				text2 += string.Format("</defs>\n<use xlink:href=\"#{0}\" x='0' y='0' style=\"fill:#000000;fill-opacity:1;stroke:none\" />\n\n</svg>\n", text);
			}
			return text2;
		}

		// Token: 0x04000135 RID: 309
		public static readonly int DefaultDotSize = 5;

		// Token: 0x04000136 RID: 310
		public static readonly int DefaultMargin = 10;

		// Token: 0x04000137 RID: 311
		public static readonly Color DefaultBackColor = Color.White;

		// Token: 0x04000138 RID: 312
		public static readonly Color DefaultForeColor = Color.Black;

		// Token: 0x04000139 RID: 313
		private static NumberFormatInfo _dotFormatProvider;
	}
}

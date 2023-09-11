using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ZXing;
using ZXing.Common;

namespace FNMES.Utility.Files
{
    public class BarCodeUtils
    {
        public static Bitmap EncodeBarCode(BarcodeFormat barcodeFormat, string message, int widthPixels, int hightPixels)
        {
            BitMatrix matrix = new MultiFormatWriter().encode(message, barcodeFormat, widthPixels, hightPixels);
            int width = matrix.Width;
            int height = matrix.Height;
            var white = System.Drawing.ColorTranslator.FromHtml("0xFFFFFFFF");
            var black = System.Drawing.ColorTranslator.FromHtml("0xFF000000");
            System.Drawing.Bitmap bmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix[x, y] ? black : white);
                }
            }
            return bmap;
        }

        public static Bitmap EncodeBarCode(string message, int widthPixels, int hightPixels)
        {
            return EncodeBarCode(BarcodeFormat.CODE_128, message, widthPixels, hightPixels);
        }

        public static Image EncodeBarCode(string msg)
        {
            return EncodeBarCode(msg, 960, 500);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Image BitmapToImage(Bitmap bitmap)
        {
            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ImageToBitmap(Image img)
        {
            return new Bitmap(img);
        }
    }
}

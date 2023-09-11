using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace FNMES.Utility.Files
{
    public class QRCodeUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">二维码内容</param>
        /// <param name="codeSizeInPixels">图片长宽</param>
        /// <returns></returns>
        public static Bitmap EncodeQrCode(string msg, int codeSizeInPixels)
        {
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");
            hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            hints.Add(EncodeHintType.MARGIN, 1);
            //msg = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(msg));
            BitMatrix matrix = new MultiFormatWriter().encode(msg, BarcodeFormat.QR_CODE, codeSizeInPixels, codeSizeInPixels, hints);
            matrix = deleteWhite(matrix);//删除白边
            Bitmap bmap = new Bitmap(matrix.Width, matrix.Height, PixelFormat.Format32bppArgb);
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    if (matrix[x, y])
                        bmap.SetPixel(x, y, Color.Black);
                }
            }
            return bmap;
        }

        /// <summary>
        /// 去除白边
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static BitMatrix deleteWhite(BitMatrix matrix)
        {
            int[] rec = matrix.getEnclosingRectangle();
            int resWidth = rec[2] + 1;
            int resHeight = rec[3] + 1;

            BitMatrix resMatrix = new BitMatrix(resWidth, resHeight);
            resMatrix.clear();
            for (int i = 0; i < resWidth; i++)
            {
                for (int j = 0; j < resHeight; j++)
                {
                    if (matrix[i + rec[0], j + rec[1]])
                        resMatrix[i, j] = true;
                }
            }
            return resMatrix;
        }

        public static Image EncodeQrCode(string msg)
        {
            return EncodeQrCode(msg, 900);
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

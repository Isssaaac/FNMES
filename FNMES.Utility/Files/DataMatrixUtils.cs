using DataMatrix.net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FNMES.Utility.Files
{
    public class DataMatrixUtils
    {

        public static Bitmap EncodeDataMatrix(DmtxSymbolSize size, string msg, int codeSizeInPixels)
        {
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            DmtxImageEncoderOptions options1 = new DmtxImageEncoderOptions();
            options1.SizeIdx = size;
            options1.MarginSize = 0;
            //options1.ModuleSize = 20;//每个size为20,生成的图片宽度为12*20
            options1.ModuleSize = (int)(codeSizeInPixels / 12.0);//每个size为20
            options1.ForeColor = Color.Black;
            options1.BackColor = Color.White;
            DmtxImageEncoderOptions options = options1;
            return encoder.EncodeImage(msg, options);
        }


        /// <summary>
        /// 默认size为12
        /// </summary>
        /// <param name="msg">DataMatrix内容</param>
        /// <param name="codeSizeInPixels">图片长宽</param>
        /// <returns></returns>
        public static Bitmap EncodeDataMatrix(string msg, int codeSizeInPixels)
        {
            return EncodeDataMatrix(DmtxSymbolSize.DmtxSymbol10x10, msg, codeSizeInPixels);
        }



        public static Image EncodeDataMatrix(string msg)
        {
            return EncodeDataMatrix(msg, 900);
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

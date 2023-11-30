using FNMES.Utility.Files.ICOGen;
using FNMES.Utility.Other;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Files
{
    public static class ImageUtils
    {
        /// <summary>
        /// 根据Image 获得ICO
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Icon GetIcon(Image image, IconSize size)
        {
            Bitmap bitmap = ResizeImage(new Bitmap(image), (int)size, (int)size);
            IconDir icon = new IconDir();
            icon.AddImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            bitmap.Dispose();
            return new Icon(icon.SaveData());
        }

        /// <summary>
        /// GIF分割
        /// </summary>
        /// <param name="gif"></param>
        /// <returns></returns>
        public static List<Image> SplitGif(Image gif)
        {
            List<Image> list = new List<Image>();
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            //获取gif帧的数量
            int count = gif.GetFrameCount(fd);

            //遍历保存图片
            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                list.Add(new Bitmap(gif));
            }
            return list;
        }
        /// <summary>
        /// 获取原图像绕中心任意角度旋转后的图像
        /// </summary>
        /// <param name="rawImg"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Image GetRotateImage(Image srcImage, int angle)
        {
            angle = angle - 90;
            angle = angle % 360;
            //原图的宽和高
            int srcWidth = srcImage.Width;
            int srcHeight = srcImage.Height;
            //图像旋转之后所占区域宽和高
            Rectangle rotateRec = GetRotateRectangle(srcWidth, srcHeight, 360 - angle);
            int rotateWidth = rotateRec.Width;
            int rotateHeight = rotateRec.Height;
            //目标位图
            Bitmap destImage = null;
            Graphics graphics = null;
            try
            {
                //定义画布，宽高为图像旋转后的宽高
                destImage = new Bitmap(rotateWidth, rotateHeight);
                //graphics根据destImage创建，因此其原点此时在destImage左上角
                graphics = Graphics.FromImage(destImage);
                //要让graphics围绕某矩形中心点旋转N度，分三步
                //第一步，将graphics坐标原点移到矩形中心点,假设其中点坐标（x,y）
                //第二步，graphics旋转相应的角度(沿当前原点)
                //第三步，移回（-x,-y）
                //获取画布中心点
                System.Drawing.Point centerPoint = new System.Drawing.Point(rotateWidth / 2, rotateHeight / 2);
                //将graphics坐标原点移到中心点
                graphics.TranslateTransform(centerPoint.X, centerPoint.Y);
                //graphics旋转相应的角度(绕当前原点)
                graphics.RotateTransform(360 - angle);
                //恢复graphics在水平和垂直方向的平移(沿当前原点)
                graphics.TranslateTransform(-centerPoint.X, -centerPoint.Y);
                //此时已经完成了graphics的旋转

                //计算:如果要将源图像画到画布上且中心与画布中心重合，需要的偏移量
                System.Drawing.Point Offset = new System.Drawing.Point((rotateWidth - srcWidth) / 2, (rotateHeight - srcHeight) / 2);
                //将源图片画到rect里（rotateRec的中心）
                graphics.DrawImage(srcImage, new Rectangle(Offset.X, Offset.Y, srcWidth, srcHeight));
                //重至绘图的所有变换
                graphics.ResetTransform();
                graphics.Save();
            }
            catch (Exception )
            {
                return srcImage;
            }
            finally
            {
                if (graphics != null)
                    graphics.Dispose();
            }
            return destImage;
        }

        /// <summary>
        /// 获取原图像绕中心任意角度旋转后的图像
        /// </summary>
        /// <param name="rawImg"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Image GetRotateSetImage(Image srcImage, int angle, int srcWidth, int srcHeight)
        {
            angle = angle - 90;
            angle = angle % 360;
            //图像旋转之后所占区域宽和高
            Rectangle rotateRec = GetRotateRectangle(srcWidth, srcHeight, 360 - angle);
            int rotateWidth = rotateRec.Width;
            int rotateHeight = rotateRec.Height;
            //目标位图
            Bitmap destImage = null;
            Graphics graphics = null;
            try
            {
                //定义画布，宽高为图像旋转后的宽高
                destImage = new Bitmap(rotateWidth, rotateHeight);
                //graphics根据destImage创建，因此其原点此时在destImage左上角
                graphics = Graphics.FromImage(destImage);
                //要让graphics围绕某矩形中心点旋转N度，分三步
                //第一步，将graphics坐标原点移到矩形中心点,假设其中点坐标（x,y）
                //第二步，graphics旋转相应的角度(沿当前原点)
                //第三步，移回（-x,-y）
                //获取画布中心点
                System.Drawing.Point centerPoint = new System.Drawing.Point(rotateWidth / 2, rotateHeight / 2);
                //将graphics坐标原点移到中心点
                graphics.TranslateTransform(centerPoint.X, centerPoint.Y);
                //graphics旋转相应的角度(绕当前原点)
                graphics.RotateTransform(360 - angle);
                //恢复graphics在水平和垂直方向的平移(沿当前原点)
                graphics.TranslateTransform(-centerPoint.X, -centerPoint.Y);
                //此时已经完成了graphics的旋转

                //计算:如果要将源图像画到画布上且中心与画布中心重合，需要的偏移量
                System.Drawing.Point Offset = new System.Drawing.Point((rotateWidth - srcWidth) / 2, (rotateHeight - srcHeight) / 2);
                //将源图片画到rect里（rotateRec的中心）
                graphics.DrawImage(srcImage, new Rectangle(Offset.X, Offset.Y, srcWidth, srcHeight));
                //重至绘图的所有变换
                graphics.ResetTransform();
                graphics.Save();
            }
            catch (Exception)
            {
                return srcImage;
            }
            finally
            {
                if (graphics != null)
                    graphics.Dispose();
            }
            return destImage;
        }

        /// <summary>
        /// 计算矩形绕中心任意角度旋转后所占区域矩形宽高
        /// </summary>
        /// <param name="width">原矩形的宽</param>
        /// <param name="height">原矩形高</param>
        /// <param name="angle">顺时针旋转角度</param>
        /// <returns></returns>
        private static Rectangle GetRotateRectangle(int width, int height, float angle)
        {
            double radian = angle * Math.PI / 180; ;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            //只需要考虑到第四象限和第三象限的情况取大值(中间用绝对值就可以包括第一和第二象限)
            int newWidth = (int)(Math.Max(Math.Abs(width * cos - height * sin), Math.Abs(width * cos + height * sin)));
            int newHeight = (int)(Math.Max(Math.Abs(width * sin - height * cos), Math.Abs(width * sin + height * cos)));
            return new Rectangle(0, 0, newWidth, newHeight);
        }

        /// <summary>
        /// 图片转为base64编码的字符串  
        /// </summary>
        /// <param name="Imagefilename"></param>
        /// <returns></returns>
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        ///  base64编码的字符串转为图片  
        /// </summary>
        /// <param name="strbase64"></param>
        /// <returns></returns>
        public static Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                string dummyData = strbase64.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                byte[] arr = Convert.FromBase64String(dummyData);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                //bmp.Save(@"d:\test.png", System.Drawing.Imaging.ImageFormat.Png);
                ms.Close();
                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 网络图片转Image
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap GetBitmap(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";

            HttpWebResponse myResponse = null;
            try
            {
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                Stream stream = myResponse.GetResponseStream();
                MemoryStream ms = null;
                Byte[] buffer = new Byte[myResponse.ContentLength];
                int offset = 0, actuallyRead = 0;
                do
                {
                    actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                    offset += actuallyRead;
                }
                while (actuallyRead > 0);
                ms = new MemoryStream(buffer);
                Image image = Image.FromStream(ms);
                myResponse.Close();
                stream.Close();
                ms.Close();
                return new Bitmap(image);
            }
            //异常请求  
            catch (Exception)
            {
                return null;
            }
            //return base64str;
        }


        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="b">图片</param>
        /// <param name="destHeight"></param>
        /// <param name="destWidth"></param>
        /// <returns></returns>
        public static Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
        {
            Image imgSource = b;
            ImageFormat thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            sW = sWidth;
            sH = sHeight;
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.DrawImage(imgSource, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, imgSource.Width, imgSource.Height), GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }


        /// <summary>
        /// 保存缓存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        /// <param name="savePath"></param>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string SaveThumb(string fileName, int destWidth, int destHeight, string savePath, string virtualPath)
        {
            Bitmap bitmap = new Bitmap(fileName);
            Bitmap outBitmap = GetThumbnail(bitmap, destHeight, destWidth);
            string outputName = UUID.StrSnowId;
            string outputFilePath = savePath + virtualPath + outputName + ".png";
            outBitmap.Save(outputFilePath, ImageFormat.Png);
            return virtualPath + outputName + ".png";
        }

        private static string GetFullPath(string path)
        {
            string absolute_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Replace("/", "\\"));
            string fullPath = System.IO.Path.GetFullPath((new Uri(absolute_path)).LocalPath);
            return fullPath;
        }

        public static Image GetRelativePathImage(string path)
        {
            return new Bitmap(GetFullPath(path));
        }

        public static Icon GetRelativePathIcon(string path)
        {
            try
            {
                return new Icon(GetFullPath(path));
            }
            catch
            {
                string base64 = "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AP+2NQD/tzYA/7c2AP+4NgD/uDYA/7g2AP+5NgD/uDYA/7s8AP/CQAD/x1AA/8JAAP/CPAD/wz8A/8M+AP/KUwD/xkgA/8RAAP/GQwD/x0QA/8dEAP/HRAD/xUEA/8VHAP/IVAD/wT8A/8A/AP++PQD/vDkD/9eQE/+1RQf/qywA/600AP+sNAD/qzQA/6o0AP+pNAD/qTQA/6g0AP+nNAD/pjQA/6U0AP+kNAD/ozMA/6IzAP+hMwD/oDMA/58zAP+fMwD/nTMA/5wzAP+cMwD/mjMA/5oyAP+ZMgD/mDIA/5cyAP+WMgD/lTIA/5QyAP+TMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+MMQD/izEA/4sxAJ0AAAAAizEADIsxAPWMMQD/jTEA/40xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lDIA/5UyAP+VMAD/q1sL/7RMB/+rNgD/tU4A/645AP+xPgD/sj8A/7RAAP+2QQD/t0IA/7hBAP+4QQD/tj8A/7U/AP+1PgD/tkIA/7dEAP+zOgD/szoA/7A1AP+2RAD/tkIA/7AzAP+wMgD/rzIA/68wAP+vMAD/ri8A/64uAP+uLQD/rSsA/6wqAP+sKQD/rioA/7ArAP+xLAD/sy4A/7UuAP+2LwD/uTQA/7o2AP+6NgD/uzYA/7s2AP+7NgD/uzYA/7s2AP+7NgD/wjkA/8hOAP/COAD/x0UA/8lNAP/FPAD/xj8A/8Y+AP/JSAD/zVQA/8hCAP/JRAD/yUUA/8pGAP/IRQD/x0QA/8U/AP/IUgD/xUcA/8A8AP/APwD/ujEA/81sDf/LeRD/qicA/681AP+uNAD/rTQA/6w0AP+rNAD/qjQA/6k0AP+oNAD/pzQA/6Y0AP+lNAD/pDQA/6MzAP+iMwD/oTMA/6AzAP+fMwD/njMA/50zAP+cMwD/mzMA/5oyAP+ZMgD/mDIA/5cyAP+XMgD/ljIA/5QyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+MMQD/izEA9YsxAAyLMQBSjDEA/40xAP+NMQD/jjEA/48xAP+QMQD/kTIA/5IyAP+TMgD/lDIA/5QyAP+WMgD/lzMA/5YxBP/Gdw//qC8A/7FGAP+vPQD/sD0A/7I9AP+zPgD/tT8A/7ZAAP+5QQD/uUIA/7lBAP+4QQD/t0AA/7c9AP+9TwD/tToA/7U7AP+0OAD/vE0A/7Q4AP+zNQD/szUA/7M0AP+zMwD/sjIA/7IxAP+xMAD/sS4A/7AuAP+wLAD/sCsA/7AqAP+xKgD/sysA/7UsAP+2LQD/ty4A/7gwAP+6MAD/uzAA/701AP++NgD/vjYA/782AP+/NgD/vzYA/8M4AP/EOgD/xDcA/8pKAP/JRgD/xjgA/81PAP/LSAD/yD0A/8pBAP/JQAD/z1UA/8xLAP/LQgD/zEYA/8tFAP/KRAD/yEQA/8ZBAP/HRwD/yVIA/8I8AP/APQD/vjkA/789BP/XkRP/tD4F/64vAP+vNAD/rjQA/600AP+sNAD/qzQA/6o0AP+pNAD/qDQA/6c0AP+mNAD/pTQA/6QzAP+jMwD/ojMA/6EzAP+fMwD/nzMA/54zAP+cMwD/nDMA/5oyAP+ZMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+MMQD/jDEAUowxAJeNMQD/jTEA/44xAP+PMQD/kDEA/5EyAP+SMgD/kzIA/5QyAP+VMgD/ljIA/5cyAP+SJgD/xoYS/6YrAP+sOQD/s0cA/645AP+xPAD/sz0A/7Q+AP+2PwD/uEAA/7lBAP+7QwD/u0MA/7tCAP+5PQD/v1MA/7c7AP+4PQD/uDwA/75NAP+2NgD/tzkA/7Y3AP+2NgD/tjUA/7Y0AP+1MgD/tDEA/7UwAP+1MAD/tC8A/7QtAP+zKQD/sygA/7ImAP+0JgD/tigA/7ovAP+9NgD/wDoA/8NAAP/EQgD/xD0A/8Q7AP/BNAD/wTIA/8I2AP/ENgD/xzgA/8c5AP/IOgD/yDkA/8o+AP/PUAD/yj0A/8xBAP/QUgD/zEEA/8xAAP/NQAD/z0kA/9JXAP/NRAD/zUYA/8xFAP/LRAD/yUMA/8hCAP/GPgD/ylEA/8VFAP/BOwD/wD0A/7kuAP/QdA//yG8O/60nAP+xNgD/rzQA/640AP+tNAD/rDQA/6s0AP+qNAD/qDQA/6g0AP+nNAD/pTQA/6Q0AP+jMwD/ojMA/6EzAP+gMwD/nzMA/54zAP+dMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+MMQCXjTEAxY0xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+XMgD/li4A/7JlDf+vQQb/qjUA/7RMAP+tNgD/sDoA/7I7AP+zPAD/tT0A/7c+AP+5QAD/ukIA/7xCAP+9QwD/vEEA/8BOAP+8QgD/uz8A/7o7AP/ATwD/uDcA/7k6AP+5OQD/uTgA/7g3AP+4NgD/uDUA/7g0AP+4MwD/uDEA/7gwAP+4MAD/ty0A/8FHAP/CRgD/wUIA/8NGAP/ERgD/wz4A/8I4AP/BNQD/wjQA/8M0AP/FOAD/yD4A/8xMAP/OUwD/yD4A/8YyAP/IOAD/yTkA/8o6AP/KOwD/yzwA/8s5AP/QSgD/0EwA/8w5AP/STQD/0k4A/88/AP/QRAD/0EEA/9RWAP/STwD/z0IA/85EAP/NQwD/y0IA/8lBAP/HPgD/yEUA/8lPAP/BOgD/wTwA/7w1AP/BQwb/15AT/7M2BP+xMgD/sDQA/680AP+uNAD/rTQA/6w0AP+rNAD/qTQA/6g0AP+nNAD/pjQA/6U0AP+kMwD/ojMA/6IzAP+hMwD/nzMA/54zAP+dMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAMeNMQDjjjEA/48xAP+QMQD/kTIA/5IyAP+TMgD/lDIA/5UyAP+WMgD/lzIA/5kzAP+bNwT/wW4O/6cvAP+wRAD/rjgA/685AP+xOgD/szsA/7Q8AP+1PQD/uD8A/7lAAP+8QQD/vUIA/79EAP+/RgD/wU0A/70/AP+8PAD/w1EA/7s6AP+8PAD/vDsA/7w6AP+7OQD/uzgA/7w3AP+7NgD/vDQA/7szAP+7MgD/vDEA/7oqAP/JVAD/uikA/74wAP+4HwD/uyYA/70nAP+/LAD/wC4A/8EuAP/CLwD/wzEA/8MyAP/EMwD/xDIA/8UwAP/LRQD/0VkA/8k6AP/INgD/yToA/8o7AP/LPAD/yz0A/8w9AP/NQAD/0lMA/89DAP/PQQD/1FYA/9FJAP/RQgD/0kMA/9NKAP/XWQD/0EQA/89DAP/OQwD/zEEA/8tAAP/IPwD/xjwA/8pPAP/FQQD/wTkA/8A8AP+5LQD/03wQ/8VkDP+uKAD/sjYA/7A0AP+vNAD/rjQA/600AP+rNAD/qjQA/6k0AP+oNAD/pzQA/6U0AP+kNAD/ozMA/6IzAP+hMwD/oDMA/58zAP+eMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA444xAPWPMQD/kDEA/5EyAP+SMgD/kzIA/5QyAP+VMgD/ljIA/5cyAP+YMgD/kyUA/8iHEf+kKAD/qzcA/7FCAP+tNgD/rzgA/7I5AP+zOgD/tTwA/7c9AP+5PwD/u0AA/7xBAP++QgD/v0AA/8ZXAP+/PwD/vz8A/8NNAP+/QQD/vz8A/789AP++OwD/vjsA/746AP++OQD/vzgA/782AP++NQD/vzQA/78yAP++LAD/y1QA/74lAP+/WQD/s04A/7dQAP/HOwD/vCQA/8AtAP/ALQD/wS4A/8EvAP/CMAD/wzIA/8QyAP/EMwD/xTQA/8Y1AP/FMQD/zk8A/9BTAP/INQD/yjsA/8o8AP/LPAD/zD0A/80/AP/NPAD/0EoA/9NSAP/OPQD/0ksA/9VXAP/RRAD/00YA/9JEAP/WVQD/1E4A/9BAAP/PQgD/zkEA/8tAAP/KPwD/yDsA/8hEAP/JTAD/wTgA/8A6AP+8MQD/w0oH/9aNE/+yMAP/sjQA/7E0AP+wNAD/rzQA/640AP+sNAD/qzQA/6o0AP+oNAD/qDQA/6Y0AP+lNAD/ozMA/6IzAP+hMwD/oDMA/58zAP+eMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAP+OMQDzjzEA95AxAP+RMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+XMgD/mDIA/5YsAP+4bg7/qjYE/6kyAP+zSAD/rDMA/683AP+xOAD/sjkA/7Q6AP+1PAD/uD0A/7k+AP+7PwD/vUAA/74/AP/GVQD/wkEA/8FDAP/DRgD/xUwA/8E/AP/BPwD/wT4A/8E9AP/BPAD/wTsA/8E5AP/COAD/wjcA/8I1AP/CNAD/wjAA/8tMAP/DLQD/vWIA/8JBAP/KPgD/tTkA/5U7AP/BVQD/xDAA/74pAP/ALgD/wS8A/8IwAP/DMQD/wzIA/8QzAP/FNAD/xjUA/8Y2AP/HNQD/yDkA/9JcAP/MRgD/yTcA/8s8AP/MPQD/zD4A/80/AP/NPwD/zkAA/9RVAP/RSQD/0EAA/9VVAP/VUAD/0kQA/9JFAP/TSQD/1lgA/9FDAP/QQQD/z0AA/8w/AP/LPgD/yT0A/8Y6AP/KTQD/xD4A/8E4AP+/OQD/uSwB/9WDEf/BWAr/sCoA/7M1AP+xNAD/rzQA/640AP+tNAD/rDQA/6o0AP+oNAD/qDQA/6Y0AP+lNAD/pDMA/6IzAP+hMwD/oDMA/58zAP+eMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA/48xAPeQMQD3kTIA/5IyAP+SMgD/lDIA/5UyAP+WMgD/lzIA/5gyAP+aMwD/nz8G/7xhDP+mLQD/sUIA/6w0AP+uNgD/rzYA/7E3AP+zOQD/tTsA/7c8AP+5PQD/uz4A/7w/AP++PwD/w0sA/8NGAP/DRAD/xEIA/8lWAP/CPwD/w0AA/8RAAP/EPwD/xD0A/8Q8AP/DOgD/xDcA/8U5AP/GNwD/xjYA/8U0AP/JQQD/yj0A/7pYAP/SSQD/zjcA/8opAP/IKAD/00QA/7BHAP+oVQD/ylIA/8AnAP/BLgD/wS8A/8IxAP/DMgD/xDIA/8UzAP/FNQD/xjUA/8c3AP/IOAD/yDQA/81IAP/SXAD/yjsA/8s7AP/MPgD/zT8A/84/AP/OQQD/zz8A/9FKAP/UVgD/0UEA/9JJAP/XWgD/00kA/9JFAP/RQgD/1FMA/9NOAP/PPwD/z0AA/84+AP/MPQD/yjwA/8g5AP/JRAD/yEkA/8E2AP/AOAD/uy0A/8ZUCf/ThhH/sSwC/7M1AP+yNQD/sDQA/680AP+tNAD/rDQA/6o0AP+pNAD/qDQA/6c0AP+lNAD/pDMA/6MzAP+iMwD/oDMA/58zAP+eMwD/nDMA/5wzAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kDEA95EyAPeSMgD/kjIA/5MyAP+UMgD/ljIA/5cyAP+YMgD/mTIA/5MnAf/IhBH/oiUA/6o1AP+vPAD/rDMA/681AP+wNgD/sjgA/7Q5AP+2OgD/uDsA/7k8AP+7PQD/vT8A/79AAP/FTwD/wkEA/8RBAP/MWAD/xUAA/8ZDAP/GQgD/x0EA/8ZAAP/GPgD/xjoA/8tJAP/PWAD/xzYA/8g4AP/INwD/yDkA/9BTAP+6TwD/01IA/8xHAP+rLAD/rzwA/8FMAP/KMAD/yikA/9FNAP+yWwD/v24A/8lCAP++JgD/wjIA/8MyAP/EMwD/xDQA/8U1AP/GNgD/xzcA/8c4AP/IOQD/yTsA/8k4AP/SWgD/0FIA/8o5AP/NQAD/zUAA/85BAP/PQgD/0EIA/9BCAP/VVgD/1E0A/9FAAP/VUwD/1lUA/9FDAP/RQwD/0UYA/9RWAP/PQgD/zj8A/84+AP/NPQD/yzwA/8k6AP/HOQD/ykwA/8M7AP/ANgD/vjUA/7ouA//XiRL/vk4I/7EsAP+zNQD/sTQA/680AP+uNAD/rDQA/6s0AP+qNAD/qDQA/6c0AP+mNAD/pDQA/6MzAP+iMwD/oDMA/58zAP+eMwD/nTMA/5wzAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD3kjIA95IyAP+TMgD/lDIA/5YyAP+XMgD/mDIA/5kyAP+WKgD/vXcQ/6YtA/+nLwD/skQA/6owAP+uMwD/sDQA/7I2AP+zOAD/tTkA/7c6AP+5OwD/uzwA/709AP++OwD/xlMA/8E+AP/EQQD/yVEA/8hHAP/IRQD/yEQA/8lDAP/JQQD/yUAA/8k9AP/MRgD/010A/8s9AP/KOwD/yToA/8c0AP/RWwD/yEEA/81tAP/MRwD/jQ4A/5kcAP+oOgD/slQA/8R2AP/QYgD/yCgA/840AP/NWAD/v3oA/852AP/EMwD/wSwA/8MzAP/ENAD/xTUA/8Y2AP/GNwD/xzgA/8g5AP/JOgD/yTsA/8k5AP/MQgD/1WIA/85GAP/MPAD/zkEA/89CAP/PQwD/0EQA/9BDAP/TSwD/11gA/9JFAP/SRgD/1lgA/9NLAP/QQgD/z0AA/9JPAP/STQD/zTwA/80/AP/NPQD/yzsA/8o7AP/HNgD/yEMA/8dFAP+/MwD/vzcA/7gpAP/IXQv/0X8R/7ApAf+zNgD/sTUA/680AP+uNAD/rTQA/6s0AP+qNAD/qDQA/6c0AP+mNAD/pDQA/6MzAP+iMwD/oDMA/58zAP+eMwD/nTMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5QyAP+UMgD/kzIA/5IyAPeSMgD3kzIA/5QyAP+VMgD/ljIA/5gyAP+ZMgD/mjIA/6VKB/+2VAv/pSsA/7BAAP+pLwD/rTIA/64zAP+wNQD/sjYA/7U3AP+2OAD/tzkA/7o6AP+8OwD/vToA/8ROAP/BPwD/xEIA/8ZGAP/KTgD/yUUA/8pGAP/KRAD/y0MA/8xCAP/MQQD/zUMA/9NZAP/PSQD/yzwA/8o8AP/HMwD/1WYA/81AAP+5dgD/vk4A/5QWAP+/PQD/00YA/9NFAP/ITwD/xHQA/9KTAP/YmwD/0FQA/8gmAP/QPgD/zmsA/9KZAP/SagD/wSsA/8MxAP/ENAD/xTUA/8Y2AP/HNwD/yDgA/8g6AP/JOgD/yjwA/8o9AP/KOAD/0VIA/9ReAP/NPgD/zkEA/89DAP/QRAD/0UUA/9FGAP/RRAD/1lgA/9VSAP/RQQD/00sA/9ZXAP/QQwD/z0EA/89CAP/TUwD/zkEA/8w8AP/MPAD/yzsA/8o6AP/HOAD/xjgA/8lKAP/BNgD/vzQA/7wyAP+6MQT/14wT/7pEBv+xLgD/sjUA/7A0AP+uNAD/rTQA/6w0AP+qNAD/qDQA/6c0AP+mNAD/pDQA/6MzAP+iMwD/oDMA/58zAP+eMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5QyAP+TMgD/kjIA95MyAPeUMgD/lTIA/5YyAP+XMgD/mDIA/5oyAP+WKgH/xn8R/6AiAP+pMwD/rDcA/6swAP+tMQD/rzMA/7E1AP+zNgD/tTcA/7c4AP+5OQD/uzoA/707AP/BQwD/w0YA/8M/AP/EPgD/zFYA/8hBAP/LRQD/zUYA/81FAP/NRAD/zkMA/85AAP/UVgD/01cA/8w7AP/MPQD/yTQA/9drAP/LMgD/v3kA/6xaAP+VIgD/x0AA/8E1AP+lKwD/rTAA/8c4AP/TQQD/03MA/9efAP/XoAD/2JAA/85DAP/JKQD/0kkA/9mNAP/epQD/zVEA/8EqAP/FNgD/xjYA/8Y3AP/HOAD/yDkA/8k6AP/JOwD/yjwA/8s9AP/LPQD/zD0A/9VhAP/SUgD/zT0A/9BEAP/QRAD/0UUA/9JGAP/SRQD/00wA/9dcAP/SRwD/0UIA/9RTAP/STgD/zz8A/849AP/QSgD/0EwA/8s6AP/MPAD/yzsA/8o5AP/JOAD/xTMA/81UAP/AMAD/vzQA/700AP+1JQD/ymYO/8x2EP+vKAD/szYA/7A0AP+uNAD/rTQA/6w0AP+qNAD/qDQA/6g0AP+mNAD/pDQA/6MzAP+iMwD/oDMA/58zAP+eMwD/nDMA/5szAP+aMgD/mTIA/5gyAP+WMgD/lTIA/5QyAP+TMgD3lDIA95QyAP+WMgD/lzIA/5gyAP+ZMgD/lSkA/8J+EP+hJQL/pSsA/68/AP+pLAD/rDAA/64yAP+wMwD/sjQA/7Q1AP+2NgD/uDcA/7o5AP+8OgD/vToA/8VOAP/CPAD/wzwA/8tVAP/IQAD/ykQA/8xEAP/ORgD/z0UA/9BEAP/QQAD/1FMA/9djAP/NOwD/zT8A/8o4AP/WZgD/zDsA/8NtAP+nWQD/mi0A/745AP+uNAD/gCAA/5QtAP+RMQD/kjsA/69FAP/NOAD/0koA/9eJAP/YoQD/2KIA/9aBAP/MNwD/yzAA/9RbAP/dowD/25cA/8g9AP/DLgD/xjgA/8c5AP/IOgD/yDsA/8k8AP/KPQD/yz0A/8s+AP/MPwD/zDwA/9BKAP/XZgD/0EYA/89BAP/RRQD/0UYA/9JHAP/TSAD/0kYA/9VWAP/UUgD/0EEA/9FGAP/UVQD/z0QA/809AP/NPwD/0VEA/80/AP/LOgD/yzsA/8o5AP/JNwD/xzYA/8pIAP/CNgD/vzMA/7wzAP+4LgD/uTUF/9aNE/+2OwX/sTAA/7A0AP+vNAD/rTQA/6w0AP+qNAD/qDQA/6c0AP+mNAD/pDQA/6IzAP+iMwD/oDMA/58zAP+dMwD/nDMA/5szAP+aMgD/mTIA/5cyAP+WMgD/lTIA/5QyAPeUMgD3ljIA/5cyAP+YMgD/mTIA/5kyAP+qUwn/sEgJ/6MpAP+uPgD/qCwA/6ovAP+tMAD/rzEA/7EzAP+zNAD/tTUA/7c2AP+5NwD/uzgA/7w2AP/GTwD/wDkA/8M8AP/JTAD/yEMA/8pDAP/MQwD/zkQA/9BFAP/SRgD/0kIA/9RRAP/ZaQD/zz0A/85BAP/NPQD/01kA/9BMAP/JYQD/nU0A/5s2AP+yLwD/uzwA/5wyAP/UTgD/1EYA/9JIAP+9TwD/n0cA/6ZYAP+/UAD/zzgA/9NaAP/YlwD/2KIA/9miAP/UbgD/zDEA/846AP/YbgD/364A/9iAAf/GMgD/xTQA/8g6AP/IOgD/yTsA/8o8AP/KPgD/yz4A/8w/AP/MQAD/zUIA/8w9AP/UWwD/1mAA/89AAP/RRgD/0kYA/9NHAP/TSAD/0kYA/9NIAP/VWAD/0k0A/849AP/SSwD/0lAA/809AP/MOwD/zkYA/9BLAP/JOAD/yjoA/8o5AP/JNwD/xjMA/8hBAP/FQAD/vjAA/7wzAP+6MwD/siMA/8xwEP/Hag7/rigA/7E1AP+vNAD/rTQA/6w0AP+qNAD/qDQA/6c0AP+lNAD/pDMA/6IzAP+hMwD/oDMA/58zAP+dMwD/nDMA/5ozAP+ZMgD/mDIA/5cyAP+WMgD/lTIA95UyAPeWMgD/lzIA/5kyAP+aMwD/mC0C/8N3EP+eIAD/qDIA/6oyAP+pLQD/qy4A/60wAP+vMQD/sjIA/7QzAP+2NAD/uDYA/7o3AP+8NwD/w0cA/8E8AP/DPAD/xkIA/8pKAP/JPwD/y0IA/81DAP/QRAD/0UUA/9NFAP/UTgD/2mkA/9FFAP/QQwD/z0AA/9FPAP/VXgD/zlQA/5pMAP+bOgD/pyYA/8lGAP+VLgD/3EsA/70yAP+9OwD/xzsA/9E3AP/TTQD/vlwA/69lAP+7cwD/ylIA/9A7AP/WbAD/2Z8A/9mjAP/anwD/0lwA/8wxAP/SRgD/2oQA/9+uAP/TZwD/xS8A/8c5AP/JOwD/yTwA/8o9AP/LPgD/yz8A/8xAAP/NQQD/zkIA/85CAP/PRQD/2GgA/9NTAP/QQQD/0kcA/9NIAP/TSAD/00cA/9FDAP/RSQD/1FUA/9JOAP/PRAD/0U8A/85CAP/MOQD/yzwA/89OAP/LPwD/yTcA/8k4AP/JNwD/xzQA/8U2AP/HRgD/vzMA/7wxAP+6MQD/tSoA/7k8B//VjRP/sjQE/68yAP+vNAD/rTQA/6s0AP+qNAD/qDQA/6c0AP+lNAD/pDMA/6IzAP+hMwD/nzMA/54zAP+dMwD/mzMA/5oyAP+ZMgD/mDIA/5cyAP+WMgD3ljIA95cyAP+YMgD/mTIA/5UoAP/EgRH/nR4B/6MpAP+sOwD/pyoA/6otAP+sLwD/ri8A/7ExAP+yMgD/tTMA/7c0AP+5NQD/uzYA/788AP/DQwD/wToA/8M6AP/LUQD/yDwA/8tAAP/NQQD/z0IA/9JDAP/TRAD/00kA/9lm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AP/HNAD/xjIA/8IwAP/AMwD/wkIA/7ozAP+zKQD/tDQA/7U8AP+rKQD/qCkA/6YoAP+iKAD/nyYA/50mAP+jNwD/mioA/5MgAPe0OwD3uEcA/7Q7AP+yNAD/ukcA/7EuAP+yLgD/sysA/7IpAP+zJwD/tCUA/7sxAP+9MAD/viwA/8ApAP/CJwD/zUIA/9iZAP/YjAD/2JIA/9iRAP/XjAD/6sJ5/////////////////////////////////+/Hv/++IAD/vh8A/70fAP+9IAD/vSQA/70nAP++KwD/vi4A/74xAP++NAD/14Jh//////////////////////////////////nu5P/SXQD/0GkA/8prAP/AUAD/xmQA/8hoAP/CVgD/0oEA/8xqAP/HRAD/y0MA/8tGAP/knnn/////////////////////////////////8su7/9lrAP/VgQD/0YAA/9KCAP/ShQD/04kA/9ONAP/UkQD/1ZcA/9STAP/XmwD/1ZMA/9WTAP/kukn//Pfw////////////////////////////////////////////////////////////////////////////////////////////////////////////+eni/9JgKf/FNAD/zEUA/9iJAP/ZkAD/yTgA/8UuAP/GMwD/xDIA/8AxAP+6LQD/vDgA/7s/AP+xLAD/rikA/7I7AP+qLQD/pikA/6MoAP+gJwD/nicA/5ghAP+fMwD/oDgA97dCAPe4RgD/szgA/7hEAP+0NwD/szIA/7MvAP+zLQD/syoA/7MmAP+7NwD/tiQA/781AP+6IgD/wCkA/8YuAP/ZmwD/14oA/9mNAP/VXAD/1V8A/9h+AP/rxXn/////////////////////////////////7sa//70fAP+9HwD/vB4A/7wgAP+8IwD/vCcA/7wqAP+9LQD/vTAA/70zAP/WgmH/////////////////////////////////+Ovk/8tbAP/SXwD/zHMA/8RcAP+/UAD/wlkA/8puAP/HZgD/x2YA/9OHAP/ObgD/x0QA/+Oaef/////////////////////////////////xzLv/yjwA/9BLAP/XcwD/1IkA/9GIAP/SiwD/044A/9ORAP/WmwD/05IA/9edAP/UkwD/2J4W//369P//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////9+Pa/8tFAf/KQQD/xzMA/9BXAP/bmwD/1HcA/8UqAP/DMQD/wDEA/7wxAP+5MAD/tS0A/7g8AP+zNwD/qSYA/7E6AP+mJwD/oygA/6AnAP+eJgD/oTEA/5srAP+TIgD3vFIA97Q6AP+2PwD/uEMA/7Q1AP+0MwD/szAA/7QuAP+zKAD/vDwA/7MiAP/DRQD/xEUA/9BfAP+8HQD/2ZcA/9aJAP/YjAD/1VgA/9JhAP/QTQD/zTcA/+SVef/////////////////////////////////wzb//uhgA/7seAP+7HQD/uiAA/7siAP+7JgD/uykA/7wsAP+8LgD/vDIA/9WBYf/////////////////////////////////58OT/znEA/9BaAP/RYwD/03sA/9JyAP/GYAD/vlQA/8JaAP/KbgD/x2YA/8hlAP/ThwD/57p5//////////////////////////////////DMu//IPwD/yD8A/8Y7AP/MTQD/1XYA/9SPAP/SkgD/05QA/9SWAP/UlwD/1JgA/9WYAP/px3f/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////235M/8czAP/NSgD/yj0A/8o4AP/VbQD/3KIA/8tXAP++JgD/vDIA/7owAP+2LwD/siwA/7AvAP+0PQD/rTIA/6guAP+jKQD/oSgA/54lAP+iMwD/lyMA/5UlAPe2PwD3tjsA/7xPAP+0NwD/tTcA/7Q0AP+0MQD/tC4A/7o6AP+1KQD/vj4A/8RIAP+/OAD/vy4A/9WJAP/ZnAD/1YYA/9l3AP/YkQD/1oMA/9iVAP/XkAD/6bl5//////////////////////////////////LPv//JRwD/uBgA/7odAP+5HwD/uiEA/7olAP+6KAD/uioA/7stAP+7MAD/1YBh//////////////////////////////////nv5P/QagD/z1UA/89aAP/PVwD/z1kA/9FdAP/SaAD/yWwA/79VAP/CWgD/y3EA/8NaAP/hq3n/////////////////////////////////78y7/8ZAAP/FQAD/xUAA/8VAAP/DOwD/yk4A/9N4AP/UlwD/0ZcA/9adAP/TlAD/2J8A//Hcpv/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////knHb/1GIA/8xGAP/JOQD/z00A/8k6AP/NQgD/2IQA/9eXAP+/PAD/tygA/7cwAP+yLgD/sC0A/6soAP+vOAD/qjEA/6QpAP+hKAD/nSQA/6M3AP+WIgD/liUA97Y+APe+UgD/tToA/7Y7AP+2OAD/tTUA/7UyAP+4NgD/tzMA/7o3AP/JWgD/tykA/8JBAP/OcAD/1p4A/9WLAP/XcQD/13UA/9BKAP/QSAD/0E0A/9NhAP/puXn/////////////////////////////////8s+//89QAP/DOAD/txcA/7geAP+5IQD/uSQA/7kmAP+5KgD/uSwA/7ovAP/Uf2H/////////////////////////////////+u3k/81MAP/OUwD/zlUA/85WAP/PWAD/z1gA/89YAP/SZQD/0XkA/8RfAP/AVQD/yGkA/96nef/////////////////////////////////vzLv/wkAA/8JAAP/CQAD/wkAA/8FAAP/BPwD/vzsA/8dOAP/TdgD/1Z0A/9abAP/WmQD/9ujF/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+vZef/ScgD/z0sA/9VmAP/KPQD/zEAA/85KAP/IOAD/zVMA/9iZAP/OgAD/tSoA/7ItAP+wLQD/rCwA/641AP+oLAD/pCkA/6EpAP+fKQD/oDAA/5kmAP+WJQD3vE8A97hDAP+3PwD/tjwA/7Y5AP+2NwD/tTQA/7s9AP+4OAD/y2QA/7QjAP/HUQD/xVcA/9WfAP/UkgD/02gA/9d6AP/PQgD/zD4A/8o5AP/JMAD/yz8A/+Wdef/////////////////////////////////02r//zEcA/85OAP/ANQD/tBYA/7cgAP+3IwD/tyYA/7goAP+4KwD/uS4A/9N9Yf/////////////////////////////////56+T/zUwA/81OAP/NUQD/zlMA/85UAP/PVQD/z1UA/89VAP/QVQD/ynEA/8BTAP/HZwD/36p5/////////////////////////////////+7Mu//BPwD/wUAA/8FAAP/BQAD/wUAA/8JAAP/CPwD/wDkA/9aWAP/apgD/1ZgA/9aZAP/368z/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////6rx5/9uyAP/ZtQD/0GAA/9JUAP/UYgD/yjcA/8xIAP/JRgD/xDoA/89oAP/YpgD/tDYA/7AvAP+sKgD/sTwA/6YoAP+kKQD/oCYA/6g5AP+bJQD/mSYA/5YlAPe8TgD3uEAA/7g/AP+3PQD/tzoA/7U0AP+9SAD/uDgA/8liAP+1LAD/ylwA/749AP/IjQD/1KIA/9BjAP/VfQD/0VkA/9mfAP/apgD/2aUA/9eRAP/QWwD/45d5//////////////////////////////////PYv//TagD/zEwA/8c5AP/ISgD/uy4A/7UfAP+2IwD/tigA/7cqAP+2KQD/1ohh//////////////////////////////////nr5P/LQgD/zUsA/81MAP/NTgD/zU8A/85RAP/OUgD/0FQA/9FVAP/NbgD/wlgA/8NbAP/jtXn/////////////////////////////////7su7/8FAAP/CQAD/wkAA/8JAAP/CQAD/wjwA/8RLAP/etAD/2qgA/9WaAP/XnQD/15oA//fszv/////////////////////////////////nrpT/2H44/9mKOP/Xhzj/1YM4/9N8OP/Tfjj/1oE4/8qHOP+3dDj/0m43//LEsP/////////////////////////////////loXn/zTgA/9mBAP/bvAD/2aYA/9BUAP/VXgD/0FkA/8Y4AP/JTgD/xEEA/8RDAP/GYQD/rigA/7Q/AP+pKQD/pysA/6MoAP+oNgD/oCoA/5woAP+ZJgD/liUA97hBAPe5QgD/uEAA/7c+AP+2OQD/vUwA/7Y2AP+8QgD/vkQA/8ZaAP+8NAD/u3gA/8KIAP/KWwD/04MA/81SAP/WkAD/0EwA/81DAP/PUAD/1HUA/9iiAP/r0nn/////////////////////////////////7tC//8NMAP/PZAD/xjcA/8c8AP/GOgD/yUsA/787AP+1KQD/tCMA/75DAP/Tf2H/////////////////////////////////+e/k/9JvAP/OUAD/zEQA/8xLAP/NTQD/zlAA/89TAP/QVgD/0VcA/9BgAP/QfQD/ym8A/+Csef/////////////////////////////////x1rv/wUIA/8E5AP/DQAD/w0AA/8E4AP/KaAD/37gA/9OYAP/UmQD/1pwA/9edAP/aowD/9+zO/////////////////////////////////+Oeef/ENAD/yEAA/8pZAP/DVwD/w1YA/8FPAP+9RwD/xFIA/8RTAP+VNgD/1rOl/////////////////////////////////+zfef/arQD/z1EA/9BDAP/blAD/3MEA/9eVAP/OTwD/02YA/8pNAP/DPQD/x08A/8FLAP+1PAD/qyoA/6osAP+lJwD/qzgA/6QuAP+eKAD/nCcA/5kmAP+WJgD3uEMA97lCAP+5QAD/uD8A/7pFAP+3PAD/vUkA/7YyAP+2LwD/uS0A/7NnAP+wbgD/xFgA/9CHAP/NYAD/1I8A/81JAP/IMwD/yDIA/8g3AP/KQAD/zEUA/+Wkef/////////////////////////////////wz7//wEQA/8dWAP/XjAD/zlUA/8Q5AP/HQQD/xDgA/8dDAP/CSAD/x1kA/9J+Yf/////////////////////////////////36eT/uT4A/8VhAP/RdAH/0FsA/85KAP/PTwD/z1QA/9FWAP/QWQD/y1sA/8dbAP/KbAD/5rt5//////////////////////////////////Xku//apgD/0ogA/8RLAP/BOwD/0ooA/+G7AP/YowD/1JoA/9SZAP/WmwD/15wA/9icAP/47s7/////////////////////////////////8NF5/9yJAP/MTgD/xjQA/8pDAP/FUAD/vkwA/71OAP+8RQD/ukQA/8ZTAP/sw6X/////////////////////////////////6Kl5/9uWAP/bwAD/2J4A/81DAP/TUwD/3KUA/9vBAP/RfwD/zFIA/9FrAP/DQgD/xUoA/7I3AP+sLQD/qSoA/7FCAP+pNQD/oCcA/54pAP+cKAD/mScA/5YmAPe3QgD3uUMA/7lBAP+4PwD/vUwA/7g9AP+4PAD/tzYA/7gvAP+xWgD/n1QA/7xVAP/OfwD/yWIA/9GLAP/LUAD/0HcA/9SKAP/ThwD/z2IA/8c5AP/EKwD/4Zd5//////////////////////////////////HQxv/QYDH/3q0x/9JgMf/XgDH/2H8x/9d8Mf/QYzH/0GIx/8ZWMf/OdTH/1ox8//////////////////////////////////jr5P+7RQD/uD0A/7tDAP/FYQD/0XkA/9RpAP/QUwD/zlMA/85iDP/Rdij/0X0w/9B/Mf/lupD/////////////////////////////////8+LC/9WVMf/bpTH/4rss/96wFv/XnwD/0ZAA/9WcAP/drgD/3KoA/9eeAP/XnAD/2J0A//fszv/////////////////////////////////vznn/4aMA/+SoAP/ciAD/zUsA/8g0AP/LPwD/yz8A/8xAAP/MPgD/zDoA/+eli//129L/9drS//Xa0v/129L/9t3S//jq0v/owGf/zkIA/9VbAP/cogD/3MIA/9iMAP/MOgD/1GcA/9y0AP/ZuwD/zW4A/8tZAP/MZAD/v0wA/64xAP+wPAD/pCgA/6UvAP+qPAD/nyoA/5snAP+ZJwD/lyYA97lGAPe5QwD/ukIA/7lBAP+8SgD/ukEA/7o/AP+2MwD/uE4A/4w5AP+0UgD/x2wA/8VlAP/OhAD/yFIA/9B/AP/NXgD/zVIA/9BfAP/TgAD/1JEA/9F6AP/kpXn/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+Ojk/8E1AP/ESAD/wE0A/7tGAP+9SQD/y3Md/+3JjP/359j//fv5/////////////////////////////////////////////////////////////////////////////v79//ny4v/w26f/3a0y/9adAP/cqwD/2KAA/9mhAP/ZngD/+OzO/////////////////////////////////+/Oef/hoQD/5KgA/+apAP/lpwD/2XgA/9BTAP/OSQD/zkgA/85GAP/QTQD/0lkA/9JjAP/WgQD/14cA/9RnAP/MOAD/zToA/9VsAP/fuwD/2ZAA/88/AP/YaQD/3KwA/9u/AP/SeAD/yDgA/9V9AP/avgD/1a4A/8hfAP/ObAD/rTEA/6krAP+mLAD/pCsA/58mAP+rQgD/nCkA/5knAP+WJgD3v1cA97c/AP+6QwD/ukIA/7g/AP++TAD/vUoA/7s2AP+aQwD/q04A/8JeAP+7YQD/y30A/8hiAP/NegD/yFEA/8I6AP/DNwD/xzsA/8pBAP/MTAD/z2kA/+jCef/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////46eT/wDMA/78wAP/BNAD/xUgA/851M//68+7////////////////////////////////////////////////////////////////////////////////////////////////////////////+/v3/6sxn/9abAP/YoAD/26UA/9qhAP/47c7/////////////////////////////////8dR5/+OoAP/ZdwD/z1EA/8k3AP/LNQD/zkMA/89SAP/SbQD/2JAA/9meAP/ZpQD/2KIA/9ihAP/YowD/2acA/9qgAP/VaQD/zTcA/88/AP/YegD/4r4A/9iBAP/PPwD/2HgA/9u1AP/auQD/zWQA/8Y+AP/WkQD/2sQA/9muAP+tMAD/qS4A/6QpAP+pNgD/qDkA/54pAP+bKAD/mSgA/5YnAPe4RAD3vVEA/7lBAP+6QgD/uUEA/7pBAP+9SAD/vUsA/6FKAP/CWQD/rlgA/8drAP/HdwD/yXMA/8ZVAP/BUwD/xE4A/8I3AP/BKAD/wy8A/8U2AP/HPQD/4Zh5//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////jp5P/AMgD/wTQA/8E0AP/BNAH/9+Te//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////7/3a0m/9ifAP/aoQD/26MA//jtzv/////////////////////////////////svXn/0UgA/8xBAP/JSgD/xF0A/79tAP/BfAD/x4wA/8+UAP/XoQD/2KMA/9mhAP/ZlAD/13oA/9VoAP/XdwD/2pkA/9mpAP/anQD/1WMA/844AP/RRwD/24gA/+K+AP/UcQD/zEQA/9eHAP/avQD/168A/8ZRAP/HSgD/2qUA/9SgAP+nJwD/rj8A/6UuAP+fKQD/nioA/5spAP+ZKAD/licA97dCAPe7SwD/vE0A/7lBAP+6QQD/uD4A/7g6AP/DZQD/vVQA/6FKAP+/WQD/wXsA/8RlAP/FWAD/tWsA/7Z2AP/EiQD/0psA/9GFAP/JSgD/wisA/8AoAP/fknn/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+Ojk/8EzAP/BNAD/wjUA/9JsRf/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////t04z/2aEA/9qhAP/boQD/+e/O/////////////////////////////////9yeef+pSwD/o1AA/6ZZAP+sXwD/tW4A/8F/AP/LigD/1YgA/9d0AP/TVwD/z0QA/808AP/NPwD/zkAA/84+AP/QQQD/12sA/9qdAP/aqgD/2poA/9VbAP/ONgD/0U4A/9yXAP/iuwD/z2EA/8pMAP/XlAD/2sEA/9OfAP/BQAD/0HUA/6w4AP+lKgD/pCwA/6ArAP+eKgD/mykA/5goAP+WJwD3t0MA97dBAP+7SwD/vlEA/7g/AP+6QAD/uTwA/8FfAP+XPgD/u1EA/7NuAP/AWwD/xmUA/65fAP+0bAD/wHEA/8aGAP/QnAD/0JUA/9OdAP/TlgD/zmwA/+Gaev/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////57eT/yUoA/8M3AP/BMQD/3I1u//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Tksf/bpQD/3akA/9+tAP/47c7/////////////////////////////////xZV4/5dAAP+lUwD/tGIA/8ZpAP/QYQD/0VAA/89CAP/NPQD/zT4A/85BAP/OQgD/z0MA/85BAP/OOwD/zzwA/9FDAP/OOQD/0UUA/9hxAP/anwD/2qsA/9qWAP/RVgD/yTcA/85WAP/epQD/4LQA/8pVAP/KVgD/16EA/9nCAP/RjwD/pisA/6YuAP+jLAD/oCwA/54qAP+bKQD/mCgA/5YoAPe2QQD3uEMA/7hCAP+6RAD/wFcA/7tGAP+/RQD/kz4A/7dMAP+kXQD/u08A/8N7AP+tWAD/sF4A/7o/AP+7OgD/vj0A/8FHAP/OfQD/05kA/9KaAP/RlQD/5MNh//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////jp5P/DNwD/yUoA/81WAP/ko4T/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////9unB/9umAP/bpAD/3akA//nvzv/////////////////////////////////Gjl//zl0A/9JMAP/RQwD/zT4A/80/AP/OQgD/zkMA/85DAP/OPQD/zjsA/9BEAP/SUAD/1mcA/9mEAP/ZlAD/2ZwA/9uOAP/VUQD/zzkA/9NIAP/ZdQD/2qEA/9mrAP/XkwD/zVAA/8Y5AP/OXgD/4LAA/96sAP/FSQD/yV8A/9mrAP/PmwD/oSEA/6ItAP+gLAD/nSoA/5spAP+YKQD/ligA97tSAPe5SAD/t0AA/7lCAP+5PwD/vkoA/6RIAP+vRAD/mEsA/7dMAP+2bQD/qUkA/6RUAP+4QAD/tjwA/7EoAP/EVAD/wEYA/742AP/ANAD/yVcA/9KMAP/SmAT/7tmi//79+///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+Ork/8M2AP/ENwD/wjAA/+arj//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////47s7/3asA/+CxAP/eqwD/+fDS////////////////////////////9ti9/9ZWB//MPwD/zkIA/85DAP/NQQD/zTwA/89CAP/RSwD/01cA/9ZuAP/XfgD/1YMA/9SIAP/ThwD/04gA/9aQAP/XlwD/154A/9urAP/dkQD/1U0A/845AP/RTAD/1nsA/9ijAP/YrAD/1Y4A/8hKAP/COQD/zGUA/+G4AP/ZmwD/vUEA/814AP+nNAD/oi0A/58sAP+dKwD/mioA/5gpAP+VKAD3tkEA97lIAP++UwD/vVEA/7tHAP+8RAD/r0wA/5JAAP+xSQD/qV0A/6pJAP+XRwD/uUMA/7E2AP+uKgD/vk4A/7UyAP++QgD/wkwA/8RJAP/DPQD/wjAA/8tpAP/PgAD/1ZwQ/9aiG//Vnhv/16Yb/9edG//Rcxv/yEcb/8Q6G//HRBv/x0cb/8VVG/+9UBv/u04b/8BTG//HXhv/yUYb/9uIb//////////////////////////////////46eT/xDcA/8M1AP/IQgD/0GAP/9RsG//SXhv/0mMb/9JtG//VfBv/03Ub/9V6G//Rbxv/xlwb/8l3G//Vohv/2a4b/9qpG//apRv/2qEb/9qkG//UoBr/4cJ5//////////////////////////////////jszv/drwD/26YA/+CvAP/cdhb/zFkb/5hBG/+MMhv/n0kb/9ZqFv/RRAD/zUIA/81BAP/PQwD/0lAA/9ReAP/UbQD/0m0A/9BrAP/PbQD/zmsA/9BxAP/tyZj/+/Pn//v05//79Of/+/Xn//v15//79uj/6s1x/9qqAP/dtQD/3JEA/9BLAP/KOwD/zlAA/9OAAP/VpAD/1qoA/9GGAP/CQgD/vTkA/854AP/mywD/2JoA/6g4AP+jLwD/nywA/50rAP+aKgD/mCkA/5UpAPe2QwD3tkMA/7dBAP+4QgD/u0kA/8RdAP+URAD/p0IA/59PAP+rSAD/ijoA/7hDAP+oJwD/sDcA/7lIAP+uLAD/sCsA/7IpAP+zJwD/ujQA/8RFAP/IUgD/w1gA/8NVAP/HXQD/zncA/9SQAP/TlwD/0ZQA/9KZAP/UmwD/0IAA/8ZGAP/AKQD/wCcA/8Q1AP/BPAD/uD4A/71DAP/CLQD/4Zhh//////////////////////////////////np5P/ENQD/ykQA/8VNAP/FUgD/vFAA/8NZAP/CVQD/w1UA/8NPAP/IXAD/ylwA/8xgAP/QaQD/0WMA/9BfAP/QZwD/0nUA/9WDAP/VgwD/1H8A/9FwAP/jp2v/////////////////////////////////9t7O/9ZzAP/dogD/1k0A/60+AP+BIAD/gR4A/7NNAP/XVAD/zUAA/85EAP/OPwD/01sA/89fAP/KUwD/yVIA/8lSAP/MWwD/zmMA/89pAP/QbwD/0XYA/+/Qpf/////////////////////////////////s0nn/3KwA/9yvAP/btAD/3bwA/9iNAP/KRgD/xDkA/8lRAP/RggD/06QA/9OnAP/MewD/ujkA/8ViAP/ftgD/054A/5sfAP+fLAD/nCsA/5oqAP+XKgD/lSkA97VCAPe2QwD/t0MA/7hDAP+6QgD/p0QA/5s4AP+YRAD/qkgA/38uAP+2RAD/piYA/6ktAP+3TAD/qisA/6spAP+tKQD/sCoA/7MqAP+1KAD/vDYA/851AP/CUQD/xFoA/8ZfAP/HYQD/yWUA/811AP/UiwD/1JkA/9KWAP/RlAD/1J8A/9SWAP/NagD/xDcA/8AoAP/DLgD/yUkA/8xQAP/ch2H/////////////////////////////////+erk/8lDAP/QcwD/w0gA/5kWAP+2QQD/tDgA/7pGAP+7RAD/v0wA/8NSAP/EUgD/yFoA/8lcAP/MXwD/zmYA/9FrAP/SbAD/z2AA/9BnAP/RZwD/0m4A/+Wra//////////////////////////////////x4M7/z1gA/9VqAP+PLAD/fhkA/5IxAP/RXQD/0UUA/85EAP/OQQD/0UwA/9JiAP/HQgD/yEQA/8pNAP/LUgD/zFgA/81eAP/OZQD/0GsA/9FxAP/TeAD/79Gk/////////////////////////////////+zSef/bqwD/2q0A/9qwAP/asgD/2rcA/9y6AP/UhQD/xEAA/743AP/EUAD/zoQA/9ClAP/RpQD/xW4A/7U5AP/JeAD/rEYA/54rAP+cLAD/misA/5cqAP+VKQD3tUsA97lIAP+4QgD/t0EA/7lHAP+YPgD/lT0A/6dFAP93IwD/s0EA/6IhAP+mKQD/qC8A/7tWAP+1SAD/rjIA/6woAP+uJwD/sSkA/7s/AP/KcAD/vk0A/8tvJf/Xjk//2JJP/9mVT//amE//2ppL/9KEHf/PfAD/1I8A/9ebAP/VmgD/0pUA/9KYAP/VoAD/04sA/8lPAP/NUQD/zE4A/9yFYf/////////////////////////////////57OT/z18A/9SdAP/CMwD/xWAs/7xjT//QgE//ynJP/9OET//PfUv/wlkZ/75LAP/CUQD/w1EA/8daAP/IWgD/y2IA/85lAP/QZgD/0WoA/9FoAP/RaQD/5Khr/////////////////////////////////+fUzv/LcgD/hSYA/4EeAP++dED/549P/9x7T//ef0//3nxP/+GLT//dik//1nJP/9l8T//agE//24RP/9yJT//cjU//3ZFP/96WT//fmk//4J9P/+GjT//z3Lr/////////////////////////////////69B5/9moAP/ZqwD/2a4A/9mxAP/YsQD/2bAA/9m0AP/btQD/0H4A/7w8AP+5NQD/wFIA/8uGAP/OpQD/zqIA/8FoAP+rQgD/ny0A/5wsAP+ZKwD/lyoA/5QqAPeXLwD3nDgA/6lGAP+2TQD/vFQA/5M8AP+gPwD/ch4A/65BAP+gHgD/oyQA/6QoAP+nLAD/pysA/644AP+4TgD/vlYA/7hEAP+8SAD/xmoA/7pLAP+/VAD/36l8/////////////////////////////fv4/9ykSP/QhAD/0YoA/9WTAP/ZnQD/2J4A/9SZAP/SlQD/1Z4A/9iZAP/QXgD/4Zdh//////////////////////////////////nu5P/NXQD/z3wA/6UjAP/ttpL////////////////////////////89/X/yW46/7pDAP/AUAD/wU8A/8NSAP/GWAD/yFkA/8tgAP/OZAD/0GgA/9JtAP/lpmv/////////////////////////////////+OHO/85gAP+PNQD/0l0A//ff0v/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////rz3n/2KcA/9ipAP/XrAD/2K8A/9ivAP/YrgD/2K4A/9etAP/YsQD/2rEA/8t2AP+2OAD/szUA/7xTAP/IiAD/zaYA/8GGAP+aJAD/nCwA/5krAP+WKwD/lCkA95kyAPeaMgD/mjEA/5ouAP+YPQD/mTsA/3UhAP+mOwD/nRwA/58hAP+iJQD/pSkA/6ctAP+qMgD/rTYA/685AP+xPAD/t0kA/8BdAP+3SQD/vVIA/79XAP/eqHn/////////////////////////////////8Niv/9GIAP/SjgD/05IA/9WSAP/YlAD/3JwA/92iAP/XnwD/05cA/9eWAP/fl2H/////////////////////////////////+e3k/8hXAP/QZwD/pzUA/9ihkP/////////////////////////////////kt5//sSsA/92lAP+zKgD/ukMA/79LAP/BTwD/xFQA/8VSAP/PYwD/y1oA/+Kia//////////////////////////////////z3M7/hyYA/91fAP/OQQD/9dbH/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+rOef/XpAD/16cA/9eqAP/XrQD/1q0A/9etAP/XrAD/16sA/9eqAP/XrQD/2a8A/9mrAP/GbgD/sDQA/64zAP+5VAD/ypYA/6M8AP+bLQD/mCwA/5YrAP+UKQD3mDIA95kyAP+aMwD/mzQA/5hAAP94JQD/oDsA/50aAP+dHQD/oCEA/6MmAP+lKgD/qC4A/6ozAP+tNwD/sDwA/7JAAP+1RQD/t0oA/7pPAP+8UwD/v1gA/96pef/////////////////////////////////z4cD/0ooA/9OQAP/VkwD/1pQA/9eVAP/ZlQD/2pUA/92aAP/hpQD/1H4A/+CbYf/////////////////////////////////47OT/wk0A/8VUAP/LZgD/16OQ//////////////////////////////////Pjsf/VmwD/3awA/9CCAP/MeAD/ym0A/810AP/PdwD/0oEA/9aOAP/TlgD/5sdr/////////////////////////////////+fVzv+7VgD/2moA/88/AP/wxan/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////58pw/9aiAP/VpQD/1agA/9WrAP/VqwD/1qwA/9WjAP/YsgD/16sA/8t6AP/VnAD/1qcA/9isAP/YqQD/wmwA/604AP+pMAD/slgA/5gnAP+YLAD/lisA/5QqAPeXMgD3mTIA/5gwAP+pSwD/fSoA/5k3AP+dGQD/mxoA/54eAP+hIgD/oycA/6YsAP+oMAD/qzQA/605AP+vPQD/skIA/7RHAP+4TAD/ulAA/7xWAP+/WwD/3qt5//////////////////////////////////Thv//SjAD/05IA/9WUAP/XlQD/2JYA/9qXAP/bmAD/3ZoA/9+bAP+6XQD/4Zhh//////////////////////////////////fq5P+/TAD/wEsA/8JNAP/pupD/////////////////////////////////8Muw/9NrAP/TeAD/1okA/9eUAP/YnAD/2Z8A/9mbAP/YmAD/140A/9V/AP/mqmv/////////////////////////////////+OPO/9tyAP/XcQD/2GgA/+GTdv/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////ftkD/1KAA/9SkAP/UpgD/1KwA/9e2AP/GXgD/vkEA/8RiAP/VowD/zogA/9WkAP/WpwD/2a0A/8uFAP+xQgD/tE4A/8FxAP+hNwD/mi0A/5csAP+WKwD/kyoA95cyAPeYMgD/mzIA/4UuAP+TNgD/nRsA/5oWAP+dGwD/nx8A/6IjAP+kKAD/py0A/6kxAP+rNgD/rTsA/7A/AP+yQwD/tUkA/7hOAP+6UwD/vFcA/79dAP/erHn/////////////////////////////////9OK//9OOAP/UlAD/1pYA/9eXAP/ZmAD/2pkA/9yaAP/dmwD/4J4A/7RLAP/ko2H/////////////////////////////////9+vk/71IAP+5RAD/vUgA/+OvkP/////////////////////////////////wxrD/zT4A/9x2AP/UWwD/0EkA/9BKAP/RSwD/0UoA/9FKAP/TVAD/0koA/+SVa//////////////////////////////////35s7/1WsA/9h1AP/ZawD/xy0S//vy7v//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////9uvO/9OcAP/TnwD/06EA/9KlAP/RkAD/w0sA/925AP/MhgD/ukUA/7MzAP++WQD/2K4A/8yIAP+xQAD/qTAA/7ZTAP+xSQD/rUMA/6lJAP+ZKwD/mCwA/5UrAP+TKgD3ljIA95cyAP+MLgD/izEA/54dAP+YEwD/mxgA/50cAP+gIAD/oiQA/6QpAP+nLgD/qTMA/6w3AP+uPAD/sEEA/7NGAP+1SgD/uE8A/7pVAP+9WgD/v18A/96tef/////////////////////////////////04r//05EA/9WXAP/WmAD/2JkA/9qaAP/bmwD/3ZwA/96dAP/ioQD/t2AA/+OQYf/////////////////////////////////05eT/szoA/89JAP/DTwD/47OQ//////////////////////////////////LgsP/XkAD/248A/9d0AP/UZAD/014A/9NeAP/VYgD/1msA/9h9AP/ZjgD/58Jr//////////////////////////////////fkzv/XcQD/2HIA/953AP/OQwD/3Xo3//rt5/////////////////////////////////////////////////////////////////////////////////////////////////////////////ft1v/YpCD/0poA/9GdAP/RnwD/0qIA/9KoAP/SowD/0aYA/8h4AP/RkAD/05wA/7lNAP+1SAD/qi4A/7dQAP+zSwD/rUAA/7NWAP+jOgD/misA/5ouAP+XLAD/lSwA/5EpAPeVMgD3lzAA/5JEAP+lNwD/nyMA/5wYAP+bFgD/nRsA/6AhAP+iJgD/pSoA/6cwAP+qNAD/rDkA/64+AP+xQgD/s0cA/7ZMAP+4UQD/ulYA/71cAP+/YQD/3q55//37+P/9+/j//fv4//37+P/9/Pj//vz4//Phvf/UkwD/1pkA/9eaAP/YmwD/2pwA/9udAP/dngD/354A/+OjAP/OkAD/4Zli//77+P/++/j//fr4//77+P/++/j//fr4//ry4v+eHAD/tUwA/81FAP/pp43//vr4//36+P/9+vj//fn4//36+P/9+vj/89Ou/9qKAP/ZmAD/2qUA/9usAP/csQD/3LEA/9ywAP/crgD/2qMA/9mWAP/ou23//vv4//77+P/++/j//vv4//78+P/++/j/9+HM/9xpAP/KbAD/yGAA/9dXAP/XWwD/1VER/+qsa//wwrD/9+DT//vw7P/9+vf//vz4//36+P/9+vj//fr4//37+P/9+/j//vv4//77+P/++/j//vv4//369v/78+j/9ujP/+/Wqf/gslb/0pMG/9CVAP/QmAD/0JoA/9GdAP/QoQD/0KMA/9GjAP/RpAD/zY0A/8yJAP+4TAD/rjQA/7dPAP+1TAD/r0AA/7VWAP+oPwD/nSsA/54xAP+bLwD/mS0A/5Yr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AP/FYwD/vVQA/8JhAP++TgD/uz8A/7tBAP+8QgD/u0MA/7tDAP+4QQD/tz4A/7hKAP+rRgD/nDEA/5wyAP+cMwD/mjIA/580AP+nOAD/pjgA/541AP+VMgD/lDIA/5MyAP+SMgD/kjIA/5ExAP+PMQD/jjEA940xAPeOMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+XMgD/mDIA/5kyAP+aMgD/mzMA/5wzAP+dMwD/njMA/58zAP+hMwD/ojMA/6MzAP+kMwD/pTQA/6c0AP+oMwD/pi0A/6s3AP+zRwD/vVoA/8tyAP+8PQD/uTEA/89zAP/QegD/y2gA/8lQAP/ATgD/tTAA/7k2AP+6NgD/uzYA/7w2AP++NgD/vzYA/78zAP+/MQD/wz0A/81XAP/VYQD/2F4A/9RMAP/dcgD/2GwA/9ZvAP/cfQD/1nIA/9dpAP/cVgD/wlQA/5A1AP+sVgD/0WMA/89TAP/NVgD/zF0A/81lAP/OagD/0XcA/92RAP/WigD/04sA/9Z/AP/UVgD/z0QA/807AP/OQQD/0UYA/9JTAP/XfQD/2qAA/96vAP/dogD/36UA/+KtAP/gsAD/1qUA/9SlAP/RjQD/y2QA/8lYAP/DTwD/vEAA/7g0AP+5OQD/uTkA/7g5AP+5OwD/tzUA/7k+AP+8RgD/uD0A/7g+AP+5QAD/uUEA/7lCAP+6QwD/uj8A/7xKAP+vSwD/njMA/50xAP+cMwD/nDMA/5oyAP+ZMgD/mDIA/5cyAP+XMgD/lTIA/5QyAP+TMgD/kjIA/5IyAP+RMgD/jzEA/44xAP+OMQD3jTEA940xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+XMgD/mDIA/5kyAP+aMgD/mzMA/5wzAP+dMwD/njMA/58zAP+gMwD/oTMA/6IzAP+kMwD/pTQA/6Y0AP+nNAD/qDQA/6k0AP+oLgD/rz0A/7xZAP/LdgD/wVMA/8BDAP/IXgD/tToA/7MyAP+0NQD/tjUA/7c2AP+4NgD/ujYA/7s2AP+7NgD/vDYA/702AP+/NgD/vjIA/78yAP/DPAD/zVcA/9+GAP/ZhAD/yFAA/8xeAP/WVQD/1lgA/5k0AP+MMAD/xF0A/89LAP/JSgD/y1MA/8xaAP/NYAD/zmYA/85sAP/QcQD/0HgA/9R9AP/VXAD/0EUA/807AP/NPQD/zD0A/8o9AP/JOgD/yDoA/8pBAP/MRgD/z2AA/96pAP/coQD/3qQA/9+fAP/emgD/yFsA/9OXAP/SpwD/zZUA/8huAP+1LwD/tjcA/7Y3AP+2OAD/tjkA/7Y6AP+2OwD/tjwA/7Y9AP+3PgD/tz8A/7dAAP+3QQD/tj4A/7xIAP+0TwD/oDUA/50vAP+eMwD/nDMA/5wzAP+aMgD/mTIA/5gyAP+YMgD/lzIA/5UyAP+UMgD/kzIA/5MyAP+SMgD/kTIA/48xAP+PMQD/jjEA/40xAPeMMQD3jTEA/40xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+XMgD/mDIA/5kyAP+aMgD/mjIA/5wzAP+cMwD/njMA/58zAP+gMwD/oTMA/6IzAP+jMwD/pDQA/6U0AP+nNAD/qDQA/6g0AP+pNAD/qS8A/6grAP+sNAD/sTwA/60wAP+wNAD/sTUA/7M1AP+zNQD/tDUA/7U1AP+2NQD/uDYA/7g2AP+6NgD/uzYA/7s2AP+8NgD/vTYA/742AP+9MAD/zlkA/9N+AP/IVAD/2lgA/7dJAP98GQD/pkMA/89MAP/HOgD/yEUA/8lLAP/KUgD/ylgA/8xeAP/MZQD/zWoA/9B0AP/TZAD/0EsA/8s9AP/KPAD/yjwA/8g8AP/IPAD/xzwA/8Y5AP/JRQD/yUwA/9SGAP/crAD/2Z8A/9+rAP/IVQD/yFgA/8VRAP/FVgD/vDwA/8RWAP/OggD/zZEA/7EsAP+zNgD/tDcA/7Q4AP+0OQD/tDkA/7Q6AP+0OwD/tTwA/7U9AP+1PgD/tDwA/7hEAP+2TgD/ojkA/50uAP+fMwD/nTMA/5wzAP+bMwD/mjIA/5kyAP+YMgD/lzIA/5YyAP+VMgD/lDIA/5MyAP+SMgD/kjIA/5EyAP+QMQD/jzEA/44xAP+NMQD/jTEA94sxAPeMMQD/jTEA/40xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+UMgD/lDIA/5YyAP+XMgD/mDIA/5gyAP+ZMgD/mjIA/5szAP+cMwD/nTMA/54zAP+fMwD/oDMA/6IzAP+iMwD/ozMA/6Q0AP+lNAD/pzQA/6g0AP+pNAD/qTQA/6s0AP+sNAD/rTQA/640AP+vNAD/sDQA/7E0AP+yNQD/szUA/7Q1AP+0NQD/tTUA/7Y1AP+3NgD/uDYA/7k2AP+6NgD/uzYA/7kvAP/NcgD/y08A/89YAP+NKgD/higA/8NMAP/IMgD/wzYA/8U+AP/GRQD/x0sA/8dRAP/IWAD/yVwA/8tmAP/QaQD/z1IA/8g/AP/HOgD/xzwA/8Y8AP/FOwD/xTwA/8M3AP/GQgD/yEkA/891AP/YqQD/1qAA/9efAP/ZnwD/2JsA/9qeAP+/RAD/vUMA/8RZAP/DVwD/uT4A/7IrAP++UQD/sTMA/7E1AP+xNgD/sTcA/7E4AP+yOQD/sjoA/7M7AP+yPAD/sjsA/7U/AP+2TQD/pTwA/50uAP+fMwD/njMA/50zAP+cMwD/mzMA/5oyAP+ZMgD/mDIA/5cyAP+WMgD/lTIA/5QyAP+TMgD/kjIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jTEA/40xAP+MMQD3ijEA94sxAP+MMQD/jTEA/40xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5IyAP+TMgD/lDIA/5UyAP+XMgD/lzIA/5gyAP+ZMgD/mjIA/5szAP+cMwD/nTMA/54zAP+fMwD/oDMA/6EzAP+iMwD/ozMA/6QzAP+lNAD/pjQA/6c0AP+oNAD/qTQA/6o0AP+rNAD/rDQA/600AP+uNAD/rjQA/680AP+wNAD/sTUA/7M1AP+zNQD/tDUA/7Q1AP+1NQD/tjUA/7c1AP+3NgD/tzMA/8lTAP+tRgD/eRsA/6lCAP/INQD/vyUA/8ExAP/CNgD/wz0A/8REAP/ESgD/xlAA/8dYAP/MYAD/z1sA/8U8AP/EOQD/xTwA/8Q7AP/DOwD/wzsA/8E3AP/DPwD/xkgA/8pjAP/TnwD/06IA/9SeAP/UnQD/1p0A/9acAP/WmgD/3KUA/8BOAP+7RAD/uUAA/7EsAP+8TgD/uUsA/64yAP+vNAD/tDgA/7g8AP+2PAD/sDgA/644AP+wOQD/rzoA/7I7AP+1SgD/qEAA/54vAP+gMwD/nzMA/54zAP+cMwD/mzMA/5oyAP+aMgD/mTIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+RMgD/kTEA/48xAP+PMQD/jjEA/40xAP+NMQD/jDEA/4sxAPeJMQD3ijEA/4sxAP+MMQD/jTEA/40xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5MyAP+TMgD/lDIA/5UyAP+WMgD/lzIA/5gyAP+ZMgD/mjIA/5oyAP+bMwD/nDMA/50zAP+fMwD/nzMA/6AzAP+hMwD/ojMA/6MzAP+kMwD/pTQA/6Y0AP+nNAD/qDQA/6k0AP+qNAD/qjQA/6s0AP+sNAD/rTQA/640AP+vNAD/rzQA/7A0AP+xNAD/sjUA/7M1AP+zNQD/tDUA/7Q1AP+6NwD/iSoA/4orAP/CPgD/vh4A/7wiAP++KgD/vzAA/782AP/APQD/wUQA/8NJAP/EUAD/xVcA/8lgAP/KXQD/ylUA/8RAAP/BPQD/vzcA/785AP+/PAD/w0cA/8ZWAP/OjAD/0JsA/8+ZAP/RnAD/0pwA/9ObAP/UnAD/1JoA/9qmAP/QhQD/x2gA/75QAP/HaQD/t0kA/6swAP+rLwD/qzMA/7M3AP+/QAD/x0YA/8RGAP/ESQD/vEIA/7E5AP+yRQD/q0IA/58xAP+gMgD/nzMA/54zAP+dMwD/nDMA/5szAP+aMgD/mTIA/5kyAP+YMgD/lzIA/5YyAP+VMgD/lDIA/5MyAP+SMgD/kjIA/5ExAP+PMQD/jjEA/44xAP+NMQD/jDEA/4sxAP+LMQD/ijEA94kxAPeJMQD/ijEA/4sxAP+MMQD/jTEA/40xAP+OMQD/jzEA/48xAP+RMgD/kjIA/5IyAP+TMgD/lDIA/5UyAP+WMgD/lzIA/5gyAP+ZMgD/mTIA/5oyAP+bMwD/nDMA/50zAP+eMwD/nzMA/58zAP+gMwD/oTMA/6IzAP+jMwD/pDMA/6U0AP+mNAD/pzQA/6g0AP+oNAD/qTQA/6o0AP+qNAD/rDQA/600AP+tNAD/rjQA/640AP+vNAD/sDQA/7E0AP+xNQD/sjUA/7Q1AP+xUAD/y00A/8E8AP+9KAD/uiIA/7olAP+8MAD/vjUA/748AP+/QgD/wUgA/8JPAP/CVAD/w1kA/8RgAP/GaAD/yW4A/8daAP/ETgD/v0EA/8NPAP/KegD/zZIA/82OAP/NlAD/zpkA/8+bAP/QmgD/0JgA/9akAP/QiwD/tTwA/64oAP+4RQD/t04A/6crAP+nLQD/qTAA/6kxAP+pMgD/tzoA/8dGAP/JSQD/01gA/8lMAP/ATgD/u08A/6AyAP+gMQD/nzMA/54zAP+dMwD/nTMA/5wzAP+aMwD/mjIA/5kyAP+YMgD/lzIA/5YyAP+WMgD/lDIA/5MyAP+TMgD/kjIA/5EyAP+QMQD/jzEA/44xAP+OMQD/jTEA/4wxAP+LMQD/izEA/4oxAP+JMQD3iDAA94kxAP+JMQD/ijEA/4sxAP+MMQD/jTEA/40xAP+OMQD/jjEA/48xAP+RMQD/kTIA/5IyAP+TMgD/lDIA/5QyAP+VMgD/ljIA/5cyAP+YMgD/mTIA/5kyAP+aMgD/mzMA/5wzAP+dMwD/njMA/58zAP+fMwD/oTMA/6EzAP+iMwD/ozMA/6QzAP+lNAD/pTQA/6c0AP+nNAD/qDQA/6g0AP+pNAD/qjQA/6s0AP+sNAD/rDQA/600AP+uNAD/rjQA/640AP+vNAD/rzIA/7pKAP/BVAD/wEYA/79CAP/AQQD/wUUA/703AP+8NgD/uzgA/75CAP++RwD/wE0A/8BUAP/CWQD/wl4A/8RkAP/EaAD/xnMA/8d1AP/IcgD/yoUA/8mDAP/KiAD/yo0A/8uSAP/MlgD/zJYA/9KiAP/OjQD/tkAA/60mAP+5SQD/tEoA/6UtAP+jKgD/pS4A/6YvAP+mMAD/pzEA/6cxAP+0OgD/xUUA/8dFAP/UXAD/yl4A/7ZAAP+xOgD/nzMA/58zAP+eMwD/nDMA/5wzAP+bMwD/mjIA/5kyAP+ZMgD/mDIA/5cyAP+WMgD/lTIA/5QyAP+TMgD/kjIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+MMQD/izEA/4sxAP+KMQD/iTEA/4gwAPeHMAD3iDAA/4gwAP+JMQD/ijEA/4sxAP+MMQD/jDEA/40xAP+OMQD/jjEA/48xAP+QMQD/kTIA/5IyAP+TMgD/kzIA/5QyAP+VMgD/ljIA/5cyAP+YMgD/mDIA/5kyAP+aMgD/mjIA/5wzAP+cMwD/nTMA/54zAP+fMwD/nzMA/6AzAP+iMwD/ojMA/6MzAP+jMwD/pDQA/6U0AP+mNAD/pzQA/6c0AP+oNAD/qDQA/6k0AP+qNAD/qjQA/6s0AP+sNAD/rDQA/600AP+tNAD/rTIA/6wwAP+xPAD/uU0A/79UAP+8QwD/uz4A/7xAAP/ATAD/vkcA/71JAP+9SQD/v1IA/79YAP/AXQD/wWMA/8JoAP/CbAD/xncA/8V4AP/GfAD/x4EA/8iGAP/JiwD/yY4A/86eAP/MkQD/tkUA/6okAP+4SQD/tE0A/6AnAP+gJwD/oywA/6MtAP+jLgD/pC8A/6QvAP+kMAD/pDEA/6kyAP+6QQD/xFUA/71HAP+7QQD/sj0A/6c3AP+dMgD/nDMA/5wzAP+bMwD/mjIA/5kyAP+ZMgD/mDIA/5cyAP+XMgD/lTIA/5QyAP+UMgD/kzIA/5IyAP+RMgD/kTIA/5AxAP+PMQD/jjEA/40xAP+NMQD/jDEA/4sxAP+LMQD/ijEA/4kxAP+IMAD/iDAA94YwAPeHMAD/iDAA/4gwAP+JMQD/ijEA/4sxAP+LMQD/jDEA/40xAP+NMQD/jjEA/48xAP+QMQD/kTIA/5EyAP+SMgD/kzIA/5MyAP+UMgD/ljIA/5YyAP+XMgD/mDIA/5gyAP+ZMgD/mjIA/5ozAP+bMwD/nDMA/50zAP+eMwD/nzMA/58zAP+gMwD/oTMA/6IzAP+iMwD/ozMA/6QzAP+kNAD/pTQA/6Y0AP+nNAD/pzQA/6g0AP+oNAD/qTQA/6k0AP+qNAD/qjQA/6o0AP+rNAD/qzQA/6w0AP+sMQD/qzAA/687AP+3TAD/vVQA/7lCAP+4OwD/uD8A/75TAP+/VgD/v1oA/75bAP+/YAD/xHAA/8FlAP+3QQD/vlgA/8NxAP/HgwD/xoYA/8qRAP/KjQD/tUkA/6kkAP+0RQD/t1IA/6o4AP+pNwD/rEEA/6Y3AP+hLwD/nigA/6AtAP+hLgD/oS8A/6AtAP+kMgD/qkMA/6g8AP+nMwD/pTYA/58zAP+eMwD/nDIA/5wzAP+bMwD/mjIA/5oyAP+ZMgD/mDIA/5cyAP+XMgD/ljIA/5UyAP+UMgD/kzIA/5IyAP+SMgD/kTIA/5ExAP+PMQD/jzEA/44xAP+NMQD/jTEA/4wxAP+LMQD/ijEA/4kxAP+JMQD/iDAA/4cwAP+HMAD3hjAA94YwAP+HMAD/iDAA/4gwAP+JMQD/ijEA/4sxAP+LMQD/jDEA/40xAP+NMQD/jjEA/48xAP+PMQD/kDEA/5EyAP+SMgD/kjIA/5MyAP+UMgD/lTIA/5YyAP+WMgD/lzIA/5gyAP+ZMgD/mTIA/5oyAP+aMwD/nDMA/5wzAP+dMwD/njMA/54zAP+fMwD/oDMA/6EzAP+hMwD/ojMA/6IzAP+jMwD/pDMA/6Q0AP+lNAD/pjQA/6Y0AP+nNAD/pzQA/6g0AP+oNAD/qDQA/6k0AP+pNAD/qTQA/6o0AP+qNAD/qjQA/6oyAP+pLwD/rToA/7RLAP+6VAD/tUAA/7Q5AP+0PAD/u1UA/8JpAP+xOQD/ri4A/68zAP+tLwD/rS8A/681AP++ZwD/tU4A/6ckAP+zQwD/t1IA/6o5AP+lLwD/pzQA/6Y0AP+kMQD/pTUA/6c6AP+pQQD/pTsA/6AxAP+fLgD/qEAA/6U8AP+eMAD/nzMA/54yAP+dMgD/nTMA/5syAP+aMwD/mTIA/5oyAP+ZMgD/mDIA/5gyAP+XMgD/ljIA/5UyAP+UMgD/lDIA/5MyAP+SMgD/kjIA/5EyAP+QMQD/jzEA/44xAP+OMQD/jTEA/40xAP+MMQD/izEA/4oxAP+JMQD/iTEA/4gwAP+HMAD/hzAA/4YwAPeFMAD3hjAA/4YwAP+HMAD/hzAA/4gwAP+JMQD/iTEA/4oxAP+LMQD/jDEA/40xAP+NMQD/jjEA/44xAP+PMQD/kDEA/5EyAP+SMgD/kjIA/5MyAP+TMgD/lDIA/5UyAP+WMgD/lzIA/5cyAP+YMgD/mTIA/5kyAP+aMgD/mjMA/5szAP+cMwD/nTMA/50zAP+eMwD/nzMA/58zAP+gMwD/oTMA/6EzAP+iMwD/ojMA/6MzAP+jMwD/pDQA/6Q0AP+lNAD/pTQA/6Y0AP+nNAD/pzQA/6c0AP+nNAD/qDQA/6g0AP+oNAD/qDQA/6g0AP+oNAD/qDIA/6cvAP+rOQD/sUkA/7hVAP+yQQD/rzYA/6svAP+wPAD/uE8A/644AP+oKwD/qzIA/6kuAP+wPwD/tVIA/6k6AP+kLwD/pTQA/6U0AP+kNAD/pDMA/6MzAP+jMwD/ojMA/6EwAP+hMgD/ozgA/6U8AP+eMQD/nzMA/54zAP+dMwD/nDMA/5wzAP+aMgD/pDcA/7E9AP+hNQD/mDEA/5gyAP+XMgD/ljIA/5YyAP+VMgD/lDIA/5MyAP+SMgD/kjIA/5EyAP+QMQD/jzEA/48xAP+OMQD/jTEA/40xAP+MMQD/izEA/4sxAP+KMQD/iTEA/4kxAP+IMAD/hzAA/4cwAP+GMAD/hTAA94QwAPeFMAD/hTAA/4YwAP+HMAD/hzAA/4gwAP+IMAD/iTEA/4oxAP+LMQD/izEA/4wxAP+NMQD/jTEA/44xAP+OMQD/jzEA/5AxAP+RMgD/kjIA/5IyAP+TMgD/kzIA/5QyAP+VMgD/ljIA/5cyAP+XMgD/mDIA/5kyAP+ZMgD/mjIA/5oyAP+bMwD/nDMA/5wzAP+dMwD/njMA/54zAP+fMwD/nzMA/6AzAP+gMwD/oTMA/6IzAP+iMwD/ojMA/6MzAP+jMwD/ozMA/6QzAP+kNAD/pDQA/6U0AP+lNAD/pjQA/6Y0AP+mNAD/pjQA/6c0AP+nNAD/pzQA/6c0AP+mMwD/pC4A/6k5AP+uQwD/u1kA/8NvAP/AZwD/qjUA/8FrAP+tPQD/s0wA/6g8AP+iMAD/pDQA/6MzAP+jMwD/ojMA/6IzAP+iMwD/oTMA/6EzAP+gMwD/oDMA/58zAP+fMwD/njMA/50zAP+dMwD/nDMA/5szAP+bMwD/mjIA/5kyAP+1PwD/wkYA/7E+AP+WMQD/ljIA/5YyAP+VMgD/lDIA/5MyAP+TMgD/kjIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+NMQD/jDEA/4sxAP+KMQD/ijEA/4kxAP+IMAD/iDAA/4cwAP+GMAD/hjAA/4UwAP+EMAD3gzAA94QwAP+EMAD/hTAA/4YwAP+GMAD/hzAA/4gwAP+IMAD/iTEA/4oxAP+KMQD/izEA/4wxAP+MMQD/jTEA/40xAP+OMQD/jzEA/48xAP+RMQD/kTIA/5IyAP+SMgD/kzIA/5QyAP+UMgD/lTIA/5YyAP+XMgD/lzIA/5gyAP+YMgD/mTIA/5oyAP+aMgD/mjIA/5szAP+cMwD/nDMA/50zAP+dMwD/njMA/58zAP+fMwD/nzMA/6AzAP+gMwD/oTMA/6EzAP+iMwD/ojMA/6IzAP+iMwD/ozMA/6MzAP+jMwD/ozMA/6MzAP+kMwD/pDMA/6Q0AP+kNAD/pDQA/6Q0AP+kNAD/pDMA/6IvAP/FegD/r0UA/7ZUAP+tQgD/vGQA/7JRAP+kNgD/ozMA/6IzAP+iMwD/ojMA/6EzAP+hMwD/oDMA/6AzAP+fMwD/nzMA/58zAP+eMwD/nTMA/5wzAP+cMwD/mzMA/5szAP+aMgD/mjIA/5kyAP+ZMgD/lzEA/6Y5AP+2QAD/ozcA/5UxAP+VMgD/lDIA/5QyAP+TMgD/kjIA/5IyAP+RMgD/kDEA/48xAP+OMQD/jjEA/40xAP+NMQD/jDEA/4wxAP+LMQD/ijEA/4kxAP+JMQD/iDAA/4gwAP+HMAD/hjAA/4YwAP+FMAD/hDAA/4QwAPeDMAD3gzAA/4QwAP+EMAD/hTAA/4YwAP+GMAD/hzAA/4gwAP+IMAD/iTEA/4kxAP+KMQD/izEA/4sxAP+MMQD/jTEA/40xAP+OMQD/jjEA/48xAP+QMQD/kTEA/5EyAP+SMgD/kjIA/5MyAP+UMgD/lDIA/5UyAP+VMgD/ljIA/5cyAP+XMgD/mDIA/5kyAP+ZMgD/mjIA/5oyAP+aMgD/mzMA/5szAP+cMwD/nTMA/50zAP+eMwD/njMA/58zAP+fMwD/nzMA/6AzAP+gMwD/oDMA/6AzAP+hMwD/oTMA/6EzAP+iMwD/ojMA/6IzAP+iMwD/ojMA/6IzAP+iMwD/ojMA/6IzAP+iMwD/oC0A/7VcAP+sRwD/rkkA/7ZfAP+jNgD/ny8A/6EzAP+gMwD/oDMA/6AzAP+fMwD/nzMA/58zAP+eMwD/njMA/50zAP+dMwD/nDMA/5wzAP+bMwD/mzMA/5oyAP+aMgD/mjIA/5kyAP+YMgD/mDIA/5cxAP+aNAD/qToA/6k7AP+aNAD/kzEA/5MyAP+TMgD/kjIA/5IyAP+RMgD/kDEA/48xAP+PMQD/jjEA/44xAP+NMQD/jTEA/4wxAP+LMQD/izEA/4oxAP+JMQD/iTAA/4gwAP+HMAD/hzAA/4YwAP+GMAD/hTAA/4QwAP+EMAD/gzAA94IwAPeDMAD/gzAA/4QwAP+EMAD/hTAA/4YwAP+GMAD/hzAA/4cwAP+IMAD/iDAA/4kxAP+KMQD/ijEA/4sxAP+MMQD/jDEA/40xAP+NMQD/jjEA/44xAP+PMQD/kDEA/5EyAP+RMgD/kjIA/5IyAP+TMgD/lDIA/5QyAP+VMgD/lTIA/5YyAP+WMgD/lzIA/5gyAP+YMgD/mTIA/5kyAP+aMgD/mjIA/5ozAP+bMwD/mzMA/5wzAP+cMwD/nDMA/50zAP+dMwD/njMA/54zAP+eMwD/nzMA/58zAP+fMwD/nzMA/58zAP+gMwD/oDMA/6AzAP+gMwD/oDMA/6AzAP+gMwD/oDMA/6AzAP+gMwD/nzAA/58xAP+eLwD/nS4A/58zAP+fMwD/nzMA/58zAP+eMwD/njMA/54zAP+dMwD/nTMA/5wzAP+cMwD/nDMA/5szAP+bMwD/mjIA/5oyAP+ZMgD/mTIA/5gyAP+YMgD/lzIA/5cyAP+WMgD/lTEA/6g7AP+nOwD/pjsA/6c7AP+SMQD/kTEA/5ExAP+QMQD/kDEA/5AxAP+PMQD/jjEA/44xAP+NMQD/jTEA/4wxAP+LMQD/izEA/4oxAP+JMQD/iTEA/4gwAP+IMAD/hzAA/4YwAP+GMAD/hTAA/4QwAP+EMAD/gzAA/4MwAP+CMAD3gTAA94IwAP+CMAD/gzAA/4MwAP+EMAD/hDAA/4UwAP+GMAD/hjAA/4cwAP+IMAD/iDAA/4kxAP+JMQD/ijEA/4sxAP+LMQD/jDEA/40xAP+NMQD/jTEA/44xAP+OMQD/jzEA/5AxAP+RMQD/kTIA/5IyAP+SMgD/kzIA/5MyAP+UMgD/lDIA/5UyAP+WMgD/ljIA/5cyAP+XMgD/mDIA/5gyAP+YMgD/mTIA/5kyAP+aMgD/mjIA/5oyAP+bMwD/mzMA/5szAP+cMwD/nDMA/5wzAP+cMwD/nTMA/50zAP+dMwD/njMA/54zAP+eMwD/njMA/54zAP+eMwD/njMA/54zAP+eMwD/njMA/54zAP+eMwD/njMA/54zAP+dMwD/nTMA/50zAP+dMwD/nDMA/5wzAP+cMwD/nDMA/5szAP+bMwD/mjMA/5oyAP+aMgD/mjIA/5kyAP+ZMgD/mDIA/5gyAP+XMgD/lzIA/5YyAP+WMgD/lTIA/5UyAP+TMQD/pjoA/6Y7AP+lOwD/pjsA/5AxAP+hOQD/tUIA/68/AP+UMwD/jjAA/44xAP+NMQD/jTEA/4wxAP+MMQD/izEA/4oxAP+KMQD/iTEA/4gwAP+IMAD/hzAA/4cwAP+GMAD/hjAA/4UwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAPeBLwD3gTAA/4IwAP+CMAD/gzAA/4MwAP+EMAD/hDAA/4UwAP+GMAD/hjAA/4cwAP+HMAD/iDAA/4gwAP+JMQD/iTEA/4oxAP+LMQD/izEA/4wxAP+NMQD/jTEA/40xAP+OMQD/jjEA/48xAP+QMQD/kDEA/5EyAP+SMgD/kjIA/5IyAP+TMgD/lDIA/5QyAP+UMgD/lTIA/5YyAP+WMgD/lzIA/5cyAP+XMgD/mDIA/5gyAP+YMgD/mTIA/5kyAP+aMgD/mjIA/5oyAP+aMgD/mjMA/5szAP+bMwD/mzMA/5szAP+cMwD/nDMA/5wzAP+cMwD/nDMA/5wzAP+cMwD/nDMA/5wzAP+cMwD/nDMA/5wzAP+cMwD/nDMA/5szAP+bMwD/mzMA/5szAP+aMwD/mjIA/5oyAP+aMgD/mjIA/5oyAP+ZMgD/mTIA/5gyAP+YMgD/mDIA/5cyAP+XMgD/ljIA/5YyAP+VMgD/lTIA/5QyAP+UMgD/lDIA/5MxAP+WMwD/pjsA/6Y7AP+XNAD/lTMA/8JGAP/hVAD/2lEA/64+AP+MMAD/jTEA/40xAP+MMQD/izEA/4sxAP+KMQD/iTEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hjAA/4UwAP+EMAD/hDAA/4QwAP+DMAD/gjAA/4IwAP+CMAD/gS8A94AvAPeBLwD/gS8A/4EwAP+CMAD/gjAA/4MwAP+DMAD/hDAA/4UwAP+FMAD/hjAA/4YwAP+HMAD/hzAA/4gwAP+IMAD/iTEA/4oxAP+KMQD/izEA/4sxAP+MMQD/jTEA/40xAP+NMQD/jjEA/44xAP+PMQD/jzEA/5AxAP+RMgD/kTIA/5IyAP+SMgD/kjIA/5MyAP+UMgD/lDIA/5QyAP+VMgD/lTIA/5YyAP+WMgD/lzIA/5cyAP+XMgD/mDIA/5gyAP+YMgD/mTIA/5kyAP+ZMgD/mTIA/5kyAP+aMgD/mjIA/5oyAP+aMgD/mjIA/5oyAP+aMgD/mjIA/5oyAP+aMgD/mjIA/5oyAP+aMgD/mjIA/5oyAP+aMgD/mjIA/5oyAP+aMgD/mTIA/5kyAP+ZMgD/mTIA/5gyAP+YMgD/mDIA/5gyAP+XMgD/lzIA/5cyAP+WMgD/ljIA/5UyAP+UMgD/lDIA/5QyAP+TMgD/kzIA/5IyAP+SMgD/kjIA/5ExAP+QMQD/jzEA/44wAP+XNAD/yksA/+NVAP/hVAD/tUEA/4wwAP+MMQD/izEA/4sxAP+KMQD/iTEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hjAA/4YwAP+FMAD/hDAA/4QwAP+DMAD/gzAA/4IwAP+CMAD/gTAA/4EvAP+ALwD3gC8A94AvAP+ALwD/gS8A/4EwAP+CMAD/gjAA/4MwAP+DMAD/hDAA/4QwAP+FMAD/hTAA/4YwAP+HMAD/hzAA/4gwAP+IMAD/iTEA/4kxAP+KMQD/ijEA/4sxAP+LMQD/jDEA/40xAP+NMQD/jTEA/44xAP+OMQD/jzEA/48xAP+PMQD/kDEA/5EyAP+RMgD/kjIA/5IyAP+SMgD/kzIA/5MyAP+UMgD/lDIA/5QyAP+UMgD/lTIA/5YyAP+WMgD/ljIA/5cyAP+XMgD/lzIA/5cyAP+YMgD/mDIA/5gyAP+YMgD/mDIA/5gyAP+ZMgD/mTIA/5kyAP+ZMgD/mTIA/5kyAP+ZMgD/mTIA/5kyAP+ZMgD/mDIA/5gyAP+YMgD/mDIA/5gyAP+YMgD/mDIA/5cyAP+XMgD/lzIA/5YyAP+WMgD/ljIA/5YyAP+VMgD/lDIA/5QyAP+UMgD/lDIA/5MyAP+TMgD/kjIA/5IyAP+SMgD/kTIA/5EyAP+QMQD/jzEA/48xAP+PMQD/jjEA/44xAP+vPwD/zEsA/8RIAP+eOAD/ijAA/4sxAP+KMQD/iTEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hjAA/4YwAP+FMAD/hTAA/4QwAP+EMAD/gzAA/4MwAP+CMAD/gjAA/4EvAP+BLwD/gC8A/4AvAPd/LwD3fy8A/4AvAP+ALwD/gS8A/4EvAP+CMAD/gjAA/4IwAP+DMAD/hDAA/4QwAP+EMAD/hTAA/4YwAP+GMAD/hzAA/4cwAP+IMAD/iDAA/4kxAP+JMQD/ijEA/4oxAP+LMQD/izEA/4wxAP+MMQD/jTEA/40xAP+OMQD/jjEA/44xAP+PMQD/jzEA/5AxAP+QMQD/kTIA/5EyAP+SMgD/kjIA/5IyAP+SMgD/kzIA/5MyAP+UMgD/lDIA/5QyAP+UMgD/lTIA/5UyAP+VMgD/ljIA/5YyAP+WMgD/ljIA/5YyAP+XMgD/lzIA/5cyAP+XMgD/lzIA/5cyAP+XMgD/lzIA/5cyAP+XMgD/lzIA/5cyAP+XMgD/lzIA/5cyAP+WMgD/ljIA/5YyAP+WMgD/ljIA/5UyAP+VMgD/lTIA/5QyAP+UMgD/lDIA/5QyAP+TMgD/kzIA/5IyAP+SMgD/kjIA/5IyAP+RMgD/kTIA/5AxAP+PMQD/jzEA/48xAP+OMQD/jjEA/40xAP+NMQD/jDAA/40xAP+WNQD/kzQA/4kwAP+KMQD/iTEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hzAA/4YwAP+FMAD/hTAA/4QwAP+EMAD/gzAA/4MwAP+CMAD/gjAA/4EwAP+BLwD/gS8A/4AvAP+ALwD/fy8A938vAPd/LwD/fy8A/4AvAP+ALwD/gC8A/4EvAP+BMAD/gjAA/4IwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4UwAP+GMAD/hjAA/4cwAP+HMAD/iDAA/4gwAP+JMQD/iTEA/4kxAP+KMQD/izEA/4sxAP+LMQD/jDEA/40xAP+NMQD/jTEA/44xAP+OMQD/jjEA/48xAP+PMQD/jzEA/5AxAP+RMgD/kTIA/5EyAP+SMgD/kjIA/5IyAP+SMgD/kzIA/5MyAP+TMgD/kzIA/5QyAP+UMgD/lDIA/5QyAP+UMgD/lTIA/5UyAP+VMgD/lTIA/5UyAP+VMgD/lTIA/5UyAP+VMgD/lTIA/5UyAP+VMgD/lTIA/5UyAP+VMgD/lTIA/5QyAP+UMgD/lDIA/5QyAP+UMgD/lDIA/5MyAP+TMgD/kzIA/5MyAP+SMgD/kjIA/5IyAP+SMgD/kTIA/5EyAP+QMQD/kDEA/48xAP+PMQD/jzEA/44xAP+OMQD/jjEA/40xAP+NMQD/jDEA/4wxAP+LMQD/izEA/4owAP+JMAD/ijEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hzAA/4YwAP+GMAD/hTAA/4QwAP+EMAD/hDAA/4MwAP+CMAD/gjAA/4IwAP+BLwD/gS8A/4AvAP+ALwD/fy8A/38vAP9/LwD3fy8A938vAP9/LwD/fy8A/38vAP+ALwD/gC8A/4EvAP+BLwD/gTAA/4IwAP+CMAD/gzAA/4MwAP+EMAD/hDAA/4UwAP+FMAD/hjAA/4YwAP+HMAD/hzAA/4gwAP+IMAD/iDAA/4kxAP+JMQD/ijEA/4sxAP+LMQD/izEA/4wxAP+MMQD/jTEA/40xAP+NMQD/jjEA/44xAP+OMQD/jjEA/48xAP+PMQD/kDEA/5AxAP+RMQD/kTIA/5EyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+TMgD/kzIA/5MyAP+TMgD/kzIA/5QyAP+UMgD/lDIA/5QyAP+UMgD/lDIA/5QyAP+UMgD/lDIA/5QyAP+UMgD/lDIA/5QyAP+TMgD/kzIA/5MyAP+TMgD/kzIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5EyAP+RMgD/kTEA/5AxAP+PMQD/jzEA/48xAP+OMQD/jjEA/44xAP+NMQD/jTEA/40xAP+NMQD/jDEA/4wxAP+LMQD/izEA/4oxAP+KMQD/iTEA/4kxAP+IMAD/iDAA/4cwAP+HMAD/hzAA/4YwAP+GMAD/hTAA/4UwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAP+BMAD/gS8A/4EvAP+ALwD/gC8A/38vAP9/LwD/fy8A/38vAPd/LwD1fy8A/38vAP9/LwD/fy8A/38vAP+ALwD/gC8A/4AvAP+BLwD/gS8A/4IwAP+CMAD/gjAA/4MwAP+DMAD/hDAA/4QwAP+FMAD/hTAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/iDAA/4gwAP+JMQD/iTEA/4oxAP+KMQD/izEA/4sxAP+LMQD/jDEA/4wxAP+NMQD/jTEA/40xAP+OMQD/jjEA/44xAP+OMQD/jjEA/48xAP+PMQD/jzEA/5AxAP+QMQD/kDEA/5EyAP+RMgD/kTIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kjIA/5IyAP+SMgD/kTIA/5EyAP+RMgD/kDEA/5AxAP+QMQD/jzEA/48xAP+PMQD/jjEA/44xAP+OMQD/jjEA/44xAP+NMQD/jTEA/40xAP+MMQD/jDEA/4sxAP+LMQD/izEA/4oxAP+KMQD/iTEA/4gwAP+IMAD/iDAA/4cwAP+HMAD/hjAA/4YwAP+GMAD/hTAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAP+BMAD/gS8A/4EvAP+ALwD/gC8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A838vAON/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+BLwD/gS8A/4EwAP+CMAD/gjAA/4IwAP+DMAD/gzAA/4QwAP+EMAD/hTAA/4UwAP+GMAD/hjAA/4cwAP+HMAD/hzAA/4gwAP+IMAD/iTEA/4kxAP+JMQD/ijEA/4oxAP+LMQD/izEA/4sxAP+MMQD/jDEA/4wxAP+NMQD/jTEA/40xAP+OMQD/jjEA/44xAP+OMQD/jjEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/5AxAP+QMQD/kDEA/5AxAP+QMgD/kTIA/5EyAP+RMgD/kTIA/5EyAP+RMgD/kTIA/5EyAP+RMgD/kTIA/5EyAP+QMQD/kDEA/5AxAP+QMQD/kDEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/44xAP+OMQD/jjEA/44xAP+NMQD/jTEA/40xAP+NMQD/jDEA/4wxAP+LMQD/izEA/4sxAP+LMQD/ijEA/4kxAP+JMQD/iTEA/4gwAP+IMAD/iDAA/4cwAP+HMAD/hjAA/4YwAP+GMAD/hTAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAP+CMAD/gTAA/4EvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwDlfy8AxX8vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gS8A/4EvAP+BMAD/gjAA/4IwAP+CMAD/gzAA/4MwAP+EMAD/hDAA/4QwAP+FMAD/hjAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/iDAA/4gwAP+JMQD/iTEA/4kxAP+JMQD/ijEA/4oxAP+LMQD/izEA/4sxAP+MMQD/jDEA/4wxAP+NMQD/jTEA/40xAP+NMQD/jTEA/44xAP+OMQD/jjEA/44xAP+OMQD/jjEA/44xAP+PMQD/jzEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/48xAP+PMQD/jzEA/44xAP+OMQD/jjEA/44xAP+OMQD/jjEA/44xAP+NMQD/jTEA/40xAP+NMQD/jTEA/4wxAP+MMQD/izEA/4sxAP+LMQD/izEA/4oxAP+KMQD/iTEA/4kxAP+JMQD/iDAA/4gwAP+IMAD/hzAA/4cwAP+HMAD/hjAA/4YwAP+FMAD/hTAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAP+CMAD/gTAA/4EvAP+BLwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAMd/LwCXfy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gC8A/4EvAP+BLwD/gTAA/4IwAP+CMAD/gjAA/4MwAP+DMAD/hDAA/4QwAP+EMAD/hTAA/4UwAP+GMAD/hjAA/4YwAP+HMAD/hzAA/4cwAP+IMAD/iDAA/4gwAP+JMQD/iTEA/4kxAP+KMQD/ijEA/4sxAP+LMQD/izEA/4sxAP+LMQD/jDEA/4wxAP+MMQD/jTEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/40xAP+OMQD/jjEA/44xAP+OMQD/jjEA/44xAP+OMQD/jjEA/44xAP+OMQD/jjEA/44xAP+OMQD/jjEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/4wxAP+MMQD/jDEA/4sxAP+LMQD/izEA/4sxAP+LMQD/ijEA/4oxAP+JMQD/iTEA/4kxAP+IMAD/iDAA/4gwAP+HMAD/hzAA/4cwAP+GMAD/hjAA/4YwAP+FMAD/hTAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gjAA/4IwAP+CMAD/gTAA/4EvAP+BLwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8Al38vAFR/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP+ALwD/gC8A/4AvAP+BLwD/gS8A/4EwAP+CMAD/gjAA/4IwAP+DMAD/gzAA/4MwAP+EMAD/hDAA/4QwAP+FMAD/hjAA/4YwAP+GMAD/hjAA/4cwAP+HMAD/hzAA/4gwAP+IMAD/iDAA/4gwAP+JMQD/iTEA/4kxAP+KMQD/ijEA/4oxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+MMQD/jDEA/4wxAP+MMQD/jDEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/40xAP+NMQD/jTEA/4wxAP+MMQD/jDEA/4wxAP+MMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/ijEA/4oxAP+KMQD/iTEA/4kxAP+JMQD/iDAA/4gwAP+IMAD/iDAA/4cwAP+HMAD/hzAA/4YwAP+GMAD/hjAA/4UwAP+FMAD/hDAA/4QwAP+EMAD/gzAA/4MwAP+DMAD/gjAA/4IwAP+CMAD/gS8A/4EvAP+BLwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwBSfy8ADH8vAPV/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gS8A/4EvAP+BMAD/gjAA/4IwAP+CMAD/gzAA/4MwAP+DMAD/hDAA/4QwAP+EMAD/hDAA/4UwAP+GMAD/hjAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/iDAA/4gwAP+IMAD/iDAA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4oxAP+KMQD/ijEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+LMQD/izEA/4sxAP+KMQD/ijEA/4oxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4kxAP+IMAD/iDAA/4gwAP+IMAD/hzAA/4cwAP+HMAD/hjAA/4YwAP+GMAD/hTAA/4UwAP+EMAD/hDAA/4QwAP+EMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+CMAD/gTAA/4EvAP+BLwD/gC8A/4AvAP+ALwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A9X8vAAwAAAAAfy8AnX8vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gC8A/4EvAP+BLwD/gS8A/4IwAP+CMAD/gjAA/4IwAP+DMAD/gzAA/4MwAP+EMAD/hDAA/4QwAP+EMAD/hTAA/4UwAP+GMAD/hjAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/hzAA/4gwAP+IMAD/iDAA/4gwAP+IMAD/iTEA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4kxAP+KMQD/ijEA/4oxAP+KMQD/ijEA/4oxAP+KMQD/ijEA/4oxAP+KMQD/ijEA/4oxAP+KMQD/ijEA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4gwAP+IMAD/iDAA/4gwAP+IMAD/hzAA/4cwAP+HMAD/hzAA/4YwAP+GMAD/hjAA/4YwAP+FMAD/hTAA/4QwAP+EMAD/hDAA/4QwAP+DMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+BMAD/gS8A/4EvAP+BLwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwCdAAAAAAAAAAB/LwAmfy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gS8A/4EvAP+BLwD/gjAA/4IwAP+CMAD/gjAA/4MwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4QwAP+FMAD/hTAA/4UwAP+GMAD/hjAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/iDAA/4gwAP+IMAD/iDAA/4gwAP+IMAD/iDAA/4gwAP+IMAD/iTEA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4kxAP+JMQD/iTEA/4kxAP+IMAD/iDAA/4gwAP+IMAD/iDAA/4gwAP+IMAD/iDAA/4gwAP+HMAD/hzAA/4cwAP+HMAD/hzAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hTAA/4UwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gzAA/4IwAP+CMAD/gjAA/4IwAP+BLwD/gS8A/4EvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vACYAAAAAAAAAAAAAAAB/LwCZfy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gC8A/4EvAP+BLwD/gS8A/4EwAP+CMAD/gjAA/4IwAP+CMAD/gzAA/4MwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4QwAP+FMAD/hTAA/4UwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/iDAA/4gwAP+IMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/hzAA/4cwAP+HMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4UwAP+FMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+DMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4EvAP+BLwD/gS8A/4AvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwCZAAAAAAAAAAAAAAAAAAAAAH8vABB/LwDjfy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gS8A/4EvAP+BLwD/gTAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gzAA/4MwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4UwAP+FMAD/hTAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+GMAD/hjAA/4YwAP+FMAD/hTAA/4UwAP+FMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/gzAA/4MwAP+DMAD/gzAA/4IwAP+CMAD/gjAA/4IwAP+BMAD/gS8A/4EvAP+BLwD/gC8A/4AvAP+ALwD/gC8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A438vABAAAAAAAAAAAAAAAAAAAAAAAAAAAH8vADp/LwD7fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gC8A/4AvAP+BLwD/gS8A/4EvAP+BMAD/gTAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gzAA/4MwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+FMAD/hTAA/4UwAP+FMAD/hTAA/4UwAP+FMAD/hTAA/4UwAP+FMAD/hTAA/4UwAP+FMAD/hTAA/4UwAP+FMAD/hTAA/4UwAP+FMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4MwAP+DMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4EwAP+BLwD/gS8A/4EvAP+BLwD/gC8A/4AvAP+ALwD/gC8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAPt/LwA6AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8vAFp/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+BLwD/gS8A/4EvAP+BMAD/gTAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+EMAD/hDAA/4QwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+BMAD/gS8A/4EvAP+BLwD/gS8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8AWgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8vAFp/LwD5fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+BLwD/gS8A/4EvAP+BLwD/gTAA/4EwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+DMAD/gzAA/4MwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+BMAD/gTAA/4EvAP+BLwD/gS8A/4EvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A+X8vAFoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8vADZ/LwDhfy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gTAA/4EwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gjAA/4IwAP+CMAD/gTAA/4EwAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAOF/LwA2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8vABB/LwCTfy8A+38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gS8A/4EvAP+BLwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAPt/LwCTfy8AEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB/LwAkfy8AmX8vAO9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP+ALwD/gC8A/4AvAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAP9/LwD/fy8A/38vAO9/LwCZfy8AJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAfy8ADH8vAE5/LwCTfy8Aw38vAOF/LwDzfy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3gC8A94AvAPeALwD3gC8A94AvAPeALwD3gC8A94AvAPeALwD3gC8A94AvAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPd/LwD3fy8A938vAPN/LwDhfy8Aw38vAJN/LwBOfy8ADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA//gAAAAAAAAAAAAAAAAf///AAAAAAAAAAAAAAAAAA///AAAAAAAAAAAAAAAAAAD//gAAAAAAAAAAAAAAAAAAf/wAAAAAAAAAAAAAAAAAAD/4AAAAAAAAAAAAAAAAAAAf8AAAAAAAAAAAAAAAAAAAD+AAAAAAAAAAAAAAAAAAAAfAAAAAAAAAAAAAAAAAAAADwAAAAAAAAAAAAAAAAAAAA4AAAAAAAAAAAAAAAAAAAAGAAAAAAAAAAAAAAAAAAAABgAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAAAAAAAAAAAAAAAAGAAAAAAAAAAAAAAAAAAAABgAAAAAAAAAAAAAAAAAAAAcAAAAAAAAAAAAAAAAAAAAPAAAAAAAAAAAAAAAAAAAAD4AAAAAAAAAAAAAAAAAAAB/AAAAAAAAAAAAAAAAAAAA/4AAAAAAAAAAAAAAAAAAAf/AAAAAAAAAAAAAAAAAAAP/4AAAAAAAAAAAAAAAAAAH//AAAAAAAAAAAAAAAAAAD//8AAAAAAAAAAAAAAAAAD///4AAAAAAAAAAAAAAAAH/8=";
                return new Icon(new MemoryStream(Convert.FromBase64String(base64)));
            }
        }

        #region 图片缩放
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="bArr">图片字节流</param>
        /// <param name="width">目标宽度，若为0，表示宽度按比例缩放</param>
        /// <param name="height">目标长度，若为0，表示长度按比例缩放</param>
        public static byte[] GetThumbnail2(byte[] bArr, int width, int height)
        {
            if (bArr == null) return null;
            MemoryStream ms = new MemoryStream(bArr);
            Bitmap bmp = (Bitmap)Image.FromStream(ms);
            ms.Close();

            bmp = GetThumbnail2(bmp, width, height);

            ImageCodecInfo imageCodecInfo = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
            encoderParameters.Param[0] = encoderParameter;

            ms = new MemoryStream();
            bmp.Save(ms, imageCodecInfo, encoderParameters);
            byte[] result = ms.ToArray();
            ms.Close();
            bmp.Dispose();

            return result;
        }
        #endregion

        #region 图片缩放
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="width">目标宽度，若为0，表示宽度按比例缩放</param>
        /// <param name="height">目标长度，若为0，表示长度按比例缩放</param>
        private static Bitmap GetThumbnail2(Bitmap bmp, int width, int height)
        {
            if (width == 0 && height == 0)
            {
                width = bmp.Width;
                height = bmp.Height;
            }
            else
            {
                if (width == 0)
                {
                    width = height * bmp.Width / bmp.Height;
                }
                if (height == 0)
                {
                    height = width * bmp.Height / bmp.Width;
                }
            }

            Image imgSource = bmp;
            Bitmap outBmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量     
            g.CompositingQuality = CompositingQuality.Default;
            g.SmoothingMode = SmoothingMode.Default;
            g.InterpolationMode = InterpolationMode.Default;
            g.DrawImage(imgSource, new Rectangle(0, 0, width, height + 1), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            imgSource.Dispose();
            bmp.Dispose();
            return outBmp;
        }
        #endregion

        #region 椭圆形缩放
        /// <summary>
        /// 椭圆形缩放
        /// </summary>
        /// <param name="bArr">图片字节流</param>
        /// <param name="width">目标宽度，若为0，表示宽度按比例缩放</param>
        /// <param name="height">目标长度，若为0，表示长度按比例缩放</param>
        public static byte[] GetEllipseThumbnail(byte[] bArr, int width, int height)
        {
            if (bArr == null) return null;
            MemoryStream ms = new MemoryStream(bArr);
            Bitmap bmp = (Bitmap)Image.FromStream(ms);

            Bitmap newBmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newBmp))
            {
                using (TextureBrush br = new TextureBrush(bmp))
                {
                    br.ScaleTransform(width / (float)bmp.Width, height / (float)bmp.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillEllipse(br, new Rectangle(Point.Empty, new Size(width, height)));
                }
            }
            MemoryStream newMs = new MemoryStream();
            newBmp.Save(newMs, System.Drawing.Imaging.ImageFormat.Png);
            byte[] result = newMs.ToArray();

            bmp.Dispose();
            newBmp.Dispose();
            ms.Close();
            newMs.Dispose();

            return result;
        }
        #endregion

        #region 方形裁剪
        /// <summary>
        /// 方形裁剪
        /// </summary>
        /// <param name="bArr">图片字节流</param>
        /// <param name="left">左上角横坐标比例</param>
        /// <param name="top">左上角纵坐标比例</param>
        /// <param name="right">右下角横坐标比例</param>
        /// <param name="bottom">右下角纵坐标比例</param>
        public static byte[] CutImage(byte[] bArr, double left, double top, double right, double bottom)
        {
            if (bArr == null) return null;
            MemoryStream ms = new MemoryStream(bArr);
            Bitmap bmp = (Bitmap)Image.FromStream(ms);

            int width = (int)((right - left) * bmp.Width);
            int height = (int)((bottom - top) * bmp.Height);

            Bitmap newBmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newBmp))
            {
                using (TextureBrush br = new TextureBrush(bmp))
                {
                    Point p = new Point((int)(left * bmp.Width), (int)(top * bmp.Height));
                    g.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(p.X, p.Y, width, height), GraphicsUnit.Pixel);
                }
            }
            MemoryStream newMs = new MemoryStream();
            newBmp.Save(newMs, System.Drawing.Imaging.ImageFormat.Png);
            byte[] result = newMs.ToArray();

            bmp.Dispose();
            newBmp.Dispose();
            ms.Close();
            newMs.Dispose();

            return result;
        }
        #endregion

        #region GetEncoder
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion



        #region 正方型裁剪并缩放

        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSaveUrl">缩略图存放地址</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForSquare(this Stream fromFile, string fileSaveUrl, int side, int quality)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileSaveUrl));
            Image initImage = Image.FromStream(fromFile, true);
            if (initImage.Width <= side && initImage.Height <= side)
            {
                initImage.Save(fileSaveUrl, ImageFormat.Jpeg);
                return;
            }
            int initWidth = initImage.Width;
            int initHeight = initImage.Height;
            if (initWidth != initHeight)
            {
                Image pickedImage;
                Graphics pickedG;
                if (initWidth > initHeight)
                {
                    pickedImage = new Bitmap(initHeight, initHeight);
                    pickedG = Graphics.FromImage(pickedImage);
                    pickedG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = SmoothingMode.HighQuality;
                    Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                    Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                    pickedG.DrawImage(initImage, toR, fromR, GraphicsUnit.Pixel);
                    initWidth = initHeight;
                }
                else
                {
                    pickedImage = new Bitmap(initWidth, initWidth);
                    pickedG = Graphics.FromImage(pickedImage);
                    pickedG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = SmoothingMode.HighQuality;
                    Rectangle fromR2 = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                    Rectangle toR2 = new Rectangle(0, 0, initWidth, initWidth);
                    pickedG.DrawImage(initImage, toR2, fromR2, GraphicsUnit.Pixel);
                    initHeight = initWidth;
                }
                initImage = (Image)pickedImage.Clone();
                initImage.Dispose();
                pickedG.Dispose();
                pickedImage.Dispose();
            }
            using (Image resultImage = new Bitmap(side, side))
            {
                using (Graphics resultG = Graphics.FromImage(resultImage))
                {
                    resultG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    resultG.SmoothingMode = SmoothingMode.HighQuality;
                    resultG.Clear(Color.White);
                    resultG.DrawImage(initImage, new Rectangle(0, 0, side, side), new Rectangle(0, 0, initWidth, initHeight), GraphicsUnit.Pixel);
                    ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;
                    foreach (ImageCodecInfo i in imageEncoders)
                    {
                        if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                        {
                            ici = i;
                        }
                    }
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                    using (EncoderParameters ep = encoderParameters)
                    {
                        resultImage.Save(fileSaveUrl, ici, ep);
                    }
                }
            }
        }

        #endregion

        #region 自定义裁剪并缩放

        /// <summary>
        /// 指定长宽裁剪
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSaveUrl">保存路径</param>
        /// <param name="maxWidth">最大宽(单位:px)</param>
        /// <param name="maxHeight">最大高(单位:px)</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForCustom(this Stream fromFile, string fileSaveUrl, int maxWidth, int maxHeight, int quality)
        {
            using (Image initImage = Image.FromStream(fromFile, true))
            {
                if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
                {
                    initImage.Save(fileSaveUrl, ImageFormat.Jpeg);
                }
                else
                {
                    double templateRate = (double)maxWidth / (double)maxHeight;
                    double initRate = (double)initImage.Width / (double)initImage.Height;
                    if (templateRate == initRate)
                    {
                        Bitmap bitmap = new Bitmap(maxWidth, maxHeight);
                        Graphics graphics = Graphics.FromImage(bitmap);
                        graphics.InterpolationMode = InterpolationMode.High;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.Clear(Color.White);
                        graphics.DrawImage(initImage, new Rectangle(0, 0, maxWidth, maxHeight), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
                        bitmap.Save(fileSaveUrl, ImageFormat.Jpeg);
                    }
                    else
                    {
                        Rectangle fromR = new Rectangle(0, 0, 0, 0);
                        Rectangle toR = new Rectangle(0, 0, 0, 0);
                        Image pickedImage;
                        Graphics pickedG;
                        if (templateRate > initRate)
                        {
                            pickedImage = new Bitmap(initImage.Width, (int)Math.Floor((double)initImage.Width / templateRate));
                            pickedG = Graphics.FromImage(pickedImage);
                            fromR.X = 0;
                            fromR.Y = (int)Math.Floor(((double)initImage.Height - (double)initImage.Width / templateRate) / 2.0);
                            fromR.Width = initImage.Width;
                            fromR.Height = (int)Math.Floor((double)initImage.Width / templateRate);
                            toR.X = 0;
                            toR.Y = 0;
                            toR.Width = initImage.Width;
                            toR.Height = (int)Math.Floor((double)initImage.Width / templateRate);
                        }
                        else
                        {
                            pickedImage = new Bitmap((int)Math.Floor((double)initImage.Height * templateRate), initImage.Height);
                            pickedG = Graphics.FromImage(pickedImage);
                            fromR.X = (int)Math.Floor(((double)initImage.Width - (double)initImage.Height * templateRate) / 2.0);
                            fromR.Y = 0;
                            fromR.Width = (int)Math.Floor((double)initImage.Height * templateRate);
                            fromR.Height = initImage.Height;
                            toR.X = 0;
                            toR.Y = 0;
                            toR.Width = (int)Math.Floor((double)initImage.Height * templateRate);
                            toR.Height = initImage.Height;
                        }
                        pickedG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        pickedG.SmoothingMode = SmoothingMode.HighQuality;
                        pickedG.DrawImage(initImage, toR, fromR, GraphicsUnit.Pixel);
                        using (Image templateImage = new Bitmap(maxWidth, maxHeight))
                        {
                            using (Graphics templateG = Graphics.FromImage(templateImage))
                            {
                                templateG.InterpolationMode = InterpolationMode.High;
                                templateG.SmoothingMode = SmoothingMode.HighQuality;
                                templateG.Clear(Color.White);
                                templateG.DrawImage(pickedImage, new Rectangle(0, 0, maxWidth, maxHeight), new Rectangle(0, 0, pickedImage.Width, pickedImage.Height), GraphicsUnit.Pixel);
                                ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
                                ImageCodecInfo ici = null;
                                foreach (ImageCodecInfo i in imageEncoders)
                                {
                                    if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                                    {
                                        ici = i;
                                    }
                                }
                                EncoderParameters ep = new EncoderParameters(1);
                                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                                templateImage.Save(fileSaveUrl, ici, ep);
                                pickedG.Dispose();
                                pickedImage.Dispose();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 等比缩放

        /// <summary>
        /// 图片等比缩放
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="savePath">缩略图存放地址</param>
        /// <param name="targetWidth">指定的最大宽度</param>
        /// <param name="targetHeight">指定的最大高度</param>
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
        public static void ZoomAuto(this Stream fromFile, string savePath, double targetWidth, double targetHeight, string watermarkText, string watermarkImage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            using (Image initImage = Image.FromStream(fromFile, true))
            {
                if ((double)initImage.Width <= targetWidth && (double)initImage.Height <= targetHeight)
                {
                    if (watermarkText != "")
                    {
                        using (Graphics gWater = Graphics.FromImage(initImage))
                        {
                            Font fontWater = new Font("黑体", 10f);
                            Brush brushWater = new SolidBrush(Color.White);
                            gWater.DrawString(watermarkText, fontWater, brushWater, 10f, 10f);
                            gWater.Dispose();
                        }
                    }
                    if (watermarkImage != "" && File.Exists(watermarkImage))
                    {
                        using (Image wrImage = Image.FromFile(watermarkImage))
                        {
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater2 = Graphics.FromImage(initImage);
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap[] remapTable = new ColorMap[]
                                {
                            new ColorMap
                            {
                                OldColor = Color.FromArgb(255, 0, 255, 0),
                                NewColor = Color.FromArgb(0, 0, 0, 0)
                            }
                                };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                                float[][] array = new float[5][];
                                int num = 0;
                                float[] array2 = new float[5];
                                array2[0] = 1f;
                                array[num] = array2;
                                int num2 = 1;
                                float[] array3 = new float[5];
                                array3[1] = 1f;
                                array[num2] = array3;
                                int num3 = 2;
                                float[] array4 = new float[5];
                                array4[2] = 1f;
                                array[num3] = array4;
                                int num4 = 3;
                                float[] array5 = new float[5];
                                array5[3] = 0.5f;
                                array[num4] = array5;
                                array[4] = new float[]
                                {
                            0f,
                            0f,
                            0f,
                            0f,
                            1f
                                };
                                ColorMatrix wmColorMatrix = new ColorMatrix(array);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater2.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater2.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                    initImage.Save(savePath, ImageFormat.Jpeg);
                }
                else
                {
                    double newWidth = (double)initImage.Width;
                    double newHeight = (double)initImage.Height;
                    if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
                    {
                        if ((double)initImage.Width > targetWidth)
                        {
                            newWidth = targetWidth;
                            newHeight = (double)initImage.Height * (targetWidth / (double)initImage.Width);
                        }
                    }
                    else if ((double)initImage.Height > targetHeight)
                    {
                        newHeight = targetHeight;
                        newWidth = (double)initImage.Width * (targetHeight / (double)initImage.Height);
                    }
                    using (Image newImage = new Bitmap((int)newWidth, (int)newHeight))
                    {
                        using (Graphics newG = Graphics.FromImage(newImage))
                        {
                            newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            newG.SmoothingMode = SmoothingMode.HighQuality;
                            newG.Clear(Color.White);
                            newG.DrawImage(initImage, new Rectangle(0, 0, newImage.Width, newImage.Height), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
                            if (watermarkText != "")
                            {
                                using (Graphics gWater3 = Graphics.FromImage(newImage))
                                {
                                    Font fontWater2 = new Font("宋体", 10f);
                                    Brush brushWater2 = new SolidBrush(Color.White);
                                    gWater3.DrawString(watermarkText, fontWater2, brushWater2, 10f, 10f);
                                    gWater3.Dispose();
                                }
                            }
                            if (watermarkImage != "" && File.Exists(watermarkImage))
                            {
                                using (Image wrImage2 = Image.FromFile(watermarkImage))
                                {
                                    if (newImage.Width >= wrImage2.Width && newImage.Height >= wrImage2.Height)
                                    {
                                        Graphics gWater4 = Graphics.FromImage(newImage);
                                        ImageAttributes imgAttributes2 = new ImageAttributes();
                                        ColorMap[] remapTable2 = new ColorMap[]
                                        {
                                    new ColorMap
                                    {
                                        OldColor = Color.FromArgb(255, 0, 255, 0),
                                        NewColor = Color.FromArgb(0, 0, 0, 0)
                                    }
                                        };
                                        imgAttributes2.SetRemapTable(remapTable2, ColorAdjustType.Bitmap);
                                        float[][] array6 = new float[5][];
                                        int num5 = 0;
                                        float[] array7 = new float[5];
                                        array7[0] = 1f;
                                        array6[num5] = array7;
                                        int num6 = 1;
                                        float[] array8 = new float[5];
                                        array8[1] = 1f;
                                        array6[num6] = array8;
                                        int num7 = 2;
                                        float[] array9 = new float[5];
                                        array9[2] = 1f;
                                        array6[num7] = array9;
                                        int num8 = 3;
                                        float[] array10 = new float[5];
                                        array10[3] = 0.5f;
                                        array6[num8] = array10;
                                        array6[4] = new float[]
                                        {
                                    0f,
                                    0f,
                                    0f,
                                    0f,
                                    1f
                                        };
                                        ColorMatrix wmColorMatrix2 = new ColorMatrix(array6);
                                        imgAttributes2.SetColorMatrix(wmColorMatrix2, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                        gWater4.DrawImage(wrImage2, new Rectangle(newImage.Width - wrImage2.Width, newImage.Height - wrImage2.Height, wrImage2.Width, wrImage2.Height), 0, 0, wrImage2.Width, wrImage2.Height, GraphicsUnit.Pixel, imgAttributes2);
                                        gWater4.Dispose();
                                    }
                                }
                            }
                            newImage.Save(savePath, ImageFormat.Jpeg);
                        }
                    }
                }
            }
        }

        #endregion

        #region 判断文件类型是否为WEB格式图片

        /// <summary>
        /// 判断文件类型是否为WEB格式图片
        /// (注：JPG,GIF,BMP,PNG)
        /// </summary>
        /// <param name="contentType">HttpPostedFile.ContentType</param>
        /// <returns>是否为WEB格式图片</returns>
        public static bool IsWebImage(string contentType)
        {
            return contentType == "image/pjpeg" || contentType == "image/jpeg" || contentType == "image/gif" || contentType == "image/bmp" || contentType == "image/png";
        }

        #endregion

        #region 裁剪图片

        /// <summary>
        /// 裁剪图片 -- 用GDI+   
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="rec">裁剪区域</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Bitmap CutImage(this Bitmap b, Rectangle rec)
        {
            int w = b.Width;
            int h = b.Height;
            if (rec.X >= w || rec.Y >= h)
            {
                return null;
            }
            if (rec.X + rec.Width > w)
            {
                rec.Width = w - rec.X;
            }
            if (rec.Y + rec.Height > h)
            {
                rec.Height = h - rec.Y;
            }
            Bitmap result;
            try
            {
                using (Bitmap bmpOut = new Bitmap(rec.Width, rec.Height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(bmpOut))
                    {
                        g.DrawImage(b, new Rectangle(0, 0, rec.Width, rec.Height), new Rectangle(rec.X, rec.Y, rec.Width, rec.Height), GraphicsUnit.Pixel);
                        result = bmpOut;
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        #endregion

        #region 缩放图片

        /// <summary>  
        ///  Resize图片   
        /// </summary>  
        /// <param name="bmp">原始Bitmap </param>  
        /// <param name="newWidth">新的宽度</param>  
        /// <param name="newHeight">新的高度</param>  
        /// <returns>处理以后的图片</returns>  
        public static Bitmap ResizeImage(this Bitmap bmp, int newWidth, int newHeight)
        {
            try
            {
                using (var b = new Bitmap(newWidth, newHeight))
                {
                    using (var g = Graphics.FromImage(b))
                    {
                        // 插值算法的质量   
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bmp, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                        return b;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 裁剪并缩放

        /// <summary>
        /// 裁剪并缩放
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="rec">裁剪的矩形区域</param>
        /// <param name="newWidth">新的宽度</param>  
        /// <param name="newHeight">新的高度</param>  
        /// <returns>处理以后的图片</returns>
        public static Bitmap CutAndResize(this Bitmap bmp, Rectangle rec, int newWidth, int newHeight) => bmp.CutImage(rec).ResizeImage(newWidth, newHeight);

        #endregion

        #region 无损压缩图片

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="quality">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, byte quality = 90, int size = 1024, bool sfsc = true)
        {
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc && firstFileInfo.Length < (long)(size * 1024))
            {
                firstFileInfo.CopyTo(dFile);
                return true;
            }
            bool result;
            using (Image iSource = Image.FromFile(sFile))
            {
                ImageFormat tFormat = iSource.RawFormat;
                int dHeight = iSource.Height;
                int dWidth = iSource.Width;
                Size temSize = new Size(iSource.Width, iSource.Height);
                int sW;
                int sH;
                if (temSize.Width > dHeight || temSize.Width > dWidth)
                {
                    if (temSize.Width * dHeight > temSize.Width * dWidth)
                    {
                        sW = dWidth;
                        sH = dWidth * temSize.Height / temSize.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = temSize.Width * dHeight / temSize.Height;
                    }
                }
                else
                {
                    sW = temSize.Width;
                    sH = temSize.Height;
                }
                using (Bitmap bmp = new Bitmap(dWidth, dHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.WhiteSmoke);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
                        using (EncoderParameters ep = new EncoderParameters())
                        {
                            using (EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, new long[] { (long)((ulong)quality) }))
                            {
                                ep.Param[0] = eParam;
                                try
                                {
                                    ImageCodecInfo jpegIcIinfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault((ImageCodecInfo t) => t.FormatDescription.Equals("JPEG"));
                                    if (jpegIcIinfo != null)
                                    {
                                        bmp.Save(dFile, jpegIcIinfo, ep);
                                        if (new FileInfo(dFile).Length > (long)(1024 * size) && quality > 10)
                                        {
                                            quality -= 10;
                                            CompressImage(sFile, dFile, quality, size, false);
                                        }
                                    }
                                    else
                                    {
                                        bmp.Save(dFile, tFormat);
                                    }
                                    result = true;
                                }
                                catch
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="src">原图片文件流</param>
        /// <param name="dest">压缩后图片文件流</param>
        /// <param name="quality">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(Stream src, Stream dest, byte quality = 90, int size = 1024, bool sfsc = true)
        {
            if (sfsc && src.Length < (long)(size * 1024))
            {
                src.CopyTo(dest);
                return true;
            }
            bool result;
            using (Image iSource = Image.FromStream(src))
            {
                ImageFormat tFormat = iSource.RawFormat;
                int dHeight = iSource.Height;
                int dWidth = iSource.Width;
                Size temSize = new Size(iSource.Width, iSource.Height);
                int sW;
                int sH;
                if (temSize.Width > dHeight || temSize.Width > dWidth)
                {
                    if (temSize.Width * dHeight > temSize.Width * dWidth)
                    {
                        sW = dWidth;
                        sH = dWidth * temSize.Height / temSize.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = temSize.Width * dHeight / temSize.Height;
                    }
                }
                else
                {
                    sW = temSize.Width;
                    sH = temSize.Height;
                }
                using (Bitmap bmp = new Bitmap(dWidth, dHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.WhiteSmoke);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
                        using (EncoderParameters ep = new EncoderParameters())
                        {
                            using (EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, new long[]
                            {
                        (long)((ulong)quality)
                            }))
                            {
                                ep.Param[0] = eParam;
                                try
                                {
                                    ImageCodecInfo jpegIcIinfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault((ImageCodecInfo t) => t.FormatDescription.Equals("JPEG"));
                                    if (jpegIcIinfo != null)
                                    {
                                        bmp.Save(dest, jpegIcIinfo, ep);
                                        if (dest.Length > (long)(1024 * size) && quality > 10)
                                        {
                                            quality -= 10;
                                            CompressImage(src, dest, quality, size, false);
                                        }
                                    }
                                    else
                                    {
                                        bmp.Save(dest, tFormat);
                                    }
                                    result = true;
                                }
                                catch
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region 缩略图

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImage">原图</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(this Image originalImage, string thumbnailPath, int width, int height, ThumbnailCutMode mode)
        {
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailCutMode.Fixed: //指定高宽缩放（可能变形）                
                    break;
                case ThumbnailCutMode.LockWidth: //指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailCutMode.LockHeight: //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailCutMode.Cut: //指定高宽裁减（不变形）                
                    if (originalImage.Width / (double)originalImage.Height > towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
            }

            //新建一个bmp图片
            using (Image bitmap = new Bitmap(towidth, toheight))
            {

                //新建一个画板
                using (Graphics g = Graphics.FromImage(bitmap))
                {

                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.High;

                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);

                    //在指定位置并且按指定大小绘制原图片的指定部分
                    //第一个：对哪张图片进行操作。
                    //二：画多么大。
                    //三：画那块区域。
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

                    //以jpg格式保存缩略图
                    bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                }
            }
        }

        #endregion

        #region 调整光暗

        /// <summary>
        /// 调整光暗
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <param name="val">增加或减少的光暗值</param>
        public static Bitmap LDPic(this Bitmap mybm, int width, int height, int val)
        {
            Bitmap bm = new Bitmap(width, height); //初始化一个记录经过处理后的图片对象
            int x, y; //x、y是循环次数，后面三个是记录红绿蓝三个值的
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    var pixel = mybm.GetPixel(x, y);
                    var resultR = pixel.R + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    var resultG = pixel.G + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    var resultB = pixel.B + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB)); //绘图
                }
            }

            return bm;
        }

        #endregion

        #region 反色处理

        /// <summary>
        /// 反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        public static Bitmap RePic(this Bitmap mybm, int width, int height)
        {
            using (var bm = new Bitmap(width, height))
            { //初始化一个记录处理后的图片的对象
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var pixel = mybm.GetPixel(x, y);
                        var resultR = 255 - pixel.R;
                        var resultG = 255 - pixel.G;
                        var resultB = 255 - pixel.B;
                        bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB)); //绘图
                    }
                }

                return bm;
            }
        }

        #endregion

        #region 浮雕处理

        /// <summary>
        /// 浮雕处理
        /// </summary>
        /// <param name="oldBitmap">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        public static Bitmap Relief(this Bitmap oldBitmap, int width, int height)
        {
            using (var newBitmap = new Bitmap(width, height))
            {
                for (int x = 0; x < width - 1; x++)
                {
                    for (int y = 0; y < height - 1; y++)
                    {
                        var color1 = oldBitmap.GetPixel(x, y);
                        var color2 = oldBitmap.GetPixel(x + 1, y + 1);
                        var r = Math.Abs(color1.R - color2.R + 128);
                        var g = Math.Abs(color1.G - color2.G + 128);
                        var b = Math.Abs(color1.B - color2.B + 128);
                        if (r > 255) r = 255;
                        if (r < 0) r = 0;
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        if (b > 255) b = 255;
                        if (b < 0) b = 0;
                        newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                return newBitmap;
            }
        }

        #endregion

        #region 拉伸图片

        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        public static async Task<Bitmap> ResizeImageAsync(this Bitmap bmp, int newW, int newH)
        {
            try
            {
                using (Bitmap bap = new Bitmap(newW, newH))
                {
                    return await Task.Run(() =>
                    {
                        using (Graphics g = Graphics.FromImage(bap))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(bap, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bap.Width, bap.Height), GraphicsUnit.Pixel);
                            return bap;
                        }
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 滤色处理

        /// <summary>
        /// 滤色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        public static Bitmap FilPic(this Bitmap mybm, int width, int height)
        {
            using (var bm = new Bitmap(width, height))
            {
                for (var x = 0; x < width; x++)
                {
                    int y;
                    for (y = 0; y < height; y++)
                    {
                        var pixel = mybm.GetPixel(x, y);
                        bm.SetPixel(x, y, Color.FromArgb(0, pixel.G, pixel.B)); //绘图
                    }
                }
                return bm;
            }
        }

        #endregion

        #region 左右翻转

        /// <summary>
        /// 左右翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        public static Bitmap RevPicLR(this Bitmap mybm, int width, int height)
        {
            using (var bm = new Bitmap(width, height))
            {
                //x,y是循环次数,z是用来记录像素点的x坐标的变化的
                for (var y = height - 1; y >= 0; y--)
                {
                    int x; //x,y是循环次数,z是用来记录像素点的x坐标的变化的
                    int z; //x,y是循环次数,z是用来记录像素点的x坐标的变化的
                    for (x = width - 1, z = 0; x >= 0; x--)
                    {
                        var pixel = mybm.GetPixel(x, y);
                        bm.SetPixel(z++, y, Color.FromArgb(pixel.R, pixel.G, pixel.B)); //绘图
                    }
                }
                return bm;
            }
        }

        #endregion

        #region 上下翻转

        /// <summary>
        /// 上下翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        public static Bitmap RevPicUD(this Bitmap mybm, int width, int height)
        {
            using (var bm = new Bitmap(width, height))
            {
                for (var x = 0; x < width; x++)
                {
                    int y;
                    int z;
                    for (y = height - 1, z = 0; y >= 0; y--)
                    {
                        var pixel = mybm.GetPixel(x, y);
                        bm.SetPixel(x, z++, Color.FromArgb(pixel.R, pixel.G, pixel.B)); //绘图
                    }
                }
                return bm;
            }
        }

        #endregion

        #region 压缩图片

        /// <summary>
        /// 压缩到指定尺寸
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newfile">新文件</param>
        public static bool Compress(this Image img, string newfile)
        {
            try
            {
                Size newSize = new Size(100, 125);
                using (Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics g = Graphics.FromImage(outBmp))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(img, new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                        var encoderParams = new EncoderParameters();
                        var quality = new long[1];
                        quality[0] = 100;
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                        ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                        ImageCodecInfo jpegICI = arrayICI.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                        if (jpegICI != null)
                        {
                            outBmp.Save(newfile, ImageFormat.Jpeg);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 图片灰度化

        /// <summary>
        /// 图片灰度化
        /// </summary>
        /// <param name="c">输入颜色</param>
        /// <returns>输出颜色</returns>
        public static Color Gray(this Color c)
        {
            int rgb = Convert.ToInt32(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
            return Color.FromArgb(rgb, rgb, rgb);
        }

        #endregion

        #region 转换为黑白图片

        /// <summary>
        /// 转换为黑白图片
        /// </summary>
        /// <param name="mybm">要进行处理的图片</param>
        /// <param name="width">图片的长度</param>
        /// <param name="height">图片的高度</param>
        public static Bitmap BWPic(this Bitmap mybm, int width, int height)
        {
            using (var bm = new Bitmap(width, height))
            {
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var pixel = mybm.GetPixel(x, y);
                        var result = (pixel.R + pixel.G + pixel.B) / 3; //记录处理后的像素值
                        bm.SetPixel(x, y, Color.FromArgb(result, result, result));
                    }
                }
                return bm;
            }
        }

        #endregion

        #region 获取图片中的各帧

        /// <summary>
        /// 获取图片中的各帧
        /// </summary>
        /// <param name="gif">源gif</param>
        /// <param name="pSavedPath">保存路径</param>
        public static void GetFrames(this Image gif, string pSavedPath)
        {
            var fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd); //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            for (int i = 0; i < count; i++) //以Jpeg格式保存各帧
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(pSavedPath + "\\frame_" + i + ".jpg", ImageFormat.Jpeg);
            }
        }

        #endregion


        /// <summary>
        /// 将dataUri保存为图片
        /// </summary>
        /// <param name="source">dataUri数据源</param>
        /// <returns></returns>
        /// <exception cref="Exception">操作失败。</exception>
        public static Bitmap SaveDataUriAsImageFile(this string source)
        {
            string strbase64 = source.Substring(source.IndexOf(',') + 1).Trim('\0');
            byte[] arr = Convert.FromBase64String(strbase64);
            using (var ms = new MemoryStream(arr))
            {
                using (var bmp = new Bitmap(ms))
                {
                    //新建第二个bitmap类型的bmp2变量。
                    using (var bmp2 = new Bitmap(bmp, bmp.Width, bmp.Height))
                    {
                        using (var draw = Graphics.FromImage(bmp2))
                        {
                            draw.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                        }
                        return bmp2;
                    }
                }
            }
        }
		
		public static Image DrawAgvParmsImages(params string[] strArray)
        {
            //float eng = 12;
            //float cha = eng * 3 / 2;
            //float num = cha / 2;
            float fontSize = 12;
            Font font = new Font("微软雅黑", fontSize * 5 / 6, FontStyle.Regular);
            Brush brush = new SolidBrush(Color.FromArgb(0x7A, 0x7A, 0x7A));
            float emptyWidth = fontSize / 3;
            float width = 0;
            float height = 0;
            GetMaxWidthAndHeight(ref width, ref height, fontSize, strArray);

            int imageWidth = (int)(width + emptyWidth) + 1;
            int imageHeight = (int)(emptyWidth * height + 5 * emptyWidth) + 1;
            Bitmap image = new Bitmap(imageWidth, imageHeight);
            Graphics gs = Graphics.FromImage(image);
            gs.FillRectangle(new SolidBrush(Color.White), 0, 0, imageWidth, imageHeight);
            gs.DrawRectangle(new Pen(Color.Black), 0, 0, imageWidth - 1, imageHeight - 1);
            for (int i = 0; i < strArray.Length; i++)
            {
                gs.DrawString(strArray[i], font, brush, emptyWidth, i * height + (i + 1) * emptyWidth);
            }
            gs.Dispose();
            return image;
        }

        private static void GetMaxWidthAndHeight(ref float width, ref float height, float fontSize, params string[] strArray)
        {
            List<float> list = new List<float>();
            foreach (string str in strArray)
            {
                list.Add(GetLength(fontSize, str));
            }
            width = list.Max();
            height = fontSize * 3 / 2;
        }

        private static float GetLength(float fontSize, string str)
        {
            float eng = fontSize;
            float cha = eng * 3 / 2;
            float num = cha / 2;


            char[] charArray = str.ToCharArray();
            int chinaCount = 0;
            int numCount = 0;
            int engCount = 0;
            int chinaCharCount = 0;
            int engCharCount = 0;

            foreach (char c in charArray)
            {
                if (c >= '0' && c <= '9')
                {
                    numCount++;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    engCount++;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    engCount++;
                }
                else if (c >= 0x4E00 && c <= 0x9FA5)
                {
                    chinaCount++;
                }
                else if (c == 0x3002)//。
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF1F)//？
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF01)//！
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF01)//	！
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF0C)//	，
                {
                    chinaCharCount++;
                }
                else if (c == 0x3001)//	、
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF1B)//	；
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF1A)//	：
                {
                    chinaCharCount++;
                }
                else if (c == 0x300C)//	「
                {
                    chinaCharCount++;
                }
                else if (c == 0x300D)//	」
                {
                    chinaCharCount++;
                }
                else if (c == 0x300E)//	『
                {
                    chinaCharCount++;
                }
                else if (c == 0x300F)//	』
                {
                    chinaCharCount++;
                }
                else if (c == 0x2018)//	‘
                {
                    chinaCharCount++;
                }
                else if (c == 0x2019)//	’
                {
                    chinaCharCount++;
                }
                else if (c == 0x201C)//	“
                {
                    chinaCharCount++;
                }
                else if (c == 0x201D)//	”
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF08)//	（
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF09)//	）
                {
                    chinaCharCount++;
                }
                else if (c == 0x3014)//	〔
                {
                    chinaCharCount++;
                }
                else if (c == 0x3015)//	〕
                {
                    chinaCharCount++;
                }
                else if (c == 0x3010)//	【
                {
                    chinaCharCount++;
                }
                else if (c == 0x3011)//	】
                {
                    chinaCharCount++;
                }
                else if (c == 0x2014)//	—
                {
                    chinaCharCount++;
                }
                else if (c == 0x2026)//	…
                {
                    chinaCharCount++;
                }
                else if (c == 0x2013)//	–
                {
                    chinaCharCount++;
                }
                else if (c == 0xFF0E)//	．
                {
                    chinaCharCount++;
                }
                else if (c == 0x300A)//	《
                {
                    chinaCharCount++;
                }
                else if (c == 0x300B)//	》
                {
                    chinaCharCount++;
                }
                else if (c == 0x3008)//	〈
                {
                    chinaCharCount++;
                }
                else if (c == 0x3009)//	〉
                {
                    chinaCharCount++;
                }
                else
                {
                    engCharCount++;
                }

            }
            return chinaCount * cha + numCount * num + engCount * eng + engCharCount * eng + chinaCharCount * cha;

        }
    }
}

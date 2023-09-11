using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace FNMES.Utility.Files.ICOGen
{
    public class IconDir
    {
        private ushort idReserved = 0;
        private ushort idType = 1;
        private ushort idCount = 1;
        private IList<IconDirentry> identries = new List<IconDirentry>();
        public IconDir()
        {
        }
        public IconDir(string iconFile)
        {
            FileStream fileStream = new FileStream(iconFile, FileMode.Open);
            byte[] iconData = new byte[fileStream.Length];
            fileStream.Read(iconData, 0, iconData.Length);
            fileStream.Close();
            LoadData(iconData);
        }
        /// <summary>
        /// 读取ICO
        /// </summary>
        /// <param name="iconData"></param>
        private void LoadData(byte[] iconData)
        {
            idReserved = BitConverter.ToUInt16(iconData, 0);
            idType = BitConverter.ToUInt16(iconData, 2);
            idCount = BitConverter.ToUInt16(iconData, 4);
            if (idType != 1 || idReserved != 0) return;
            int readIndex = 6;
            for (ushort i = 0; i != idCount; i++)
            {
                identries.Add(new IconDirentry(iconData, ref readIndex));
            }

        }

        /// <summary>
        /// 保存ICO
        /// </summary>
        /// <param name="fileName"></param>
        public Stream SaveData()
        {
            byte[] retByte;
            if (ImageCount == 0) return null;
            MemoryStream memoryStream = new MemoryStream();
            byte[] temp = BitConverter.GetBytes(idReserved);
            memoryStream.Write(temp, 0, temp.Length);
            temp = BitConverter.GetBytes(idType);
            memoryStream.Write(temp, 0, temp.Length);
            temp = BitConverter.GetBytes((ushort)ImageCount);
            memoryStream.Write(temp, 0, temp.Length);
            for (int i = 0; i != ImageCount; i++)
            {
                memoryStream.WriteByte(identries[i].Width);
                memoryStream.WriteByte(identries[i].Height);
                memoryStream.WriteByte(identries[i].ColorCount);
                memoryStream.WriteByte(identries[i].Breserved);
                temp = BitConverter.GetBytes(identries[i].Planes);
                memoryStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].Bitcount);
                memoryStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].ImageSize);
                memoryStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].ImageRVA);
                memoryStream.Write(temp, 0, temp.Length);
            }
            for (int i = 0; i != ImageCount; i++)
            {
                memoryStream.Write(identries[i].ImageData, 0, identries[i].ImageData.Length);
            }
            return memoryStream;
        }

        /// <summary>
        /// 保存ICO
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveData(string fileName)
        {
            if (ImageCount == 0) return;
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            byte[] temp = BitConverter.GetBytes(idReserved);
            fileStream.Write(temp, 0, temp.Length);
            temp = BitConverter.GetBytes(idType);
            fileStream.Write(temp, 0, temp.Length);
            temp = BitConverter.GetBytes((ushort)ImageCount);
            fileStream.Write(temp, 0, temp.Length);
            for (int i = 0; i != ImageCount; i++)
            {
                fileStream.WriteByte(identries[i].Width);
                fileStream.WriteByte(identries[i].Height);
                fileStream.WriteByte(identries[i].ColorCount);
                fileStream.WriteByte(identries[i].Breserved);
                temp = BitConverter.GetBytes(identries[i].Planes);
                fileStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].Bitcount);
                fileStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].ImageSize);
                fileStream.Write(temp, 0, temp.Length);
                temp = BitConverter.GetBytes(identries[i].ImageRVA);
                fileStream.Write(temp, 0, temp.Length);
            }
            for (int i = 0; i != ImageCount; i++)
            {
                fileStream.Write(identries[i].ImageData, 0, identries[i].ImageData.Length);
            }
            fileStream.Close();
        }
        /// <summary>
        /// 根据索引返回图形
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bitmap GetImage(int index)
        {
            int ReadIndex = 0;
            BitmapInfo MyBitmap = new BitmapInfo(identries[index].ImageData, ref ReadIndex);
            return MyBitmap.IconBmp;
        }
        public void AddImage(Image setBitmap, Rectangle setRectangle)
        {
            if (setRectangle.Width > 255 || setRectangle.Height > 255) return;


            Bitmap iconBitmap = new Bitmap(setRectangle.Width, setRectangle.Height);
            Graphics g = Graphics.FromImage(iconBitmap);
            g.DrawImage(setBitmap, new Rectangle(0, 0, iconBitmap.Width, iconBitmap.Height), setRectangle, GraphicsUnit.Pixel);
            g.Dispose();
            MemoryStream bmpMemory = new MemoryStream();
            iconBitmap.Save(bmpMemory, ImageFormat.Bmp);

            IconDirentry newIconDirentry = new IconDirentry();
            bmpMemory.Position = 14;        //只使用13位后的数字 40开头 
            newIconDirentry.ImageData = new byte[bmpMemory.Length - 14 + 128];
            bmpMemory.Read(newIconDirentry.ImageData, 0, newIconDirentry.ImageData.Length);
            newIconDirentry.Width = (byte)setRectangle.Width;
            newIconDirentry.Height = (byte)setRectangle.Height;
            //BMP图形和ICO的高不一样  ICO的高是BMP的2倍
            byte[] Height = BitConverter.GetBytes((uint)newIconDirentry.Height * 2);
            newIconDirentry.ImageData[8] = Height[0];
            newIconDirentry.ImageData[9] = Height[1];
            newIconDirentry.ImageData[10] = Height[2];
            newIconDirentry.ImageData[11] = Height[3];


            newIconDirentry.ImageSize = (uint)newIconDirentry.ImageData.Length;
            identries.Add(newIconDirentry);
            uint rvaIndex = 6 + (uint)(identries.Count * 16);
            for (int i = 0; i != identries.Count; i++)
            {
                identries[i].ImageRVA = rvaIndex;
                rvaIndex += identries[i].ImageSize;
            }
        }
        public void DelImage(int index)
        {

            identries.RemoveAt(index);
            uint rvaIndex = 6 + (uint)(identries.Count * 16);
            for (int i = 0; i != identries.Count; i++)
            {
                identries[i].ImageRVA = rvaIndex;
                rvaIndex += identries[i].ImageSize;
            }
        }
        /// <summary>
        /// 返回图形数量
        /// </summary>
        public int ImageCount { get { return identries.Count; } }
    }
}

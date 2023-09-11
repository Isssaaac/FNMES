using System;
using System.IO;

namespace FNMES.Utility.Files.ICOGen
{
    public class IconDirentry
    {
        public IconDirentry()
        {
        }
        public IconDirentry(byte[] iconDate, ref int readIndex)
        {
            bWidth = iconDate[readIndex];
            readIndex++;
            bHeight = iconDate[readIndex];
            readIndex++;
            bColorCount = iconDate[readIndex];
            readIndex++;
            breserved = iconDate[readIndex];
            readIndex++;
            wplanes = BitConverter.ToUInt16(iconDate, readIndex);
            readIndex += 2;
            wbitcount = BitConverter.ToUInt16(iconDate, readIndex);
            readIndex += 2;
            dwbytesinres = BitConverter.ToUInt32(iconDate, readIndex);
            readIndex += 4;
            dwimageoffset = BitConverter.ToUInt32(iconDate, readIndex);
            readIndex += 4;
            MemoryStream memoryData = new MemoryStream(iconDate, (int)dwimageoffset, (int)dwbytesinres);
            imageData = new byte[dwbytesinres];
            memoryData.Read(imageData, 0, imageData.Length);
        }
        private byte bWidth = 16;
        private byte bHeight = 16;
        private byte bColorCount = 0;
        private byte breserved = 0;        //4 
        private ushort wplanes = 1;
        private ushort wbitcount = 32;      //8
        private uint dwbytesinres = 0;
        private uint dwimageoffset = 0;         //16
        private byte[] imageData;

        /// <summary>
        /// 图像宽度，以象素为单位。一个字节
        /// </summary>
        public byte Width { get { return bWidth; } set { bWidth = value; } }
        /// <summary>
        /// 图像高度，以象素为单位。一个字节  
        /// </summary>
        public byte Height { get { return bHeight; } set { bHeight = value; } }
        /// <summary>
        /// 图像中的颜色数（如果是>=8bpp的位图则为0）
        /// </summary>
        public byte ColorCount { get { return bColorCount; } set { bColorCount = value; } }
        /// <summary>
        /// 保留字必须是0
        /// </summary>
        public byte Breserved { get { return breserved; } set { breserved = value; } }
        /// <summary>
        /// 为目标设备说明位面数，其值将总是被设为1
        /// </summary>
        public ushort Planes { get { return wplanes; } set { wplanes = value; } }
        /// <summary>
        /// 每象素所占位数。
        /// </summary>
        public ushort Bitcount { get { return wbitcount; } set { wbitcount = value; } }
        /// <summary>
        /// 字节大小。
        /// </summary>
        public uint ImageSize { get { return dwbytesinres; } set { dwbytesinres = value; } }
        /// <summary>
        /// 起点偏移位置。
        /// </summary>
        public uint ImageRVA { get { return dwimageoffset; } set { dwimageoffset = value; } }
        /// <summary>
        /// 图形数据
        /// </summary>
        public byte[] ImageData { get { return imageData; } set { imageData = value; } }

    }
}
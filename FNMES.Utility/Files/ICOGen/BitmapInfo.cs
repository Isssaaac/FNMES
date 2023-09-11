using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FNMES.Utility.Files.ICOGen
{
    public class BitmapInfo
    {
        private uint biSize = 40;
        private ushort biPlanes = 1;
        private uint biCompression = 0;
        public IList<Color> ColorTable = new List<Color>();
        /// <summary>
        /// 占4位，位图信息头(Bitmap Info Header)的长度,一般为$28  
        /// </summary>
        public uint InfoSize { get { return biSize; } set { biSize = value; } }
        /// <summary>
        /// 占4位，位图的宽度，以象素为单位
        /// </summary>
        public uint Width { get; set; }
        /// <summary>
        /// 占4位，位图的高度，以象素为单位  
        /// </summary>
        public uint Height { get; set; }
        /// <summary>
        /// 占2位，位图的位面数（注：该值将总是1）  
        /// </summary>
        public ushort Planes { get { return biPlanes; } set { biPlanes = value; } }
        /// <summary>
        /// 占2位，每个象素的位数，设为32(32Bit位图)  
        /// </summary>
        public ushort BitCount { get; set; }
        /// <summary>
        /// 占4位，压缩说明，设为0(不压缩)   
        /// </summary>
        public uint Compression { get { return biCompression; } set { biCompression = value; } }
        /// <summary>
        /// 占4位，用字节数表示的位图数据的大小。该数必须是4的倍数  
        /// </summary>
        public uint SizeImage { get; set; }
        /// <summary>
        ///  占4位，用象素/米表示的水平分辨率 
        /// </summary>
        public uint XPelsPerMeter { get; set; }
        /// <summary>
        /// 占4位，用象素/米表示的垂直分辨率  
        /// </summary>
        public uint YPelsPerMeter { get; set; }
        /// <summary>
        /// 占4位，位图使用的颜色数  
        /// </summary>
        public uint ClrUsed { get; set; }
        /// <summary>
        /// 占4位，指定重要的颜色数(到此处刚好40个字节，$28)  
        /// </summary>
        public uint ClrImportant { get; set; }
        private Bitmap _IconBitMap;
        /// <summary>
        /// 图形
        /// </summary>
        public Bitmap IconBmp { get { return _IconBitMap; } set { _IconBitMap = value; } }
        public BitmapInfo(byte[] ImageData, ref int ReadIndex)
        {
            #region 基本数据
            biSize = BitConverter.ToUInt32(ImageData, ReadIndex);
            if (biSize != 40) return;
            ReadIndex += 4;
            Width = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            Height = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            biPlanes = BitConverter.ToUInt16(ImageData, ReadIndex);
            ReadIndex += 2;
            BitCount = BitConverter.ToUInt16(ImageData, ReadIndex);
            ReadIndex += 2;
            biCompression = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            SizeImage = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            XPelsPerMeter = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            YPelsPerMeter = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            ClrUsed = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            ClrImportant = BitConverter.ToUInt32(ImageData, ReadIndex);
            ReadIndex += 4;
            #endregion
            int ColorCount = RgbCount();
            if (ColorCount == -1) return;
            for (int i = 0; i != ColorCount; i++)
            {
                byte Blue = ImageData[ReadIndex];
                byte Green = ImageData[ReadIndex + 1];
                byte Red = ImageData[ReadIndex + 2];
                byte Reserved = ImageData[ReadIndex + 3];
                ColorTable.Add(Color.FromArgb((int)Reserved, (int)Red, (int)Green, (int)Blue));
                ReadIndex += 4;
            }
            int Size = (int)(BitCount * Width) / 8;       // 象素的大小*象素数 /字节数              
            if ((double)Size < BitCount * Width / 8) Size++;       //如果是 宽19*4（16色）/8 =9.5 就+1;
            if (Size < 4) Size = 4;
            byte[] WidthByte = new byte[Size];
            _IconBitMap = new Bitmap((int)Width, (int)(Height / 2));
            for (int i = (int)(Height / 2); i != 0; i--)
            {
                for (int z = 0; z != Size; z++)
                {
                    WidthByte[z] = ImageData[ReadIndex + z];
                }
                ReadIndex += Size;
                IconSet(_IconBitMap, i - 1, WidthByte);
            }

            //取掩码
            int MaskSize = (int)(Width / 8);
            if ((double)MaskSize < Width / 8) MaskSize++;       //如果是 宽19*4（16色）/8 =9.5 就+1;
            if (MaskSize < 4) MaskSize = 4;
            byte[] MashByte = new byte[MaskSize];
            for (int i = (int)(Height / 2); i != 0; i--)
            {
                for (int z = 0; z != MaskSize; z++)
                {
                    MashByte[z] = ImageData[ReadIndex + z];
                }
                ReadIndex += MaskSize;
                IconMask(_IconBitMap, i - 1, MashByte);
            }

        }
        private int RgbCount()
        {
            switch (BitCount)
            {
                case 1: return 2;
                case 4: return 16;
                case 8: return 256;
                case 24: return 0;
                case 32: return 0;
                default: return -1;
            }
        }
        private void IconSet(Bitmap IconImage, int RowIndex, byte[] ImageByte)
        {
            int Index = 0;
            switch (BitCount)
            {
                case 1:
                    #region 一次读8位 绘制8个点
                    for (int i = 0; i != ImageByte.Length; i++)
                    {
                        System.Collections.BitArray MyArray = new System.Collections.BitArray(new byte[] { ImageByte[i] });
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[7])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[6])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[5])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[4])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[3])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[2])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[1])]);
                        Index++;
                        if (Index >= IconImage.Width)
                            return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[GetBitNumb(MyArray[0])]);
                        Index++;
                    }
                    #endregion
                    break;
                case 4:
                    #region 一次读8位 绘制2个点
                    for (int i = 0; i != ImageByte.Length; i++)
                    {
                        int High = ImageByte[i] >> 4;  //取高4位
                        int Low = ImageByte[i] - (High << 4); //取低4位
                        if (Index >= IconImage.Width) return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[High]);
                        Index++;
                        if (Index >= IconImage.Width) return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[Low]);
                        Index++;
                    }
                    #endregion
                    break;
                case 8:
                    #region 一次读8位 绘制一个点
                    for (int i = 0; i != ImageByte.Length; i++)
                    {
                        if (Index >= IconImage.Width) return;
                        IconImage.SetPixel(Index, RowIndex, ColorTable[ImageByte[i]]);
                        Index++;
                    }
                    #endregion
                    break;
                case 24:
                    #region 一次读24位 绘制一个点

                    for (int i = 0; i != ImageByte.Length / 3; i++)
                    {
                        if (i >= IconImage.Width) return;
                        IconImage.SetPixel(i, RowIndex, Color.FromArgb(ImageByte[Index + 2], ImageByte[Index + 1], ImageByte[Index]));
                        Index += 3;
                    }
                    #endregion
                    break;
                case 32:
                    #region 一次读32位 绘制一个点

                    for (int i = 0; i != ImageByte.Length / 4; i++)
                    {
                        if (i >= IconImage.Width) return;

                        IconImage.SetPixel(i, RowIndex, Color.FromArgb(ImageByte[Index + 2], ImageByte[Index + 1], ImageByte[Index]));
                        Index += 4;
                    }
                    #endregion
                    break;
                default:
                    break;
            }
        }
        private void IconMask(Bitmap IconImage, int RowIndex, byte[] MaskByte)
        {
            System.Collections.BitArray Set = new System.Collections.BitArray(MaskByte);
            int ReadIndex = 0;
            for (int i = Set.Count; i != 0; i--)
            {
                if (ReadIndex >= IconImage.Width) return;
                Color SetColor = IconImage.GetPixel(ReadIndex, RowIndex);
                if (!Set[i - 1]) IconImage.SetPixel(ReadIndex, RowIndex, Color.FromArgb(255, SetColor.R, SetColor.G, SetColor.B));
                ReadIndex++;
            }
        }
        private int GetBitNumb(bool BitArray)
        {
            if (BitArray)
                return 1;
            return 0;
        }
    }
}

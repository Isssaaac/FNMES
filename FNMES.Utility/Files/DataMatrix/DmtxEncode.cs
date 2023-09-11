using System;
using System.Collections.Generic;
using System.Drawing;

namespace DataMatrix.net
{
    // Token: 0x0200001B RID: 27
    internal class DmtxEncode
    {
        // Token: 0x060000CE RID: 206 RVA: 0x0000590C File Offset: 0x00003B0C
        internal DmtxEncode()
        {
            this._scheme = DmtxScheme.DmtxSchemeAscii;
            this._sizeIdxRequest = DmtxSymbolSize.DmtxSymbolSquareAuto;
            this._marginSize = 10;
            this._moduleSize = 5;
            this._pixelPacking = DmtxPackOrder.DmtxPack24bppRGB;
            this._imageFlip = DmtxFlip.DmtxFlipNone;
            this._rowPadBytes = 0;
        }

        // Token: 0x060000CF RID: 207 RVA: 0x0000595C File Offset: 0x00003B5C
        private DmtxEncode(DmtxEncode src)
        {
            this._scheme = src._scheme;
            this._sizeIdxRequest = src._sizeIdxRequest;
            this._marginSize = src._marginSize;
            this._moduleSize = src._moduleSize;
            this._pixelPacking = src._pixelPacking;
            this._imageFlip = src._imageFlip;
            this._rowPadBytes = src._rowPadBytes;
            this._image = src._image;
            this._message = src._message;
            this._method = src._method;
            this._region = src._region;
        }

        // Token: 0x060000D0 RID: 208 RVA: 0x000059F8 File Offset: 0x00003BF8
        internal bool EncodeDataMatrixRaw(byte[] inputString)
        {
            return this.EncodeDataMatrix(null, null, inputString, true);
        }

        // Token: 0x060000D1 RID: 209 RVA: 0x00005A24 File Offset: 0x00003C24
        internal bool EncodeDataMatrix(Color? foreColor, Color? backColor, byte[] inputString)
        {
            return this.EncodeDataMatrix(foreColor, backColor, inputString, false);
        }

        // Token: 0x060000D2 RID: 210 RVA: 0x00005A40 File Offset: 0x00003C40
        internal bool EncodeDataMatrix(Color? foreColor, Color? backColor, byte[] inputString, bool encodeRaw)
        {
            byte[] array = new byte[4096];
            DmtxSymbolSize sizeIdxRequest = this._sizeIdxRequest;
            int num = this.EncodeDataCodewords(array, inputString, ref sizeIdxRequest);
            bool result;
            if (num <= 0)
            {
                result = false;
            }
            else
            {
                if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto || sizeIdxRequest == DmtxSymbolSize.DmtxSymbolRectAuto)
                {
                    throw new Exception("Invalid symbol size for encoding!");
                }
                int padCount = this.AddPadChars(array, ref num, DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdxRequest));
                this._region = new DmtxRegion();
                this._region.SizeIdx = sizeIdxRequest;
                this._region.SymbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdxRequest);
                this._region.SymbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, sizeIdxRequest);
                this._region.MappingRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, sizeIdxRequest);
                this._region.MappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdxRequest);
                this._message = new DmtxMessage(sizeIdxRequest, DmtxFormat.Matrix)
                {
                    PadCount = padCount
                };
                for (int i = 0; i < num; i++)
                {
                    this._message.Code[i] = array[i];
                }
                DmtxCommon.GenReedSolEcc(this._message, this._region.SizeIdx);
                DmtxDecode.ModulePlacementEcc200(this._message.Array, this._message.Code, this._region.SizeIdx, DmtxConstants.DmtxModuleOnRGB);
                int num2 = 2 * this._marginSize + this._region.SymbolCols * this._moduleSize;
                int num3 = 2 * this._marginSize + this._region.SymbolRows * this._moduleSize;
                int bitsPerPixel = DmtxCommon.GetBitsPerPixel(this._pixelPacking);
                if (bitsPerPixel == DmtxConstants.DmtxUndefined)
                {
                    result = false;
                }
                else
                {
                    if (bitsPerPixel % 8 != 0)
                    {
                        throw new Exception("Invalid color depth for encoding!");
                    }
                    byte[] pxl = new byte[num2 * num3 * (bitsPerPixel / 8) + this._rowPadBytes];
                    this._image = new DmtxImage(pxl, num2, num3, this._pixelPacking)
                    {
                        ImageFlip = this._imageFlip,
                        RowPadBytes = this._rowPadBytes
                    };
                    if (encodeRaw)
                    {
                        this.PrintPatternRaw();
                    }
                    else
                    {
                        this.PrintPattern(foreColor, backColor);
                    }
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x060000D3 RID: 211 RVA: 0x00005C8C File Offset: 0x00003E8C
        internal bool EncodeDataMosaic(byte[] inputString)
        {
            int[] array = new int[3];
            List<byte[]> list = new List<byte[]>(3);
            for (int i = 0; i < 3; i++)
            {
                list.Add(new byte[4096]);
            }
            DmtxSymbolSize dmtxSymbolSize2;
            DmtxSymbolSize dmtxSymbolSize = dmtxSymbolSize2 = this._sizeIdxRequest;
            int num = this.EncodeDataCodewords(list[0], inputString, ref dmtxSymbolSize2);
            bool result;
            if (num <= 0)
            {
                result = false;
            }
            else
            {
                int num2 = (inputString.Length + 2) / 3;
                array[0] = num2;
                array[1] = num2;
                array[2] = inputString.Length - (array[0] + array[1]);
                DmtxSymbolSize dmtxSymbolSize3 = this.FindCorrectSymbolSize(num2, dmtxSymbolSize);
                if (dmtxSymbolSize3 == DmtxSymbolSize.DmtxSymbolShapeAuto)
                {
                    result = false;
                }
                else
                {
                    DmtxSymbolSize dmtxSymbolSize4;
                    if (dmtxSymbolSize == DmtxSymbolSize.DmtxSymbolSquareAuto)
                    {
                        dmtxSymbolSize4 = DmtxSymbolSize.DmtxSymbol144x144;
                    }
                    else if (dmtxSymbolSize == DmtxSymbolSize.DmtxSymbolRectAuto)
                    {
                        dmtxSymbolSize4 = DmtxSymbolSize.DmtxSymbol16x48;
                    }
                    else
                    {
                        dmtxSymbolSize4 = dmtxSymbolSize3;
                    }
                    byte[] array2 = new byte[array[0]];
                    for (int i = 0; i < array[0]; i++)
                    {
                        array2[i] = inputString[i];
                    }
                    byte[] array3 = new byte[array[1]];
                    for (int i = array[0]; i < array[0] + array[1]; i++)
                    {
                        array3[i - array[0]] = inputString[i];
                    }
                    byte[] array4 = new byte[array[2]];
                    for (int i = array[0] + array[1]; i < inputString.Length; i++)
                    {
                        array4[i - array[0] - array[1]] = inputString[i];
                    }
                    DmtxSymbolSize dmtxSymbolSize5;
                    for (dmtxSymbolSize5 = dmtxSymbolSize3; dmtxSymbolSize5 <= dmtxSymbolSize4; dmtxSymbolSize5++)
                    {
                        dmtxSymbolSize2 = dmtxSymbolSize5;
                        this.EncodeDataCodewords(list[0], array2, ref dmtxSymbolSize2);
                        if (dmtxSymbolSize2 == dmtxSymbolSize5)
                        {
                            dmtxSymbolSize2 = dmtxSymbolSize5;
                            this.EncodeDataCodewords(list[1], array3, ref dmtxSymbolSize2);
                            if (dmtxSymbolSize2 == dmtxSymbolSize5)
                            {
                                dmtxSymbolSize2 = dmtxSymbolSize5;
                                this.EncodeDataCodewords(list[2], array4, ref dmtxSymbolSize2);
                                if (dmtxSymbolSize2 == dmtxSymbolSize5)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    this._sizeIdxRequest = dmtxSymbolSize5;
                    DmtxEncode dmtxEncode = new DmtxEncode(this);
                    DmtxEncode dmtxEncode2 = new DmtxEncode(this);
                    this.EncodeDataMatrix(null, null, array2);
                    dmtxEncode.EncodeDataMatrix(null, null, array3);
                    dmtxEncode2.EncodeDataMatrix(null, null, array4);
                    int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, dmtxSymbolSize5);
                    int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, dmtxSymbolSize5);
                    for (int i = 0; i < this._region.MappingCols * this._region.MappingRows; i++)
                    {
                        this._message.Array[i] = 0;
                    }
                    DmtxDecode.ModulePlacementEcc200(this._message.Array, this._message.Code, this._region.SizeIdx, DmtxConstants.DmtxModuleOnRed);
                    for (int j = 0; j < symbolAttribute; j++)
                    {
                        for (int k = 0; k < symbolAttribute2; k++)
                        {
                            byte[] array5 = this._message.Array;
                            int num3 = j * symbolAttribute2 + k;
                            array5[num3] &= (byte)(255 ^ (DmtxConstants.DmtxModuleAssigned | DmtxConstants.DmtxModuleVisited));
                        }
                    }
                    DmtxDecode.ModulePlacementEcc200(this._message.Array, dmtxEncode.Message.Code, this._region.SizeIdx, DmtxConstants.DmtxModuleOnGreen);
                    for (int j = 0; j < symbolAttribute; j++)
                    {
                        for (int k = 0; k < symbolAttribute2; k++)
                        {
                            byte[] array6 = this._message.Array;
                            int num4 = j * symbolAttribute2 + k;
                            array6[num4] &= (byte)(255 ^ (DmtxConstants.DmtxModuleAssigned | DmtxConstants.DmtxModuleVisited));
                        }
                    }
                    DmtxDecode.ModulePlacementEcc200(this._message.Array, dmtxEncode2.Message.Code, this._region.SizeIdx, DmtxConstants.DmtxModuleOnBlue);
                    this.PrintPattern(null, null);
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x060000D4 RID: 212 RVA: 0x000060D0 File Offset: 0x000042D0
        private void PrintPatternRaw()
        {
            this._rawData = new bool[this._region.SymbolCols, this._region.SymbolRows];
            for (int i = 0; i < this._region.SymbolRows; i++)
            {
                for (int j = 0; j < this._region.SymbolCols; j++)
                {
                    int num = this._message.SymbolModuleStatus(this._region.SizeIdx, i, j);
                    this._rawData[j, this._region.SymbolRows - i - 1] = ((num & DmtxConstants.DmtxModuleOnBlue) != 0);
                }
            }
        }

        // Token: 0x060000D5 RID: 213 RVA: 0x00006178 File Offset: 0x00004378
        private void PrintPattern(Color? foreColor, Color? backColor)
        {
            int[] array = new int[3];
            double num = (double)this._marginSize;
            DmtxMatrix3 m = DmtxMatrix3.Translate(num, num);
            DmtxMatrix3 m2 = DmtxMatrix3.Scale((double)this._moduleSize, (double)this._moduleSize);
            DmtxMatrix3 matrix = m2 * m;
            int rowSizeBytes = this._image.RowSizeBytes;
            int height = this._image.Height;
            for (int i = 0; i < rowSizeBytes * height; i++)
            {
                this._image.Pxl[i] = byte.MaxValue;
            }
            for (int j = 0; j < this._region.SymbolRows; j++)
            {
                for (int k = 0; k < this._region.SymbolCols; k++)
                {
                    DmtxVector2 vector = new DmtxVector2((double)k, (double)j);
                    DmtxVector2 dmtxVector = vector * matrix;
                    int num2 = (int)dmtxVector.X;
                    int num3 = (int)dmtxVector.Y;
                    int num4 = this._message.SymbolModuleStatus(this._region.SizeIdx, j, k);
                    for (int l = num3; l < num3 + this._moduleSize; l++)
                    {
                        for (int n = num2; n < num2 + this._moduleSize; n++)
                        {
                            if (foreColor != null && backColor != null)
                            {
                                array[0] = (int)(((num4 & DmtxConstants.DmtxModuleOnRed) != 0) ? foreColor.Value.B : backColor.Value.B);
                                array[1] = (int)(((num4 & DmtxConstants.DmtxModuleOnGreen) != 0) ? foreColor.Value.G : backColor.Value.G);
                                array[2] = (int)(((num4 & DmtxConstants.DmtxModuleOnBlue) != 0) ? foreColor.Value.R : backColor.Value.R);
                            }
                            else
                            {
                                array[0] = (((num4 & DmtxConstants.DmtxModuleOnBlue) != 0) ? 0 : 255);
                                array[1] = (((num4 & DmtxConstants.DmtxModuleOnGreen) != 0) ? 0 : 255);
                                array[2] = (((num4 & DmtxConstants.DmtxModuleOnRed) != 0) ? 0 : 255);
                            }
                            this._image.SetPixelValue(n, l, 0, (byte)array[0]);
                            this._image.SetPixelValue(n, l, 1, (byte)array[1]);
                            this._image.SetPixelValue(n, l, 2, (byte)array[2]);
                        }
                    }
                }
            }
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x00006418 File Offset: 0x00004618
        private int AddPadChars(byte[] buf, ref int dataWordCount, int paddedSize)
        {
            int num = 0;
            if (dataWordCount < paddedSize)
            {
                num++;
                buf[dataWordCount++] = DmtxConstants.DmtxCharAsciiPad;
            }
            while (dataWordCount < paddedSize)
            {
                num++;
                buf[dataWordCount] = this.Randomize253State(DmtxConstants.DmtxCharAsciiPad, dataWordCount + 1);
                dataWordCount++;
            }
            return num;
        }

        // Token: 0x060000D7 RID: 215 RVA: 0x0000647C File Offset: 0x0000467C
        private byte Randomize253State(byte codewordValue, int codewordPosition)
        {
            int num = 149 * codewordPosition % 253 + 1;
            int num2 = (int)codewordValue + num;
            if (num2 > 254)
            {
                num2 -= 254;
            }
            if (num2 < 0 || num2 > 255)
            {
                throw new Exception("Error randomizing 253 state!");
            }
            return (byte)num2;
        }

        // Token: 0x060000D8 RID: 216 RVA: 0x000064E0 File Offset: 0x000046E0
        private int EncodeDataCodewords(byte[] buf, byte[] inputString, ref DmtxSymbolSize sizeIdx)
        {
            int num;
            switch (this._scheme)
            {
                case DmtxScheme.DmtxSchemeAutoFast:
                    num = 0;
                    break;
                case DmtxScheme.DmtxSchemeAutoBest:
                    num = this.EncodeAutoBest(buf, inputString);
                    break;
                default:
                    num = this.EncodeSingleScheme(buf, inputString, this._scheme);
                    break;
            }
            sizeIdx = this.FindCorrectSymbolSize(num, sizeIdx);
            int result;
            if (sizeIdx == DmtxSymbolSize.DmtxSymbolShapeAuto)
            {
                result = 0;
            }
            else
            {
                result = num;
            }
            return result;
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00006548 File Offset: 0x00004748
        private DmtxSymbolSize FindCorrectSymbolSize(int dataWords, DmtxSymbolSize sizeIdxRequest)
        {
            DmtxSymbolSize result;
            if (dataWords <= 0)
            {
                result = DmtxSymbolSize.DmtxSymbolShapeAuto;
            }
            else
            {
                DmtxSymbolSize dmtxSymbolSize;
                if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto || sizeIdxRequest == DmtxSymbolSize.DmtxSymbolRectAuto)
                {
                    int num;
                    int num2;
                    if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto)
                    {
                        num = 0;
                        num2 = DmtxConstants.DmtxSymbolSquareCount;
                    }
                    else
                    {
                        num = DmtxConstants.DmtxSymbolSquareCount;
                        num2 = DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount;
                    }
                    for (dmtxSymbolSize = (DmtxSymbolSize)num; dmtxSymbolSize < (DmtxSymbolSize)num2; dmtxSymbolSize++)
                    {
                        if (DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize) >= dataWords)
                        {
                            break;
                        }
                    }
                    if (dmtxSymbolSize == (DmtxSymbolSize)num2)
                    {
                        return DmtxSymbolSize.DmtxSymbolShapeAuto;
                    }
                }
                else
                {
                    dmtxSymbolSize = sizeIdxRequest;
                }
                if (dataWords > DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize))
                {
                    result = DmtxSymbolSize.DmtxSymbolShapeAuto;
                }
                else
                {
                    result = dmtxSymbolSize;
                }
            }
            return result;
        }

        // Token: 0x060000DA RID: 218 RVA: 0x0000660C File Offset: 0x0000480C
        private int EncodeSingleScheme(byte[] buf, byte[] codewords, DmtxScheme scheme)
        {
            DmtxChannel dmtxChannel = new DmtxChannel();
            DmtxEncode.InitChannel(dmtxChannel, codewords);
            while (dmtxChannel.InputIndex < dmtxChannel.Input.Length)
            {
                int result;
                if (!this.EncodeNextWord(dmtxChannel, scheme))
                {
                    result = 0;
                }
                else
                {
                    if (dmtxChannel.Invalid == DmtxChannelStatus.DmtxChannelValid)
                    {
                        continue;
                    }
                    result = 0;
                }
                return result;
            }
            int num = dmtxChannel.EncodedLength / 12;
            for (int i = 0; i < num; i++)
            {
                buf[i] = dmtxChannel.EncodedWords[i];
            }
            return num;
        }

        // Token: 0x060000DB RID: 219 RVA: 0x0000669C File Offset: 0x0000489C
        private int EncodeAutoBest(byte[] buf, byte[] codewords)
        {
            DmtxChannelGroup dmtxChannelGroup = new DmtxChannelGroup();
            DmtxChannelGroup dmtxChannelGroup2 = new DmtxChannelGroup();
            for (DmtxScheme dmtxScheme = DmtxScheme.DmtxSchemeAscii; dmtxScheme <= DmtxScheme.DmtxSchemeBase256; dmtxScheme++)
            {
                DmtxChannel channel = dmtxChannelGroup.Channels[(int)dmtxScheme];
                DmtxEncode.InitChannel(channel, codewords);
                bool flag = this.EncodeNextWord(channel, dmtxScheme);
                if (flag)
                {
                    return 0;
                }
            }
            while (dmtxChannelGroup.Channels[0].InputIndex < dmtxChannelGroup.Channels[0].Input.Length)
            {
                for (DmtxScheme dmtxScheme = DmtxScheme.DmtxSchemeAscii; dmtxScheme <= DmtxScheme.DmtxSchemeBase256; dmtxScheme++)
                {
                    dmtxChannelGroup2.Channels[(int)dmtxScheme] = this.FindBestChannel(dmtxChannelGroup, dmtxScheme);
                }
                dmtxChannelGroup = dmtxChannelGroup2;
            }
            DmtxChannel dmtxChannel = dmtxChannelGroup.Channels[0];
            for (DmtxScheme dmtxScheme = DmtxScheme.DmtxSchemeC40; dmtxScheme <= DmtxScheme.DmtxSchemeBase256; dmtxScheme++)
            {
                if (dmtxChannelGroup.Channels[(int)dmtxScheme].Invalid == DmtxChannelStatus.DmtxChannelValid)
                {
                    if (dmtxChannelGroup.Channels[(int)dmtxScheme].EncodedLength < dmtxChannel.EncodedLength)
                    {
                        dmtxChannel = dmtxChannelGroup.Channels[(int)dmtxScheme];
                    }
                }
            }
            int num = dmtxChannel.EncodedLength / 12;
            for (int i = 0; i < num; i++)
            {
                buf[i] = dmtxChannel.EncodedWords[i];
            }
            return num;
        }

        // Token: 0x060000DC RID: 220 RVA: 0x000067EC File Offset: 0x000049EC
        private DmtxChannel FindBestChannel(DmtxChannelGroup group, DmtxScheme targetScheme)
        {
            DmtxChannel dmtxChannel = null;
            for (DmtxScheme dmtxScheme = DmtxScheme.DmtxSchemeAscii; dmtxScheme <= DmtxScheme.DmtxSchemeBase256; dmtxScheme++)
            {
                DmtxChannel dmtxChannel2 = group.Channels[(int)dmtxScheme];
                if (dmtxChannel2.Invalid == DmtxChannelStatus.DmtxChannelValid)
                {
                    if (dmtxChannel2.InputIndex != dmtxChannel2.Input.Length)
                    {
                        if (!this.EncodeNextWord(dmtxChannel2, targetScheme))
                        {
                        }
                        if ((dmtxChannel2.Invalid & DmtxChannelStatus.DmtxChannelUnsupportedChar) != DmtxChannelStatus.DmtxChannelValid)
                        {
                            dmtxChannel = dmtxChannel2;
                            break;
                        }
                        if ((dmtxChannel2.Invalid & DmtxChannelStatus.DmtxChannelCannotUnlatch) == DmtxChannelStatus.DmtxChannelValid)
                        {
                            if (dmtxChannel == null || dmtxChannel2.CurrentLength < dmtxChannel.CurrentLength)
                            {
                                dmtxChannel = dmtxChannel2;
                            }
                        }
                    }
                }
            }
            return dmtxChannel;
        }

        // Token: 0x060000DD RID: 221 RVA: 0x000068B4 File Offset: 0x00004AB4
        private bool EncodeNextWord(DmtxChannel channel, DmtxScheme targetScheme)
        {
            if (channel.EncScheme != targetScheme)
            {
                this.ChangeEncScheme(channel, targetScheme, DmtxUnlatch.Explicit);
                if (channel.Invalid != DmtxChannelStatus.DmtxChannelValid)
                {
                    return false;
                }
            }
            if (channel.EncScheme != targetScheme)
            {
                throw new Exception("For encoding, channel scheme must equal target scheme!");
            }
            bool result;
            switch (channel.EncScheme)
            {
                case DmtxScheme.DmtxSchemeAscii:
                    result = this.EncodeAsciiCodeword(channel);
                    break;
                case DmtxScheme.DmtxSchemeC40:
                    result = this.EncodeTripletCodeword(channel);
                    break;
                case DmtxScheme.DmtxSchemeText:
                    result = this.EncodeTripletCodeword(channel);
                    break;
                case DmtxScheme.DmtxSchemeX12:
                    result = this.EncodeTripletCodeword(channel);
                    break;
                case DmtxScheme.DmtxSchemeEdifact:
                    result = this.EncodeEdifactCodeword(channel);
                    break;
                case DmtxScheme.DmtxSchemeBase256:
                    result = this.EncodeBase256Codeword(channel);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        // Token: 0x060000DE RID: 222 RVA: 0x00006970 File Offset: 0x00004B70
        private bool EncodeBase256Codeword(DmtxChannel channel)
        {
            byte[] array = new byte[2];
            if (channel.EncScheme != DmtxScheme.DmtxSchemeBase256)
            {
                throw new Exception("Invalid encoding scheme selected!");
            }
            int num = channel.FirstCodeWord / 12;
            array[0] = DmtxMessage.UnRandomize255State(channel.EncodedWords[num], channel.FirstCodeWord / 12 + 1);
            int num2;
            if (array[0] <= 249)
            {
                num2 = (int)array[0];
            }
            else
            {
                num2 = (int)(250 * (array[0] - 249));
                num2 += (int)DmtxMessage.UnRandomize255State(channel.EncodedWords[num + 1], channel.FirstCodeWord / 12 + 2);
            }
            num2++;
            int num3;
            if (num2 <= 249)
            {
                num3 = 1;
                array[0] = (byte)num2;
                array[1] = 0;
            }
            else
            {
                num3 = 2;
                array[0] = (byte)(num2 / 250 + 249);
                array[1] = (byte)(num2 % 250);
            }
            if (num2 <= 0 || num2 > 1555)
            {
                throw new Exception("Encoding failed, data length out of range!");
            }
            if (num2 == 250)
            {
                for (int i = channel.CurrentLength / 12 - 1; i > channel.FirstCodeWord / 12; i--)
                {
                    byte codewordValue = DmtxMessage.UnRandomize255State(channel.EncodedWords[i], i + 1);
                    channel.EncodedWords[i + 1] = this.Randomize255State(codewordValue, i + 2);
                }
                this.IncrementProgress(channel, 12);
                channel.EncodedLength += 12;
            }
            for (int i = 0; i < num3; i++)
            {
                channel.EncodedWords[num + i] = this.Randomize255State(array[i], channel.FirstCodeWord / 12 + i + 1);
            }
            this.PushInputWord(channel, this.Randomize255State(channel.Input[channel.InputIndex], channel.CurrentLength / 12 + 1));
            this.IncrementProgress(channel, 12);
            channel.InputIndex++;
            return true;
        }

        // Token: 0x060000DF RID: 223 RVA: 0x00006B6C File Offset: 0x00004D6C
        private bool EncodeEdifactCodeword(DmtxChannel channel)
        {
            if (channel.EncScheme != DmtxScheme.DmtxSchemeEdifact)
            {
                throw new Exception("Invalid encoding scheme selected!");
            }
            byte b = channel.Input[channel.InputIndex];
            bool result;
            if (b < 32 || b > 94)
            {
                channel.Invalid = DmtxChannelStatus.DmtxChannelUnsupportedChar;
                result = false;
            }
            else
            {
                this.PushInputWord(channel, (byte)(b & 63));
                this.IncrementProgress(channel, 9);
                channel.InputIndex++;
                this.CheckForEndOfSymbolEdifact(channel);
                result = true;
            }
            return result;
        }

        // Token: 0x060000E0 RID: 224 RVA: 0x00006BF8 File Offset: 0x00004DF8
        private void CheckForEndOfSymbolEdifact(DmtxChannel channel)
        {
            if (channel.InputIndex > channel.Input.Length)
            {
                throw new Exception("Input index out of range while encoding!");
            }
            int num = channel.Input.Length - channel.InputIndex;
            if (num <= 4)
            {
                int num2 = channel.CurrentLength / 12;
                DmtxSymbolSize sizeIdx = this.FindCorrectSymbolSize(num2, DmtxSymbolSize.DmtxSymbolSquareAuto);
                int num3 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx) - num2;
                if (channel.CurrentLength % 12 == 0 && (num3 == 1 || num3 == 2))
                {
                    int num4 = num;
                    if (num4 <= num3)
                    {
                        this.ChangeEncScheme(channel, DmtxScheme.DmtxSchemeAscii, DmtxUnlatch.Implicit);
                        for (int i = 0; i < num; i++)
                        {
                            if (!this.EncodeNextWord(channel, DmtxScheme.DmtxSchemeAscii))
                            {
                                break;
                            }
                            if (channel.Invalid != DmtxChannelStatus.DmtxChannelValid)
                            {
                                throw new Exception("Error checking for end of symbol edifact");
                            }
                        }
                    }
                }
                else if (num == 0)
                {
                    this.ChangeEncScheme(channel, DmtxScheme.DmtxSchemeAscii, DmtxUnlatch.Explicit);
                }
            }
        }

        // Token: 0x060000E1 RID: 225 RVA: 0x00006D10 File Offset: 0x00004F10
        private void PushInputWord(DmtxChannel channel, byte codeword)
        {
            if (channel.EncodedLength / 12 > 4674)
            {
                throw new Exception("Can't push input word, encoded length exceeds limits!");
            }
            switch (channel.EncScheme)
            {
                case DmtxScheme.DmtxSchemeAscii:
                    channel.EncodedWords[channel.CurrentLength / 12] = codeword;
                    channel.EncodedLength += 12;
                    break;
                case DmtxScheme.DmtxSchemeC40:
                    channel.EncodedWords[channel.EncodedLength / 12] = codeword;
                    channel.EncodedLength += 12;
                    break;
                case DmtxScheme.DmtxSchemeText:
                    channel.EncodedWords[channel.EncodedLength / 12] = codeword;
                    channel.EncodedLength += 12;
                    break;
                case DmtxScheme.DmtxSchemeX12:
                    channel.EncodedWords[channel.EncodedLength / 12] = codeword;
                    channel.EncodedLength += 12;
                    break;
                case DmtxScheme.DmtxSchemeEdifact:
                    {
                        int num = channel.CurrentLength % 4;
                        int num2 = (channel.CurrentLength + 9) / 12 - num;
                        DmtxQuadruplet quadrupletValues = this.GetQuadrupletValues(channel.EncodedWords[num2], channel.EncodedWords[num2 + 1], channel.EncodedWords[num2 + 2]);
                        quadrupletValues.Value[num] = codeword;
                        for (int i = num + 1; i < 4; i++)
                        {
                            quadrupletValues.Value[i] = 0;
                        }
                        switch (num)
                        {
                            case 0:
                                channel.EncodedWords[num2] = (byte)((int)quadrupletValues.Value[0] << 2 | quadrupletValues.Value[1] >> 4);
                                break;
                            case 1:
                                channel.EncodedWords[num2 + 1] = (byte)((int)(quadrupletValues.Value[1] & 15) << 4 | quadrupletValues.Value[2] >> 2);
                                break;
                            case 2:
                                channel.EncodedWords[num2 + 2] = (byte)((int)(quadrupletValues.Value[2] & 3) << 6 | (int)quadrupletValues.Value[3]);
                                break;
                            case 3:
                                channel.EncodedWords[num2 + 2] = (byte)((int)(quadrupletValues.Value[2] & 3) << 6 | (int)quadrupletValues.Value[3]);
                                break;
                        }
                        channel.EncodedLength += 9;
                        break;
                    }
                case DmtxScheme.DmtxSchemeBase256:
                    channel.EncodedWords[channel.CurrentLength / 12] = codeword;
                    channel.EncodedLength += 12;
                    break;
            }
        }

        // Token: 0x060000E2 RID: 226 RVA: 0x00006F50 File Offset: 0x00005150
        private bool EncodeTripletCodeword(DmtxChannel channel)
        {
            int[] array = new int[4];
            byte[] array2 = new byte[6];
            DmtxTriplet triplet = default(DmtxTriplet);
            if (channel.EncScheme != DmtxScheme.DmtxSchemeX12 && channel.EncScheme != DmtxScheme.DmtxSchemeText && channel.EncScheme != DmtxScheme.DmtxSchemeC40)
            {
                throw new Exception("Invalid encoding scheme selected!");
            }
            if (channel.CurrentLength > channel.EncodedLength)
            {
                throw new Exception("Encoding length out of range!");
            }
            if (channel.CurrentLength == channel.EncodedLength)
            {
                if (channel.CurrentLength % 12 != 0)
                {
                    throw new Exception("Invalid encoding length!");
                }
                int inputIndex = channel.InputIndex;
                int num = 0;
                for (; ; )
                {
                    while (num < 3 && inputIndex < channel.Input.Length)
                    {
                        byte inputWord = channel.Input[inputIndex++];
                        int c40TextX12Words = this.GetC40TextX12Words(array, inputWord, channel.EncScheme);
                        if (c40TextX12Words == 0)
                        {
                            goto Block_7;
                        }
                        for (int i = 0; i < c40TextX12Words; i++)
                        {
                            array2[num++] = (byte)array[i];
                        }
                    }
                    triplet.Value[0] = array2[0];
                    triplet.Value[1] = array2[1];
                    triplet.Value[2] = array2[2];
                    if (num >= 3)
                    {
                        this.PushTriplet(channel, triplet);
                        array2[0] = array2[3];
                        array2[1] = array2[4];
                        array2[2] = array2[5];
                        num -= 3;
                    }
                    if (inputIndex == channel.Input.Length)
                    {
                        goto Block_11;
                    }
                    if (num == 0)
                    {
                        goto Block_16;
                    }
                }
            Block_7:
                channel.Invalid = DmtxChannelStatus.DmtxChannelUnsupportedChar;
                return false;
            Block_11:
                while (channel.CurrentLength < channel.EncodedLength)
                {
                    this.IncrementProgress(channel, 8);
                    channel.InputIndex++;
                }
                if (channel.CurrentLength == channel.EncodedLength + 8)
                {
                    channel.CurrentLength = channel.EncodedLength;
                    channel.InputIndex--;
                }
                if (channel.Input.Length < channel.InputIndex)
                {
                    throw new Exception("Channel input index exceeds range!");
                }
                int inputCount = channel.Input.Length - channel.InputIndex;
                if (!this.ProcessEndOfSymbolTriplet(channel, triplet, num, inputCount))
                {
                    return false;
                }
            Block_16:;
            }
            if (channel.CurrentLength < channel.EncodedLength)
            {
                this.IncrementProgress(channel, 8);
                channel.InputIndex++;
            }
            return true;
        }

        // Token: 0x060000E3 RID: 227 RVA: 0x00007208 File Offset: 0x00005408
        private int GetC40TextX12Words(int[] outputWords, byte inputWord, DmtxScheme encScheme)
        {
            if (encScheme != DmtxScheme.DmtxSchemeX12 && encScheme != DmtxScheme.DmtxSchemeText && encScheme != DmtxScheme.DmtxSchemeC40)
            {
                throw new Exception("Invalid encoding scheme selected!");
            }
            int result = 0;
            if (inputWord > 127)
            {
                if (encScheme == DmtxScheme.DmtxSchemeX12)
                {
                    return 0;
                }
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift2;
                outputWords[result++] = 30;
                inputWord -= 128;
            }
            if (encScheme == DmtxScheme.DmtxSchemeX12)
            {
                if (inputWord == 13)
                {
                    outputWords[result++] = 0;
                }
                else if (inputWord == 42)
                {
                    outputWords[result++] = 1;
                }
                else if (inputWord == 62)
                {
                    outputWords[result++] = 2;
                }
                else if (inputWord == 32)
                {
                    outputWords[result++] = 3;
                }
                else if (inputWord >= 48 && inputWord <= 57)
                {
                    outputWords[result++] = (int)(inputWord - 44);
                }
                else if (inputWord >= 65 && inputWord <= 90)
                {
                    outputWords[result++] = (int)(inputWord - 51);
                }
            }
            else if (inputWord <= 31)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift1;
                outputWords[result++] = (int)inputWord;
            }
            else if (inputWord == 32)
            {
                outputWords[result++] = 3;
            }
            else if (inputWord <= 47)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift2;
                outputWords[result++] = (int)(inputWord - 33);
            }
            else if (inputWord <= 57)
            {
                outputWords[result++] = (int)(inputWord - 44);
            }
            else if (inputWord <= 64)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift2;
                outputWords[result++] = (int)(inputWord - 43);
            }
            else if (inputWord <= 90 && encScheme == DmtxScheme.DmtxSchemeC40)
            {
                outputWords[result++] = (int)(inputWord - 51);
            }
            else if (inputWord <= 90 && encScheme == DmtxScheme.DmtxSchemeText)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift3;
                outputWords[result++] = (int)(inputWord - 64);
            }
            else if (inputWord <= 95)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift2;
                outputWords[result++] = (int)(inputWord - 69);
            }
            else if (inputWord == 96 && encScheme == DmtxScheme.DmtxSchemeText)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift3;
                outputWords[result++] = 0;
            }
            else if (inputWord <= 122 && encScheme == DmtxScheme.DmtxSchemeText)
            {
                outputWords[result++] = (int)(inputWord - 83);
            }
            else if (inputWord <= 127)
            {
                outputWords[result++] = (int)DmtxConstants.DmtxCharTripletShift3;
                outputWords[result++] = (int)(inputWord - 96);
            }
            return result;
        }

        // Token: 0x060000E4 RID: 228 RVA: 0x000074B8 File Offset: 0x000056B8
        private bool ProcessEndOfSymbolTriplet(DmtxChannel channel, DmtxTriplet triplet, int tripletCount, int inputCount)
        {
            if (channel.CurrentLength % 12 != 0)
            {
                throw new Exception("Invalid current length for encoding!");
            }
            int num = tripletCount - inputCount;
            int num2 = channel.CurrentLength / 12;
            DmtxSymbolSize dmtxSymbolSize = this.FindCorrectSymbolSize(num2 + ((inputCount == 3) ? 2 : inputCount), this._sizeIdxRequest);
            bool result;
            if (dmtxSymbolSize == DmtxSymbolSize.DmtxSymbolShapeAuto)
            {
                result = false;
            }
            else
            {
                int num3 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize) - num2;
                if (inputCount == 1 && num3 == 1)
                {
                    this.ChangeEncScheme(channel, DmtxScheme.DmtxSchemeAscii, DmtxUnlatch.Implicit);
                    if (!this.EncodeNextWord(channel, DmtxScheme.DmtxSchemeAscii))
                    {
                        return false;
                    }
                    if (channel.Invalid != DmtxChannelStatus.DmtxChannelValid || channel.InputIndex != channel.Input.Length)
                    {
                        throw new Exception("Error processing end of symbol triplet!");
                    }
                }
                else if (num3 == 2)
                {
                    if (tripletCount == 3)
                    {
                        this.PushTriplet(channel, triplet);
                        this.IncrementProgress(channel, 24);
                        channel.EncScheme = DmtxScheme.DmtxSchemeAscii;
                        channel.InputIndex += 3;
                        channel.InputIndex -= num;
                    }
                    else if (tripletCount == 2)
                    {
                        triplet.Value[2] = 0;
                        this.PushTriplet(channel, triplet);
                        this.IncrementProgress(channel, 24);
                        channel.EncScheme = DmtxScheme.DmtxSchemeAscii;
                        channel.InputIndex += 2;
                        channel.InputIndex -= num;
                    }
                    else if (tripletCount == 1)
                    {
                        this.ChangeEncScheme(channel, DmtxScheme.DmtxSchemeAscii, DmtxUnlatch.Explicit);
                        if (!this.EncodeNextWord(channel, DmtxScheme.DmtxSchemeAscii))
                        {
                            return false;
                        }
                        if (channel.Invalid != DmtxChannelStatus.DmtxChannelValid)
                        {
                            throw new Exception("Error processing end of symbol triplet!");
                        }
                    }
                }
                else
                {
                    num2 = channel.CurrentLength / 12;
                    num3 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize) - num2;
                    if (num3 > 0)
                    {
                        this.ChangeEncScheme(channel, DmtxScheme.DmtxSchemeAscii, DmtxUnlatch.Explicit);
                        while (channel.InputIndex < channel.Input.Length)
                        {
                            if (!this.EncodeNextWord(channel, DmtxScheme.DmtxSchemeAscii))
                            {
                                return false;
                            }
                            if (channel.Invalid != DmtxChannelStatus.DmtxChannelValid)
                            {
                                throw new Exception("Error processing end of symbol triplet!");
                            }
                        }
                    }
                }
                if (channel.InputIndex != channel.Input.Length)
                {
                    throw new Exception("Could not fully process end of symbol triplet!");
                }
                result = true;
            }
            return result;
        }

        // Token: 0x060000E5 RID: 229 RVA: 0x00007740 File Offset: 0x00005940
        private void PushTriplet(DmtxChannel channel, DmtxTriplet triplet)
        {
            int num = 1600 * (int)triplet.Value[0] + (int)(40 * triplet.Value[1]) + (int)triplet.Value[2] + 1;
            this.PushInputWord(channel, (byte)(num / 256));
            this.PushInputWord(channel, (byte)(num % 256));
        }

        // Token: 0x060000E6 RID: 230 RVA: 0x00007798 File Offset: 0x00005998
        private bool EncodeAsciiCodeword(DmtxChannel channel)
        {
            if (channel.EncScheme != DmtxScheme.DmtxSchemeAscii)
            {
                throw new Exception("Invalid encoding scheme selected!");
            }
            byte b = channel.Input[channel.InputIndex];
            if (this.IsDigit(b) && channel.CurrentLength >= channel.FirstCodeWord + 12)
            {
                int num = (channel.CurrentLength - 12) / 12;
                byte b2 = (byte)(channel.EncodedWords[num] - 1);
                byte b3 = 0;
                if (num > channel.FirstCodeWord / 12)
                    b3 = channel.EncodedWords[num - 1];

                if (b3 != 235 && this.IsDigit(b2))
                {
                    channel.EncodedWords[num] = (byte)(10 * (b2 - 48) + (b - 48) + 130);
                    channel.InputIndex++;
                    return true;
                }
            }
            bool result;
            if (b == DmtxConstants.DmtxCharFNC1)
            {
                this.PushInputWord(channel, DmtxConstants.DmtxCharFNC1);
                this.IncrementProgress(channel, 12);
                channel.InputIndex++;
                result = true;
            }
            else
            {
                if (b >= 128)
                {
                    this.PushInputWord(channel, DmtxConstants.DmtxCharAsciiUpperShift);
                    this.IncrementProgress(channel, 12);
                    b -= 128;
                }
                this.PushInputWord(channel, (byte)(b + 1));
                this.IncrementProgress(channel, 12);
                channel.InputIndex++;
                result = true;
            }
            return result;
        }

        // Token: 0x060000E7 RID: 231 RVA: 0x00007914 File Offset: 0x00005B14
        private bool IsDigit(byte inputValue)
        {
            return inputValue >= 48 && inputValue <= 57;
        }

        // Token: 0x060000E8 RID: 232 RVA: 0x00007938 File Offset: 0x00005B38
        private void ChangeEncScheme(DmtxChannel channel, DmtxScheme targetScheme, DmtxUnlatch unlatchType)
        {
            if (channel.EncScheme == targetScheme)
            {
                throw new Exception("Target scheme already equals channel scheme, cannot be changed!");
            }
            switch (channel.EncScheme)
            {
                case DmtxScheme.DmtxSchemeAscii:
                    if (channel.CurrentLength % 12 != 0)
                    {
                        throw new Exception("Invalid current length detected encoding ascii code");
                    }
                    break;
                case DmtxScheme.DmtxSchemeC40:
                case DmtxScheme.DmtxSchemeText:
                case DmtxScheme.DmtxSchemeX12:
                    if (channel.CurrentLength % 12 != 0)
                    {
                        channel.Invalid = DmtxChannelStatus.DmtxChannelCannotUnlatch;
                        return;
                    }
                    if (channel.CurrentLength != channel.EncodedLength)
                    {
                        channel.Invalid = DmtxChannelStatus.DmtxChannelCannotUnlatch;
                        return;
                    }
                    if (unlatchType == DmtxUnlatch.Explicit)
                    {
                        this.PushInputWord(channel, (byte)DmtxConstants.DmtxCharTripletUnlatch);
                        this.IncrementProgress(channel, 12);
                    }
                    break;
                case DmtxScheme.DmtxSchemeEdifact:
                    {
                        if (channel.CurrentLength % 3 != 0)
                        {
                            throw new Exception("Error changing encryption scheme, current length is invalid!");
                        }
                        if (unlatchType == DmtxUnlatch.Explicit)
                        {
                            this.PushInputWord(channel, (byte)DmtxConstants.DmtxCharEdifactUnlatch);
                            this.IncrementProgress(channel, 9);
                        }
                        int num = channel.CurrentLength % 4 * 3;
                        channel.CurrentLength += num;
                        channel.EncodedLength += num;
                        break;
                    }
            }
            channel.EncScheme = DmtxScheme.DmtxSchemeAscii;
            switch (targetScheme)
            {
                case DmtxScheme.DmtxSchemeC40:
                    this.PushInputWord(channel, DmtxConstants.DmtxCharC40Latch);
                    this.IncrementProgress(channel, 12);
                    break;
                case DmtxScheme.DmtxSchemeText:
                    this.PushInputWord(channel, DmtxConstants.DmtxCharTextLatch);
                    this.IncrementProgress(channel, 12);
                    break;
                case DmtxScheme.DmtxSchemeX12:
                    this.PushInputWord(channel, DmtxConstants.DmtxCharX12Latch);
                    this.IncrementProgress(channel, 12);
                    break;
                case DmtxScheme.DmtxSchemeEdifact:
                    this.PushInputWord(channel, DmtxConstants.DmtxCharEdifactLatch);
                    this.IncrementProgress(channel, 12);
                    break;
                case DmtxScheme.DmtxSchemeBase256:
                    this.PushInputWord(channel, DmtxConstants.DmtxCharBase256Latch);
                    this.IncrementProgress(channel, 12);
                    this.PushInputWord(channel, this.Randomize255State(0, 2));
                    this.IncrementProgress(channel, 12);
                    break;
            }
            channel.EncScheme = targetScheme;
            channel.FirstCodeWord = channel.CurrentLength - 12;
            if (channel.FirstCodeWord % 12 != 0)
            {
                throw new Exception("Error while changin encoding scheme, invalid first code word!");
            }
        }

        // Token: 0x060000E9 RID: 233 RVA: 0x00007B7C File Offset: 0x00005D7C
        private byte Randomize255State(byte codewordValue, int codewordPosition)
        {
            int num = 149 * codewordPosition % 255 + 1;
            int num2 = (int)codewordValue + num;
            return (byte)((num2 <= 255) ? num2 : (num2 - 256));
        }

        // Token: 0x060000EA RID: 234 RVA: 0x00007BB8 File Offset: 0x00005DB8
        private void IncrementProgress(DmtxChannel channel, int encodedUnits)
        {
            if (channel.EncScheme == DmtxScheme.DmtxSchemeC40 || channel.EncScheme == DmtxScheme.DmtxSchemeText)
            {
                int num = channel.CurrentLength % 6 / 2;
                int num2 = channel.CurrentLength / 12 - (num >> 1);
                if (DmtxEncode.GetTripletValues(channel.EncodedWords[num2], channel.EncodedWords[num2 + 1]).Value[num] <= 2)
                {
                    channel.CurrentLength += 8;
                }
            }
            channel.CurrentLength += encodedUnits;
        }

        // Token: 0x060000EB RID: 235 RVA: 0x00007C44 File Offset: 0x00005E44
        private static DmtxTriplet GetTripletValues(byte cw1, byte cw2)
        {
            DmtxTriplet result = default(DmtxTriplet);
            int num = (int)cw1 << 8 | (int)cw2;
            result.Value[0] = (byte)((num - 1) / 1600);
            result.Value[1] = (byte)((num - 1) / 40 % 40);
            result.Value[2] = (byte)((num - 1) % 40);
            return result;
        }

        // Token: 0x060000EC RID: 236 RVA: 0x00007C9C File Offset: 0x00005E9C
        private DmtxQuadruplet GetQuadrupletValues(byte cw1, byte cw2, byte cw3)
        {
            DmtxQuadruplet result = default(DmtxQuadruplet);
            result.Value[0] = (byte)(cw1 >> 2);
            result.Value[1] = (byte)((int)(cw1 & 3) << 4 | (cw2 & 240) >> 4);
            result.Value[2] = (byte)((int)(cw2 & 15) << 2 | (cw3 & 192) >> 6);
            result.Value[3] = (byte)(cw3 & 63);
            return result;
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00007D05 File Offset: 0x00005F05
        private static void InitChannel(DmtxChannel channel, byte[] codewords)
        {
            channel.EncScheme = DmtxScheme.DmtxSchemeAscii;
            channel.Invalid = DmtxChannelStatus.DmtxChannelValid;
            channel.InputIndex = 0;
            channel.Input = codewords;
        }

        // Token: 0x17000057 RID: 87
        // (get) Token: 0x060000EE RID: 238 RVA: 0x00007D28 File Offset: 0x00005F28
        // (set) Token: 0x060000EF RID: 239 RVA: 0x00007D40 File Offset: 0x00005F40
        internal int Method
        {
            get
            {
                return this._method;
            }
            set
            {
                this._method = value;
            }
        }

        // Token: 0x17000058 RID: 88
        // (get) Token: 0x060000F0 RID: 240 RVA: 0x00007D4C File Offset: 0x00005F4C
        // (set) Token: 0x060000F1 RID: 241 RVA: 0x00007D64 File Offset: 0x00005F64
        internal DmtxScheme Scheme
        {
            get
            {
                return this._scheme;
            }
            set
            {
                this._scheme = value;
            }
        }

        // Token: 0x17000059 RID: 89
        // (get) Token: 0x060000F2 RID: 242 RVA: 0x00007D70 File Offset: 0x00005F70
        // (set) Token: 0x060000F3 RID: 243 RVA: 0x00007D88 File Offset: 0x00005F88
        internal DmtxSymbolSize SizeIdxRequest
        {
            get
            {
                return this._sizeIdxRequest;
            }
            set
            {
                this._sizeIdxRequest = value;
            }
        }

        // Token: 0x1700005A RID: 90
        // (get) Token: 0x060000F4 RID: 244 RVA: 0x00007D94 File Offset: 0x00005F94
        // (set) Token: 0x060000F5 RID: 245 RVA: 0x00007DAC File Offset: 0x00005FAC
        internal int MarginSize
        {
            get
            {
                return this._marginSize;
            }
            set
            {
                this._marginSize = value;
            }
        }

        // Token: 0x1700005B RID: 91
        // (get) Token: 0x060000F6 RID: 246 RVA: 0x00007DB8 File Offset: 0x00005FB8
        // (set) Token: 0x060000F7 RID: 247 RVA: 0x00007DD0 File Offset: 0x00005FD0
        internal int ModuleSize
        {
            get
            {
                return this._moduleSize;
            }
            set
            {
                this._moduleSize = value;
            }
        }

        // Token: 0x1700005C RID: 92
        // (get) Token: 0x060000F8 RID: 248 RVA: 0x00007DDC File Offset: 0x00005FDC
        // (set) Token: 0x060000F9 RID: 249 RVA: 0x00007DF4 File Offset: 0x00005FF4
        internal DmtxPackOrder PixelPacking
        {
            get
            {
                return this._pixelPacking;
            }
            set
            {
                this._pixelPacking = value;
            }
        }

        // Token: 0x1700005D RID: 93
        // (get) Token: 0x060000FA RID: 250 RVA: 0x00007E00 File Offset: 0x00006000
        // (set) Token: 0x060000FB RID: 251 RVA: 0x00007E18 File Offset: 0x00006018
        internal DmtxFlip ImageFlip
        {
            get
            {
                return this._imageFlip;
            }
            set
            {
                this._imageFlip = value;
            }
        }

        // Token: 0x1700005E RID: 94
        // (get) Token: 0x060000FC RID: 252 RVA: 0x00007E24 File Offset: 0x00006024
        // (set) Token: 0x060000FD RID: 253 RVA: 0x00007E3C File Offset: 0x0000603C
        internal int RowPadBytes
        {
            get
            {
                return this._rowPadBytes;
            }
            set
            {
                this._rowPadBytes = value;
            }
        }

        // Token: 0x1700005F RID: 95
        // (get) Token: 0x060000FE RID: 254 RVA: 0x00007E48 File Offset: 0x00006048
        // (set) Token: 0x060000FF RID: 255 RVA: 0x00007E60 File Offset: 0x00006060
        internal DmtxMessage Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        // Token: 0x17000060 RID: 96
        // (get) Token: 0x06000100 RID: 256 RVA: 0x00007E6C File Offset: 0x0000606C
        // (set) Token: 0x06000101 RID: 257 RVA: 0x00007E84 File Offset: 0x00006084
        internal DmtxImage Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
            }
        }

        // Token: 0x17000061 RID: 97
        // (get) Token: 0x06000102 RID: 258 RVA: 0x00007E90 File Offset: 0x00006090
        // (set) Token: 0x06000103 RID: 259 RVA: 0x00007EA8 File Offset: 0x000060A8
        internal DmtxRegion Region
        {
            get
            {
                return this._region;
            }
            set
            {
                this._region = value;
            }
        }

        // Token: 0x17000062 RID: 98
        // (get) Token: 0x06000104 RID: 260 RVA: 0x00007EB4 File Offset: 0x000060B4
        // (set) Token: 0x06000105 RID: 261 RVA: 0x00007ECC File Offset: 0x000060CC
        public bool[,] RawData
        {
            get
            {
                return this._rawData;
            }
            set
            {
                this._rawData = value;
            }
        }

        // Token: 0x04000103 RID: 259
        private int _method;

        // Token: 0x04000104 RID: 260
        private DmtxScheme _scheme;

        // Token: 0x04000105 RID: 261
        private DmtxSymbolSize _sizeIdxRequest;

        // Token: 0x04000106 RID: 262
        private int _marginSize;

        // Token: 0x04000107 RID: 263
        private int _moduleSize;

        // Token: 0x04000108 RID: 264
        private DmtxPackOrder _pixelPacking;

        // Token: 0x04000109 RID: 265
        private DmtxFlip _imageFlip;

        // Token: 0x0400010A RID: 266
        private int _rowPadBytes;

        // Token: 0x0400010B RID: 267
        private DmtxMessage _message;

        // Token: 0x0400010C RID: 268
        private DmtxImage _image;

        // Token: 0x0400010D RID: 269
        private DmtxRegion _region;

        // Token: 0x0400010E RID: 270
        private bool[,] _rawData;
    }
}

using System;

namespace DataMatrix.net
{
	// Token: 0x02000023 RID: 35
	internal class DmtxMessage
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x0000E128 File Offset: 0x0000C328
		internal DmtxMessage(DmtxSymbolSize sizeIdx, DmtxFormat symbolFormat)
		{
			if (symbolFormat != DmtxFormat.Matrix && symbolFormat != DmtxFormat.Mosaic)
			{
				throw new ArgumentException("Only DmtxFormats Matrix and Mosaic are currently supported");
			}
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, sizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);
			this.Array = new byte[symbolAttribute2 * symbolAttribute];
			int num = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx) + DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolErrorWords, sizeIdx);
			this.Code = new byte[num];
			this.Output = new byte[10 * num];
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000E1A8 File Offset: 0x0000C3A8
		internal void DecodeDataStream(DmtxSymbolSize sizeIdx, byte[] outputStart)
		{
			bool flag = false;
			this.Output = (outputStart ?? this.Output);
			this._outputIdx = 0;
			byte[] code = this.Code;
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);
			if (code[0] == DmtxConstants.DmtxChar05Macro || code[0] == DmtxConstants.DmtxChar06Macro)
			{
				this.PushOutputMacroHeader(code[0]);
				flag = true;
			}
			int i = 0;
			while (i < symbolAttribute)
			{
				DmtxScheme encodationScheme = DmtxMessage.GetEncodationScheme(this.Code[i]);
				if (encodationScheme != DmtxScheme.DmtxSchemeAscii)
				{
					i++;
				}
				switch (encodationScheme)
				{
				case DmtxScheme.DmtxSchemeAscii:
					i = this.DecodeSchemeAscii(i, symbolAttribute);
					break;
				case DmtxScheme.DmtxSchemeC40:
				case DmtxScheme.DmtxSchemeText:
					i = this.DecodeSchemeC40Text(i, symbolAttribute, encodationScheme);
					break;
				case DmtxScheme.DmtxSchemeX12:
					i = this.DecodeSchemeX12(i, symbolAttribute);
					break;
				case DmtxScheme.DmtxSchemeEdifact:
					i = this.DecodeSchemeEdifact(i, symbolAttribute);
					break;
				case DmtxScheme.DmtxSchemeBase256:
					i = this.DecodeSchemeBase256(i, symbolAttribute);
					break;
				}
			}
			if (flag)
			{
				this.PushOutputMacroTrailer();
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000E2B4 File Offset: 0x0000C4B4
		private void PushOutputMacroHeader(byte macroType)
		{
			this.PushOutputWord(91);
			this.PushOutputWord(41);
			this.PushOutputWord(62);
			this.PushOutputWord(30);
			this.PushOutputWord(48);
			if (macroType == DmtxConstants.DmtxChar05Macro)
			{
				this.PushOutputWord(53);
			}
			else
			{
				if (macroType != DmtxConstants.DmtxChar06Macro)
				{
					throw new ArgumentException("Macro Header only supported for char05 and char06");
				}
				this.PushOutputWord(54);
			}
			this.PushOutputWord(29);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000E33C File Offset: 0x0000C53C
		private void PushOutputMacroTrailer()
		{
			this.PushOutputWord(30);
			this.PushOutputWord(4);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000E350 File Offset: 0x0000C550
		private void PushOutputWord(byte value)
		{
			this.Output[this._outputIdx++] = value;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000E378 File Offset: 0x0000C578
		private static DmtxScheme GetEncodationScheme(byte val)
		{
			DmtxScheme result;
			if (val == DmtxConstants.DmtxCharC40Latch)
			{
				result = DmtxScheme.DmtxSchemeC40;
			}
			else if (val == DmtxConstants.DmtxCharBase256Latch)
			{
				result = DmtxScheme.DmtxSchemeBase256;
			}
			else if (val == DmtxConstants.DmtxCharEdifactLatch)
			{
				result = DmtxScheme.DmtxSchemeEdifact;
			}
			else if (val == DmtxConstants.DmtxCharTextLatch)
			{
				result = DmtxScheme.DmtxSchemeText;
			}
			else if (val == DmtxConstants.DmtxCharX12Latch)
			{
				result = DmtxScheme.DmtxSchemeX12;
			}
			else
			{
				result = DmtxScheme.DmtxSchemeAscii;
			}
			return result;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000E3F0 File Offset: 0x0000C5F0
		private int DecodeSchemeAscii(int startIndex, int endIndex)
		{
			bool flag = false;
			while (startIndex < endIndex)
			{
				byte b = this.Code[startIndex];
				if (DmtxMessage.GetEncodationScheme(this.Code[startIndex]) == DmtxScheme.DmtxSchemeAscii)
				{
					startIndex++;
					if (flag)
					{
						this.PushOutputWord((byte)(b + 127));
						flag = false;
					}
					else if (b == DmtxConstants.DmtxCharAsciiUpperShift)
					{
						flag = true;
					}
					else
					{
						if (b == DmtxConstants.DmtxCharAsciiPad)
						{
							this.PadCount = endIndex - startIndex;
							return endIndex;
						}
						if (b <= 128)
						{
							this.PushOutputWord((byte)(b - 1));
						}
						else if (b <= 229)
						{
							byte b2 = (byte)(b - 130);
							this.PushOutputWord((byte)(b2 / 10 + 48));
							this.PushOutputWord((byte)(b2 - b2 / 10 * 10 + 48));
						}
					}
					continue;
				}
				return startIndex;
			}
			return startIndex;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000E4F4 File Offset: 0x0000C6F4
		private int DecodeSchemeC40Text(int startIndex, int endIndex, DmtxScheme encScheme)
		{
			int[] array = new int[3];
			C40TextState c40TextState = new C40TextState
			{
				Shift = DmtxConstants.DmtxC40TextBasicSet,
				UpperShift = false
			};
			if (encScheme != DmtxScheme.DmtxSchemeC40 && encScheme != DmtxScheme.DmtxSchemeText)
			{
				throw new ArgumentException("Invalid scheme selected for decodind!");
			}
			while (startIndex < endIndex)
			{
				int num = (int)this.Code[startIndex] << 8 | (int)this.Code[startIndex + 1];
				array[0] = (num - 1) / 1600;
				array[1] = (num - 1) / 40 % 40;
				array[2] = (num - 1) % 40;
				startIndex += 2;
				for (int i = 0; i < 3; i++)
				{
					if (c40TextState.Shift == DmtxConstants.DmtxC40TextBasicSet)
					{
						if (array[i] <= 2)
						{
							c40TextState.Shift = array[i] + 1;
						}
						else if (array[i] == 3)
						{
							this.PushOutputC40TextWord(ref c40TextState, 32);
						}
						else if (array[i] <= 13)
						{
							this.PushOutputC40TextWord(ref c40TextState, array[i] - 13 + 57);
						}
						else if (array[i] <= 39)
						{
							if (encScheme == DmtxScheme.DmtxSchemeC40)
							{
								this.PushOutputC40TextWord(ref c40TextState, array[i] - 39 + 90);
							}
							else if (encScheme == DmtxScheme.DmtxSchemeText)
							{
								this.PushOutputC40TextWord(ref c40TextState, array[i] - 39 + 122);
							}
						}
					}
					else if (c40TextState.Shift == DmtxConstants.DmtxC40TextShift1)
					{
						this.PushOutputC40TextWord(ref c40TextState, array[i]);
					}
					else if (c40TextState.Shift == DmtxConstants.DmtxC40TextShift2)
					{
						if (array[i] <= 14)
						{
							this.PushOutputC40TextWord(ref c40TextState, array[i] + 33);
						}
						else if (array[i] <= 21)
						{
							this.PushOutputC40TextWord(ref c40TextState, array[i] + 43);
						}
						else if (array[i] <= 26)
						{
							this.PushOutputC40TextWord(ref c40TextState, array[i] + 69);
						}
						else if (array[i] == 27)
						{
							this.PushOutputC40TextWord(ref c40TextState, 29);
						}
						else if (array[i] == 30)
						{
							c40TextState.UpperShift = true;
							c40TextState.Shift = DmtxConstants.DmtxC40TextBasicSet;
						}
					}
					else if (c40TextState.Shift == DmtxConstants.DmtxC40TextShift3)
					{
						if (encScheme == DmtxScheme.DmtxSchemeC40)
						{
							this.PushOutputC40TextWord(ref c40TextState, array[i] + 96);
						}
						else if (encScheme == DmtxScheme.DmtxSchemeText)
						{
							if (array[i] == 0)
							{
								this.PushOutputC40TextWord(ref c40TextState, array[i] + 96);
							}
							else if (array[i] <= 26)
							{
								this.PushOutputC40TextWord(ref c40TextState, array[i] - 26 + 90);
							}
							else
							{
								this.PushOutputC40TextWord(ref c40TextState, array[i] - 31 + 127);
							}
						}
					}
				}
				int result;
				if ((int)this.Code[startIndex] == DmtxConstants.DmtxCharTripletUnlatch)
				{
					result = startIndex + 1;
				}
				else
				{
					if (endIndex - startIndex != 1)
					{
						continue;
					}
					result = startIndex;
				}
				return result;
			}
			return startIndex;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000E854 File Offset: 0x0000CA54
		private void PushOutputC40TextWord(ref C40TextState state, int value)
		{
			if (value < 0 || value >= 256)
			{
				throw new ArgumentException("Invalid value: Exceeds range for conversion to byte");
			}
			this.Output[this._outputIdx] = (byte)value;
			if (state.UpperShift)
			{
				if (value < 0 || value >= 256)
				{
					throw new ArgumentException("Invalid value: Exceeds range for conversion to upper case character");
				}
				byte[] output = this.Output;
				int outputIdx = this._outputIdx;
				output[outputIdx] += 128;
			}
			this._outputIdx++;
			state.Shift = DmtxConstants.DmtxC40TextBasicSet;
			state.UpperShift = false;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000E904 File Offset: 0x0000CB04
		private int DecodeSchemeX12(int startIndex, int endIndex)
		{
			int[] array = new int[3];
			while (startIndex < endIndex)
			{
				int num = (int)this.Code[startIndex] << 8 | (int)this.Code[startIndex + 1];
				array[0] = (num - 1) / 1600;
				array[1] = (num - 1) / 40 % 40;
				array[2] = (num - 1) % 40;
				startIndex += 2;
				for (int i = 0; i < 3; i++)
				{
					if (array[i] == 0)
					{
						this.PushOutputWord(13);
					}
					else if (array[i] == 1)
					{
						this.PushOutputWord(42);
					}
					else if (array[i] == 2)
					{
						this.PushOutputWord(62);
					}
					else if (array[i] == 3)
					{
						this.PushOutputWord(32);
					}
					else if (array[i] <= 13)
					{
						this.PushOutputWord((byte)(array[i] + 44));
					}
					else if (array[i] <= 90)
					{
						this.PushOutputWord((byte)(array[i] + 51));
					}
				}
				int result;
				if ((int)this.Code[startIndex] == DmtxConstants.DmtxCharTripletUnlatch)
				{
					result = startIndex + 1;
				}
				else
				{
					if (endIndex - startIndex != 1)
					{
						continue;
					}
					result = startIndex;
				}
				return result;
			}
			return startIndex;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000EA5C File Offset: 0x0000CC5C
		private int DecodeSchemeEdifact(int startIndex, int endIndex)
		{
			byte[] array = new byte[4];
			while (startIndex < endIndex)
			{
				array[0] = (byte)((this.Code[startIndex] & 252) >> 2);
				array[1] = (byte)((int)(this.Code[startIndex] & 3) << 4 | (this.Code[startIndex + 1] & 240) >> 4);
				array[2] = (byte)((int)(this.Code[startIndex + 1] & 15) << 2 | (this.Code[startIndex + 2] & 192) >> 6);
				array[3] = ((byte)(this.Code[startIndex + 2] & 63));
				int i = 0;
				while (i < 4)
				{
					if (i < 3)
					{
						startIndex++;
					}
					if ((int)array[i] == DmtxConstants.DmtxCharEdifactUnlatch)
					{
						if (this.Output[this._outputIdx] != 0)
						{
							throw new Exception("Error decoding edifact scheme");
						}
						return startIndex;
					}
					else
					{
						this.PushOutputWord((byte)((int)array[i] ^ (int)((array[i] & 32) ^ 32) << 1));
						i++;
					}
				}
				if (endIndex - startIndex >= 3)
				{
					continue;
				}
				return startIndex;
			}
			return startIndex;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000EB78 File Offset: 0x0000CD78
		private int DecodeSchemeBase256(int startIndex, int endIndex)
		{
			int num = startIndex + 1;
			int num2 = (int)DmtxMessage.UnRandomize255State(this.Code[startIndex++], num++);
			int num3;
			if (num2 == 0)
			{
				num3 = endIndex;
			}
			else if (num2 <= 249)
			{
				num3 = startIndex + num2;
			}
			else
			{
				int num4 = (int)DmtxMessage.UnRandomize255State(this.Code[startIndex++], num++);
				num3 = startIndex + (num2 - 249) * 250 + num4;
			}
			if (num3 > endIndex)
			{
				throw new Exception("Error decoding scheme base 256");
			}
			while (startIndex < num3)
			{
				this.PushOutputWord(DmtxMessage.UnRandomize255State(this.Code[startIndex++], num++));
			}
			return startIndex;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000EC44 File Offset: 0x0000CE44
		internal static byte UnRandomize255State(byte value, int idx)
		{
			int num = 149 * idx % 255 + 1;
			int num2 = (int)value - num;
			if (num2 < 0)
			{
				num2 += 256;
			}
			if (num2 < 0 || num2 >= 256)
			{
				throw new Exception("Error unrandomizing 255 state");
			}
			return (byte)num2;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000ECA0 File Offset: 0x0000CEA0
		internal int SymbolModuleStatus(DmtxSymbolSize sizeIdx, int symbolRow, int symbolCol)
		{
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionRows, sizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionCols, sizeIdx);
			int symbolAttribute3 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdx);
			int symbolAttribute4 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);
			int num = symbolAttribute3 - symbolRow - 1;
			int num2 = num - 1 - 2 * (num / (symbolAttribute + 2));
			int num3 = symbolCol - 1 - 2 * (symbolCol / (symbolAttribute2 + 2));
			int result;
			if (symbolRow % (symbolAttribute + 2) == 0 || symbolCol % (symbolAttribute2 + 2) == 0)
			{
				result = (DmtxConstants.DmtxModuleOnRGB | ((DmtxConstants.DmtxModuleData == 0) ? 1 : 0));
			}
			else if ((symbolRow + 1) % (symbolAttribute + 2) == 0)
			{
				result = ((((symbolCol & 1) != 0) ? 0 : DmtxConstants.DmtxModuleOnRGB) | ((DmtxConstants.DmtxModuleData == 0) ? 1 : 0));
			}
			else if ((symbolCol + 1) % (symbolAttribute2 + 2) == 0)
			{
				result = ((((symbolRow & 1) != 0) ? 0 : DmtxConstants.DmtxModuleOnRGB) | ((DmtxConstants.DmtxModuleData == 0) ? 1 : 0));
			}
			else
			{
				result = ((int)this.Array[num2 * symbolAttribute4 + num3] | DmtxConstants.DmtxModuleData);
			}
			return result;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x0000EDB7 File Offset: 0x0000CFB7
		internal int PadCount { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x0000EDC0 File Offset: 0x0000CFC0
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x0000EDD7 File Offset: 0x0000CFD7
		internal byte[] Array { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x0000EDE0 File Offset: 0x0000CFE0
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x0000EDF7 File Offset: 0x0000CFF7
		internal byte[] Code { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x0000EE00 File Offset: 0x0000D000
		// (set) Token: 0x060001C8 RID: 456 RVA: 0x0000EE17 File Offset: 0x0000D017
		internal byte[] Output { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x0000EE20 File Offset: 0x0000D020
		internal int ArraySize
		{
			get
			{
				return this.Array.Length;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000EE3C File Offset: 0x0000D03C
		internal int CodeSize
		{
			get
			{
				return this.Code.Length;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000EE58 File Offset: 0x0000D058
		internal int OutputSize
		{
			get
			{
				return this.Output.Length;
			}
		}

		// Token: 0x0400013E RID: 318
		private int _outputIdx;
	}
}

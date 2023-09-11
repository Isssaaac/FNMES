using System;

namespace DataMatrix.net
{
	// Token: 0x02000024 RID: 36
	internal static class DmtxCommon
	{
		// Token: 0x060001CC RID: 460 RVA: 0x0000EE74 File Offset: 0x0000D074
		internal static void GenReedSolEcc(DmtxMessage message, DmtxSymbolSize sizeIdx)
		{
			byte[] array = new byte[69];
			byte[] array2 = new byte[68];
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolErrorWords, sizeIdx);
			int num = symbolAttribute + symbolAttribute2;
			int symbolAttribute3 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribBlockErrorWords, sizeIdx);
			int symbolAttribute4 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
			if (symbolAttribute3 != symbolAttribute2 / symbolAttribute4)
			{
				throw new Exception("Error generation reed solomon error correction");
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 1;
			}
			for (int j = 1; j <= symbolAttribute3; j++)
			{
				for (int k = j - 1; k >= 0; k--)
				{
					array[k] = DmtxCommon.GfDoublify(array[k], j);
					if (k > 0)
					{
						array[k] = DmtxCommon.GfSum(array[k], array[k - 1]);
					}
				}
			}
			for (int l = 0; l < symbolAttribute4; l++)
			{
				for (int m = 0; m < array2.Length; m++)
				{
					array2[m] = 0;
				}
				for (int j = l; j < symbolAttribute; j += symbolAttribute4)
				{
					int b = (int)DmtxCommon.GfSum(array2[symbolAttribute3 - 1], message.Code[j]);
					for (int k = symbolAttribute3 - 1; k > 0; k--)
					{
						array2[k] = DmtxCommon.GfSum(array2[k - 1], DmtxCommon.GfProduct(array[k], b));
					}
					array2[0] = DmtxCommon.GfProduct(array[0], b);
				}
				int blockDataSize = DmtxCommon.GetBlockDataSize(sizeIdx, l);
				int num2 = symbolAttribute3;
				for (int j = l + symbolAttribute4 * blockDataSize; j < num; j += symbolAttribute4)
				{
					message.Code[j] = array2[--num2];
				}
				if (num2 != 0)
				{
					throw new Exception("Error generation error correction code!");
				}
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000F068 File Offset: 0x0000D268
		private static byte GfProduct(byte a, int b)
		{
			byte result;
			if (a == 0 || b == 0)
			{
				result = 0;
			}
			else
			{
				result = (byte)DmtxConstants.aLogVal[(DmtxConstants.logVal[(int)a] + DmtxConstants.logVal[b]) % 255];
			}
			return result;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000F0AC File Offset: 0x0000D2AC
		private static byte GfSum(byte a, byte b)
		{
			return (byte)(a ^ b);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000F0C4 File Offset: 0x0000D2C4
		private static byte GfDoublify(byte a, int b)
		{
			byte result;
			if (a == 0)
			{
				result = 0;
			}
			else if (b == 0)
			{
				result = a;
			}
			else
			{
				result = (byte)DmtxConstants.aLogVal[(DmtxConstants.logVal[(int)a] + b) % 255];
			}
			return result;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000F10C File Offset: 0x0000D30C
		internal static int GetSymbolAttribute(DmtxSymAttribute attribute, DmtxSymbolSize sizeIdx)
		{
			int result;
			if (sizeIdx < DmtxSymbolSize.DmtxSymbol10x10 || sizeIdx >= (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount))
			{
				result = DmtxConstants.DmtxUndefined;
			}
			else
			{
				switch (attribute)
				{
				case DmtxSymAttribute.DmtxSymAttribSymbolRows:
					result = DmtxConstants.SymbolRows[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribSymbolCols:
					result = DmtxConstants.SymbolCols[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribDataRegionRows:
					result = DmtxConstants.DataRegionRows[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribDataRegionCols:
					result = DmtxConstants.DataRegionCols[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribHorizDataRegions:
					result = DmtxConstants.HorizDataRegions[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribVertDataRegions:
					result = ((sizeIdx < (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount) ? DmtxConstants.HorizDataRegions[(int)sizeIdx] : 1);
					break;
				case DmtxSymAttribute.DmtxSymAttribMappingMatrixRows:
					result = DmtxConstants.DataRegionRows[(int)sizeIdx] * DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribVertDataRegions, sizeIdx);
					break;
				case DmtxSymAttribute.DmtxSymAttribMappingMatrixCols:
					result = DmtxConstants.DataRegionCols[(int)sizeIdx] * DmtxConstants.HorizDataRegions[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribInterleavedBlocks:
					result = DmtxConstants.InterleavedBlocks[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribBlockErrorWords:
					result = DmtxConstants.BlockErrorWords[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribBlockMaxCorrectable:
					result = DmtxConstants.BlockMaxCorrectable[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribSymbolDataWords:
					result = DmtxConstants.SymbolDataWords[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribSymbolErrorWords:
					result = DmtxConstants.BlockErrorWords[(int)sizeIdx] * DmtxConstants.InterleavedBlocks[(int)sizeIdx];
					break;
				case DmtxSymAttribute.DmtxSymAttribSymbolMaxCorrectable:
					result = DmtxConstants.BlockMaxCorrectable[(int)sizeIdx] * DmtxConstants.InterleavedBlocks[(int)sizeIdx];
					break;
				default:
					result = DmtxConstants.DmtxUndefined;
					break;
				}
			}
			return result;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000F254 File Offset: 0x0000D454
		internal static int GetBlockDataSize(DmtxSymbolSize sizeIdx, int blockIdx)
		{
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
			int num = symbolAttribute / symbolAttribute2;
			int result;
			if (symbolAttribute < 1 || symbolAttribute2 < 1)
			{
				result = DmtxConstants.DmtxUndefined;
			}
			else
			{
				result = ((sizeIdx == DmtxSymbolSize.DmtxSymbol144x144 && blockIdx < 8) ? (num + 1) : num);
			}
			return result;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000F2A8 File Offset: 0x0000D4A8
		internal static DmtxSymbolSize FindCorrectSymbolSize(int dataWords, DmtxSymbolSize sizeIdxRequest)
		{
			DmtxSymbolSize result;
			if (dataWords <= 0)
			{
				result = DmtxSymbolSize.DmtxSymbolShapeAuto;
			}
			else
			{
				DmtxSymbolSize dmtxSymbolSize3;
				if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto || sizeIdxRequest == DmtxSymbolSize.DmtxSymbolRectAuto)
				{
					DmtxSymbolSize dmtxSymbolSize;
					DmtxSymbolSize dmtxSymbolSize2;
					if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto)
					{
						dmtxSymbolSize = DmtxSymbolSize.DmtxSymbol10x10;
						dmtxSymbolSize2 = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
					}
					else
					{
						dmtxSymbolSize = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
						dmtxSymbolSize2 = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
					}
					for (dmtxSymbolSize3 = dmtxSymbolSize; dmtxSymbolSize3 < dmtxSymbolSize2; dmtxSymbolSize3++)
					{
						if (DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize3) >= dataWords)
						{
							break;
						}
					}
					if (dmtxSymbolSize3 == dmtxSymbolSize2)
					{
						return DmtxSymbolSize.DmtxSymbolShapeAuto;
					}
				}
				else
				{
					dmtxSymbolSize3 = sizeIdxRequest;
				}
				if (dataWords > DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, dmtxSymbolSize3))
				{
					result = DmtxSymbolSize.DmtxSymbolShapeAuto;
				}
				else
				{
					result = dmtxSymbolSize3;
				}
			}
			return result;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000F36C File Offset: 0x0000D56C
		internal static int GetBitsPerPixel(DmtxPackOrder pack)
		{
			if (pack <= DmtxPackOrder.DmtxPack8bppK)
			{
				if (pack == DmtxPackOrder.DmtxPack1bppK)
				{
					return 1;
				}
				if (pack == DmtxPackOrder.DmtxPack8bppK)
				{
					return 8;
				}
			}
			else
			{
				switch (pack)
				{
				case DmtxPackOrder.DmtxPack16bppRGB:
				case DmtxPackOrder.DmtxPack16bppRGBX:
				case DmtxPackOrder.DmtxPack16bppXRGB:
				case DmtxPackOrder.DmtxPack16bppBGR:
				case DmtxPackOrder.DmtxPack16bppBGRX:
				case DmtxPackOrder.DmtxPack16bppXBGR:
				case DmtxPackOrder.DmtxPack16bppYCbCr:
					return 16;
				default:
					switch (pack)
					{
					case DmtxPackOrder.DmtxPack24bppRGB:
					case DmtxPackOrder.DmtxPack24bppBGR:
					case DmtxPackOrder.DmtxPack24bppYCbCr:
						return 24;
					default:
						switch (pack)
						{
						case DmtxPackOrder.DmtxPack32bppRGBX:
						case DmtxPackOrder.DmtxPack32bppXRGB:
						case DmtxPackOrder.DmtxPack32bppBGRX:
						case DmtxPackOrder.DmtxPack32bppXBGR:
						case DmtxPackOrder.DmtxPack32bppCMYK:
							return 32;
						}
						break;
					}
					break;
				}
			}
			return DmtxConstants.DmtxUndefined;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000F418 File Offset: 0x0000D618
		internal static T Min<T>(T x, T y) where T : IComparable<T>
		{
			return (x.CompareTo(y) < 0) ? x : y;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000F440 File Offset: 0x0000D640
		internal static T Max<T>(T x, T y) where T : IComparable<T>
		{
			return (x.CompareTo(y) < 0) ? y : x;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000F468 File Offset: 0x0000D668
		internal static bool DecodeCheckErrors(byte[] code, int codeIndex, DmtxSymbolSize sizeIdx, int fix)
		{
			byte[] array = new byte[255];
			int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
			int symbolAttribute2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribBlockErrorWords, sizeIdx);
			int num = 0;
			for (int i = 0; i < symbolAttribute; i++)
			{
				int num2 = symbolAttribute2 + DmtxCommon.GetBlockDataSize(sizeIdx, i);
				for (int j = 0; j < num2; j++)
				{
					array[j] = code[j * symbolAttribute + i];
				}
				num = num;
				for (int j = 0; j < num2; j++)
				{
					code[j * symbolAttribute + i] = array[j];
				}
			}
			return fix == DmtxConstants.DmtxUndefined || fix < 0 || fix >= num;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000F52C File Offset: 0x0000D72C
		internal static double RightAngleTrueness(DmtxVector2 c0, DmtxVector2 c1, DmtxVector2 c2, double angle)
		{
			DmtxVector2 dmtxVector = c0 - c1;
			DmtxVector2 dmtxVector2 = c2 - c1;
			dmtxVector.Norm();
			dmtxVector2.Norm();
			DmtxMatrix3 matrix = DmtxMatrix3.Rotate(angle);
			dmtxVector2 *= matrix;
			return dmtxVector.Dot(dmtxVector2);
		}
	}
}

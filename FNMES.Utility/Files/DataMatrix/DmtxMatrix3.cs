using System;

namespace DataMatrix.net
{
	// Token: 0x0200001A RID: 26
	internal class DmtxMatrix3
	{
		// Token: 0x060000BE RID: 190 RVA: 0x000051C1 File Offset: 0x000033C1
		private DmtxMatrix3()
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000051CC File Offset: 0x000033CC
		internal DmtxMatrix3(DmtxMatrix3 src)
		{
			double[,] array = new double[3, 3];
			array[0, 0] = src[0, 0];
			array[0, 1] = src[0, 1];
			array[0, 2] = src[0, 2];
			array[1, 0] = src[1, 0];
			array[1, 1] = src[1, 1];
			array[1, 2] = src[1, 2];
			array[2, 0] = src[2, 0];
			array[2, 1] = src[2, 1];
			array[2, 2] = src[2, 2];
			this._data = array;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005284 File Offset: 0x00003484
		internal static DmtxMatrix3 Identity()
		{
			return DmtxMatrix3.Translate(0.0, 0.0);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000052F8 File Offset: 0x000034F8
		internal static DmtxMatrix3 Translate(double tx, double ty)
		{
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[,]
			{
				{
					1.0,
					0.0,
					0.0
				},
				{
					0.0,
					1.0,
					0.0
				},
				{
					0.0,
					0.0,
					1.0
				}
			};
			array[2, 0] = tx;
			array[2, 1] = ty;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005340 File Offset: 0x00003540
		internal static DmtxMatrix3 Rotate(double angle)
		{
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = Math.Cos(angle);
			array[0, 1] = Math.Sin(angle);
			array[1, 0] = -Math.Sin(angle);
			array[1, 1] = Math.Cos(angle);
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000053B4 File Offset: 0x000035B4
		internal static DmtxMatrix3 Scale(double sx, double sy)
		{
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = sx;
			array[1, 1] = sy;
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005450 File Offset: 0x00003650
		internal static DmtxMatrix3 Shear(double shx, double shy)
		{
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[,]
			{
				{
					1.0,
					0.0,
					0.0
				},
				{
					0.0,
					1.0,
					0.0
				},
				{
					0.0,
					0.0,
					1.0
				}
			};
			array[0, 1] = shy;
			array[1, 0] = shx;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005498 File Offset: 0x00003698
		internal static DmtxMatrix3 LineSkewTop(double b0, double b1, double sz)
		{
			if (b0 < DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("b0 must be larger than zero in top line skew transformation");
			}
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = b1 / b0;
			array[0, 2] = (b1 - b0) / (sz * b0);
			array[1, 1] = sz / b0;
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000551C File Offset: 0x0000371C
		internal static DmtxMatrix3 LineSkewTopInv(double b0, double b1, double sz)
		{
			if (b1 < DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("b1 must be larger than zero in top line skew transformation (inverse)");
			}
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = b0 / b1;
			array[0, 2] = (b0 - b1) / (sz * b1);
			array[1, 1] = b0 / sz;
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000055A0 File Offset: 0x000037A0
		internal static DmtxMatrix3 LineSkewSide(double b0, double b1, double sz)
		{
			if (b0 < DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("b0 must be larger than zero in side line skew transformation (inverse)");
			}
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = sz / b0;
			array[1, 1] = b1 / b0;
			array[1, 2] = (b1 - b0) / (sz * b0);
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005624 File Offset: 0x00003824
		internal static DmtxMatrix3 LineSkewSideInv(double b0, double b1, double sz)
		{
			if (b1 < DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("b1 must be larger than zero in top line skew transformation (inverse)");
			}
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] array = new double[3, 3];
			array[0, 0] = b0 / sz;
			array[1, 1] = b0 / b1;
			array[1, 2] = (b0 - b1) / (sz * b1);
			array[2, 2] = 1.0;
			dmtxMatrix2._data = array;
			return dmtxMatrix;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000056A8 File Offset: 0x000038A8
		public static DmtxMatrix3 operator *(DmtxMatrix3 m1, DmtxMatrix3 m2)
		{
			DmtxMatrix3 dmtxMatrix = new DmtxMatrix3();
			DmtxMatrix3 dmtxMatrix2 = dmtxMatrix;
			double[,] data = new double[3, 3];
			dmtxMatrix2._data = data;
			DmtxMatrix3 dmtxMatrix3 = dmtxMatrix;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						DmtxMatrix3 dmtxMatrix4;
						int i2;
						int j2;
						(dmtxMatrix4 = dmtxMatrix3)[i2 = i, j2 = j] = dmtxMatrix4[i2, j2] + m1[i, k] * m2[k, j];
					}
				}
			}
			return dmtxMatrix3;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005744 File Offset: 0x00003944
		public static DmtxVector2 operator *(DmtxVector2 vector, DmtxMatrix3 matrix)
		{
			double num = Math.Abs(vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + matrix[2, 2]);
			if (num <= DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("Multiplication of vector and matrix resulted in invalid result");
			}
			return new DmtxVector2((vector.X * matrix[0, 0] + vector.Y * matrix[1, 0] + matrix[2, 0]) / num, (vector.X * matrix[0, 1] + vector.Y * matrix[1, 1] + matrix[2, 1]) / num);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000057F8 File Offset: 0x000039F8
		public override string ToString()
		{
			return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n{6}\t{7}\t{8}\n", new object[]
			{
				this._data[0, 0],
				this._data[0, 1],
				this._data[0, 2],
				this._data[1, 0],
				this._data[1, 1],
				this._data[1, 2],
				this._data[2, 0],
				this._data[2, 1],
				this._data[2, 2]
			});
		}

		// Token: 0x17000056 RID: 86
		internal double this[int i, int j]
		{
			get
			{
				return this._data[i, j];
			}
			set
			{
				this._data[i, j] = value;
			}
		}

		// Token: 0x04000102 RID: 258
		private double[,] _data;
	}
}

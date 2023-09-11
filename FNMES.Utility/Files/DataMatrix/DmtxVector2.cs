using System;

namespace DataMatrix.net
{
	// Token: 0x0200001F RID: 31
	internal class DmtxVector2
	{
		// Token: 0x06000174 RID: 372 RVA: 0x0000D218 File Offset: 0x0000B418
		internal DmtxVector2()
		{
			this.X = 0.0;
			this.Y = 0.0;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000D243 File Offset: 0x0000B443
		internal DmtxVector2(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000D260 File Offset: 0x0000B460
		public static DmtxVector2 operator +(DmtxVector2 v1, DmtxVector2 v2)
		{
			DmtxVector2 dmtxVector = new DmtxVector2(v1.X, v1.Y);
			dmtxVector.X += v2.X;
			dmtxVector.Y += v2.Y;
			return dmtxVector;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000D2B0 File Offset: 0x0000B4B0
		public static DmtxVector2 operator -(DmtxVector2 v1, DmtxVector2 v2)
		{
			DmtxVector2 dmtxVector = new DmtxVector2(v1.X, v1.Y);
			dmtxVector.X -= v2.X;
			dmtxVector.Y -= v2.Y;
			return dmtxVector;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000D300 File Offset: 0x0000B500
		public static DmtxVector2 operator *(DmtxVector2 v1, double factor)
		{
			return new DmtxVector2(v1.X * factor, v1.Y * factor);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000D328 File Offset: 0x0000B528
		internal double Cross(DmtxVector2 v2)
		{
			return this.X * v2.Y - this.Y * v2.X;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000D358 File Offset: 0x0000B558
		internal double Norm()
		{
			double num = this.Mag();
			double result;
			if (num <= DmtxConstants.DmtxAlmostZero)
			{
				result = -1.0;
			}
			else
			{
				this.X /= num;
				this.Y /= num;
				result = num;
			}
			return result;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000D3AC File Offset: 0x0000B5AC
		internal double Dot(DmtxVector2 v2)
		{
			return Math.Sqrt(this.X * v2.X + this.Y * v2.Y);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000D3E0 File Offset: 0x0000B5E0
		internal double Mag()
		{
			return Math.Sqrt(this.X * this.X + this.Y * this.Y);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000D414 File Offset: 0x0000B614
		internal double DistanceFromRay2(DmtxRay2 ray)
		{
			if (Math.Abs(1.0 - ray.V.Mag()) > DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("DistanceFromRay2: The ray's V vector must be a unit vector");
			}
			return ray.V.Cross(this - ray.P);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000D474 File Offset: 0x0000B674
		internal double DistanceAlongRay2(DmtxRay2 ray)
		{
			if (Math.Abs(1.0 - ray.V.Mag()) > DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("DistanceAlongRay2: The ray's V vector must be a unit vector");
			}
			return (this - ray.P).Dot(ray.V);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000D4D4 File Offset: 0x0000B6D4
		internal bool Intersect(DmtxRay2 p0, DmtxRay2 p1)
		{
			double num = p1.V.Cross(p0.V);
			bool result;
			if (Math.Abs(num) < DmtxConstants.DmtxAlmostZero)
			{
				result = false;
			}
			else
			{
				double num2 = p1.V.Cross(p1.P - p0.P);
				result = this.PointAlongRay2(p0, num2 / num);
			}
			return result;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000D538 File Offset: 0x0000B738
		internal bool PointAlongRay2(DmtxRay2 ray, double t)
		{
			if (Math.Abs(1.0 - ray.V.Mag()) > DmtxConstants.DmtxAlmostZero)
			{
				throw new ArgumentException("PointAlongRay: The ray's V vector must be a unit vector");
			}
			DmtxVector2 dmtxVector = new DmtxVector2(ray.V.X * t, ray.V.Y * t);
			this.X = ray.P.X + dmtxVector.X;
			this.Y = ray.P.Y + dmtxVector.Y;
			return true;
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000D5D4 File Offset: 0x0000B7D4
		// (set) Token: 0x06000182 RID: 386 RVA: 0x0000D5EB File Offset: 0x0000B7EB
		internal double X { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000D5F4 File Offset: 0x0000B7F4
		// (set) Token: 0x06000184 RID: 388 RVA: 0x0000D60B File Offset: 0x0000B80B
		internal double Y { get; set; }
	}
}

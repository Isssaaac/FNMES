using System;
using System.Collections.Generic;

namespace DataMatrix.net
{
	// Token: 0x0200001C RID: 28
	internal class DmtxDecode
	{
        private byte[] _cache;
        private int _edgeMax;
        private int _edgeMin;
        private int _edgeThresh;
        private DmtxScanGrid _grid;
        private DmtxImage _image;
        private int _scale;
        private int _scanGap;
        private DmtxSymbolSize _sizeIdxExpected;
        private double _squareDevn;
        private int _xMax;
        private int _xMin;
        private int _yMax;
        private int _yMin;

        internal DmtxDecode(DmtxImage img, int scale)
        {
            int num = img.Width / scale;
            int num2 = img.Height / scale;
            this._edgeMin = DmtxConstants.DmtxUndefined;
            this._edgeMax = DmtxConstants.DmtxUndefined;
            this._scanGap = 1;
            this._squareDevn = Math.Cos(0.87266462599716477);
            this._sizeIdxExpected = DmtxSymbolSize.DmtxSymbolShapeAuto;
            this._edgeThresh = 10;
            this._xMin = 0;
            this._xMax = num - 1;
            this._yMin = 0;
            this._yMax = num2 - 1;
            this._scale = scale;
            this._cache = new byte[num * num2];
            this._image = img;
            this.ValidateSettingsAndInitScanGrid();
        }

        internal void CacheFillQuad(DmtxPixelLoc p0, DmtxPixelLoc p1, DmtxPixelLoc p2, DmtxPixelLoc p3)
        {
            int num2;
            int num3;
            DmtxBresLine[] lineArray = new DmtxBresLine[4];
            DmtxPixelLoc locInside = new DmtxPixelLoc
            {
                X = 0,
                Y = 0
            };
            lineArray[0] = new DmtxBresLine(p0, p1, locInside);
            lineArray[1] = new DmtxBresLine(p1, p2, locInside);
            lineArray[2] = new DmtxBresLine(p2, p3, locInside);
            lineArray[3] = new DmtxBresLine(p3, p0, locInside);
            int x = this._yMax;
            int num5 = 0;
            x = DmtxCommon.Min<int>(x, p0.Y);
            num5 = DmtxCommon.Max<int>(num5, p0.Y);
            x = DmtxCommon.Min<int>(x, p1.Y);
            num5 = DmtxCommon.Max<int>(num5, p1.Y);
            x = DmtxCommon.Min<int>(x, p2.Y);
            num5 = DmtxCommon.Max<int>(num5, p2.Y);
            x = DmtxCommon.Min<int>(x, p3.Y);
            num5 = DmtxCommon.Max<int>(num5, p3.Y);
            int num6 = (num5 - x) + 1;
            int[] numArray = new int[num6];
            int[] numArray2 = new int[num6];
            for (num2 = 0; num2 < num6; num2++)
            {
                numArray[num2] = this._xMax;
            }
            for (num2 = 0; num2 < 4; num2++)
            {
                while ((lineArray[num2].Loc.X != lineArray[num2].Loc1.X) || (lineArray[num2].Loc.Y != lineArray[num2].Loc1.Y))
                {
                    num3 = lineArray[num2].Loc.Y - x;
                    numArray[num3] = DmtxCommon.Min<int>(numArray[num3], lineArray[num2].Loc.X);
                    numArray2[num3] = DmtxCommon.Max<int>(numArray2[num3], lineArray[num2].Loc.X);
                    lineArray[num2].Step(1, 0);
                }
            }
            for (int i = x; (i < num5) && (i < this._yMax); i++)
            {
                num3 = i - x;
                for (int j = numArray[num3]; (j < numArray2[num3]) && (j < this._xMax); j++)
                {
                    if ((j >= 0) && (i >= 0))
                    {
                        try
                        {
                            int cacheIndex = this.GetCacheIndex(j, i);
                            this._cache[cacheIndex] = (byte)(this._cache[cacheIndex] | 0x80);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private int CountJumpTally(DmtxRegion reg, int xStart, int yStart, DmtxDirection dir)
        {
            int num2 = 0;
            int num4 = 0;
            int dmtxModuleOn = DmtxConstants.DmtxModuleOn;
            int num6 = 0;
            if ((xStart != 0) && (yStart != 0))
            {
                throw new Exception("CountJumpTally failed, xStart or yStart must be zero!");
            }
            if (dir == DmtxDirection.DmtxDirRight)
            {
                num2 = 1;
            }
            else
            {
                num4 = 1;
            }
            if ((((xStart == -1) || (xStart == reg.SymbolCols)) || (yStart == -1)) || (yStart == reg.SymbolRows))
            {
                dmtxModuleOn = DmtxConstants.DmtxModuleOff;
            }
            bool flag = reg.OffColor > reg.OnColor;
            int num7 = Math.Abs((int)((0.4 * (reg.OnColor - reg.OffColor)) + 0.5));
            int num8 = this.ReadModuleColor(reg, yStart, xStart, reg.SizeIdx, reg.FlowBegin.Plane);
            int num9 = flag ? (reg.OffColor - num8) : (num8 - reg.OffColor);
            int symbolCol = xStart + num2;
            for (int i = yStart + num4; ((dir == DmtxDirection.DmtxDirRight) && (symbolCol < reg.SymbolCols)) || ((dir == DmtxDirection.DmtxDirUp) && (i < reg.SymbolRows)); i += num4)
            {
                int num10 = num9;
                num8 = this.ReadModuleColor(reg, i, symbolCol, reg.SizeIdx, reg.FlowBegin.Plane);
                num9 = flag ? (reg.OffColor - num8) : (num8 - reg.OffColor);
                if (dmtxModuleOn == DmtxConstants.DmtxModuleOff)
                {
                    if (num9 > (num10 + num7))
                    {
                        num6++;
                        dmtxModuleOn = DmtxConstants.DmtxModuleOn;
                    }
                }
                else if (num9 < (num10 - num7))
                {
                    num6++;
                    dmtxModuleOn = DmtxConstants.DmtxModuleOff;
                }
                symbolCol += num2;
            }
            return num6;
        }

        private int DecodeGetCache(int x, int y)
        {
            int width = this.Width;
            int height = this.Height;
            if ((((x < 0) || (x >= width)) || (y < 0)) || (y >= height))
            {
                return DmtxConstants.DmtxUndefined;
            }
            return ((y * width) + x);
        }

        private int DistanceSquared(DmtxPixelLoc a, DmtxPixelLoc b)
        {
            int num = a.X - b.X;
            int num2 = a.Y - b.Y;
            return ((num * num) + (num2 * num2));
        }

        private DmtxBestLine FindBestSolidLine(DmtxRegion reg, int step0, int step1, int streamDir, int houghAvoid)
        {
            int num13;
            int[,] numArray = new int[3, DmtxConstants.DmtxHoughRes];
            char[] chArray = new char[DmtxConstants.DmtxHoughRes];
            int sign = 0;
            int stepsTotal = 0;
            DmtxBestLine line = new DmtxBestLine();
            int num5 = 0;
            int num6 = 0;
            if (step0 != 0)
            {
                if (step0 > 0)
                {
                    sign = 1;
                    stepsTotal = ((step1 - step0) + reg.StepsTotal) % reg.StepsTotal;
                }
                else
                {
                    sign = -1;
                    stepsTotal = ((step0 - step1) + reg.StepsTotal) % reg.StepsTotal;
                }
                if (stepsTotal == 0)
                {
                    stepsTotal = reg.StepsTotal;
                }
            }
            else if (step1 != 0)
            {
                sign = (step1 > 0) ? 1 : -1;
                stepsTotal = Math.Abs(step1);
            }
            else if (step1 == 0)
            {
                sign = 1;
                stepsTotal = reg.StepsTotal;
            }
            if (sign != streamDir)
            {
                throw new Exception("Sign must equal stream direction!");
            }
            DmtxFollow followBeg = this.FollowSeek(reg, step0);
            DmtxPixelLoc loc = followBeg.Loc;
            line.StepNeg = num13 = step0;
            line.StepBeg = line.StepPos = num13;
            line.LocBeg = followBeg.Loc;
            line.LocPos = followBeg.Loc;
            line.LocNeg = followBeg.Loc;
            int index = 0;
            while (index < DmtxConstants.DmtxHoughRes)
            {
                if (houghAvoid == DmtxConstants.DmtxUndefined)
                {
                    chArray[index] = '\x0001';
                }
                else
                {
                    int num7 = (houghAvoid + (DmtxConstants.DmtxHoughRes / 6)) % DmtxConstants.DmtxHoughRes;
                    int num8 = ((houghAvoid - (DmtxConstants.DmtxHoughRes / 6)) + DmtxConstants.DmtxHoughRes) % DmtxConstants.DmtxHoughRes;
                    if (num7 > num8)
                    {
                        chArray[index] = ((index > num7) || (index < num8)) ? '\x0001' : '\0';
                    }
                    else
                    {
                        chArray[index] = ((index > num7) && (index < num8)) ? '\x0001' : '\0';
                    }
                }
                index++;
            }
            for (int i = 0; i < stepsTotal; i++)
            {
                int num9 = followBeg.Loc.X - loc.X;
                int num10 = followBeg.Loc.Y - loc.Y;
                for (index = 0; index < DmtxConstants.DmtxHoughRes; index++)
                {
                    if (chArray[index] != '\0')
                    {
                        int num11 = (DmtxConstants.rHvX[index] * num10) - (DmtxConstants.rHvY[index] * num9);
                        if ((num11 >= -384) && (num11 <= 0x180))
                        {
                            int num12;
                            if (num11 > 0x80)
                            {
                                num12 = 2;
                            }
                            else if (num11 >= -128)
                            {
                                num12 = 1;
                            }
                            else
                            {
                                num12 = 0;
                            }
                            numArray[num12, index]++;
                            if (numArray[num12, index] > numArray[num6, num5])
                            {
                                num5 = index;
                                num6 = num12;
                            }
                        }
                    }
                }
                followBeg = this.FollowStep(reg, followBeg, sign);
            }
            line.Angle = num5;
            line.HOffset = num6;
            line.Mag = numArray[num6, num5];
            return line;
        }

        private DmtxBestLine FindBestSolidLine2(DmtxPixelLoc loc0, int tripSteps, int sign, int houghAvoid)
        {
            DmtxPixelLoc loc2;
            int num11;
            int[,] numArray = new int[3, DmtxConstants.DmtxHoughRes];
            char[] chArray = new char[DmtxConstants.DmtxHoughRes];
            DmtxBestLine line = new DmtxBestLine();
            int num3 = 0;
            int num4 = 0;
            DmtxFollow followBeg = this.FollowSeekLoc(loc0);
            line.LocNeg = loc2 = followBeg.Loc;
            line.LocPos = loc2 ;
            DmtxPixelLoc loc = line.LocBeg = loc2;
            line.StepNeg = num11 = 0;
            line.StepBeg = line.StepPos = num11;
            int index = 0;
            while (index < DmtxConstants.DmtxHoughRes)
            {
                if (houghAvoid == DmtxConstants.DmtxUndefined)
                {
                    chArray[index] = '\x0001';
                }
                else
                {
                    int num5 = (houghAvoid + (DmtxConstants.DmtxHoughRes / 6)) % DmtxConstants.DmtxHoughRes;
                    int num6 = ((houghAvoid - (DmtxConstants.DmtxHoughRes / 6)) + DmtxConstants.DmtxHoughRes) % DmtxConstants.DmtxHoughRes;
                    if (num5 > num6)
                    {
                        chArray[index] = ((index > num5) || (index < num6)) ? '\x0001' : '\0';
                    }
                    else
                    {
                        chArray[index] = ((index > num5) && (index < num6)) ? '\x0001' : '\0';
                    }
                }
                index++;
            }
            for (int i = 0; i < tripSteps; i++)
            {
                int num7 = followBeg.Loc.X - loc.X;
                int num8 = followBeg.Loc.Y - loc.Y;
                for (index = 0; index < DmtxConstants.DmtxHoughRes; index++)
                {
                    if (chArray[index] != '\0')
                    {
                        int num9 = (DmtxConstants.rHvX[index] * num8) - (DmtxConstants.rHvY[index] * num7);
                        if ((num9 >= -384) && (num9 <= 0x180))
                        {
                            int num10;
                            if (num9 > 0x80)
                            {
                                num10 = 2;
                            }
                            else if (num9 >= -128)
                            {
                                num10 = 1;
                            }
                            else
                            {
                                num10 = 0;
                            }
                            numArray[num10, index]++;
                            if (numArray[num10, index] > numArray[num4, num3])
                            {
                                num3 = index;
                                num4 = num10;
                            }
                        }
                    }
                }
                followBeg = this.FollowStep2(followBeg, sign);
            }
            line.Angle = num3;
            line.HOffset = num4;
            line.Mag = numArray[num4, num3];
            return line;
        }

        private DmtxPointFlow FindStrongestNeighbor(DmtxPointFlow center, int sign)
        {
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DmtxPointFlow[] flowArray = new DmtxPointFlow[8];
            int num = (sign < 0) ? center.Depart : ((center.Depart + 4) % 8);
            int num2 = 0;
            int dmtxUndefined = DmtxConstants.DmtxUndefined;
            for (int i = 0; i < 8; i++)
            {
                loc.X = center.Loc.X + DmtxConstants.DmtxPatternX[i];
                loc.Y = center.Loc.Y + DmtxConstants.DmtxPatternY[i];
                int index = this.DecodeGetCache(loc.X, loc.Y);
                if (index != DmtxConstants.DmtxUndefined)
                {
                    if ((this._cache[index] & 0x80) != 0)
                    {
                        if (++num2 > 2)
                        {
                            return DmtxConstants.DmtxBlankEdge;
                        }
                    }
                    else
                    {
                        int num6 = Math.Abs((int)(num - i));
                        if (num6 > 4)
                        {
                            num6 = 8 - num6;
                        }
                        if (num6 <= 1)
                        {
                            flowArray[i] = this.GetPointFlow(center.Plane, loc, i);
                            if (((dmtxUndefined == DmtxConstants.DmtxUndefined) || (flowArray[i].Mag > flowArray[dmtxUndefined].Mag)) || ((flowArray[i].Mag == flowArray[dmtxUndefined].Mag) && ((i & 1) != 0)))
                            {
                                dmtxUndefined = i;
                            }
                        }
                    }
                }
            }
            return ((dmtxUndefined == DmtxConstants.DmtxUndefined) ? DmtxConstants.DmtxBlankEdge : flowArray[dmtxUndefined]);
        }

        private void FindTravelLimits(DmtxRegion reg, ref DmtxBestLine line)
        {
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            DmtxFollow follow;
            DmtxPixelLoc loc;
            DmtxFollow followBeg = follow = this.FollowSeek(reg, line.StepBeg);
            DmtxPixelLoc loc2 = followBeg.Loc;
            int num11 = DmtxConstants.rHvX[line.Angle];
            int num12 = DmtxConstants.rHvY[line.Angle];
            int num13 = 0;
            DmtxPixelLoc b = loc = followBeg.Loc;
            int num14 = num2 = 0;
            int num15 = num3 = num4 = num5 = num6 = 0;
            int num16 = num7 = num8 = num9 = num10 = 0;
            for (int i = 0; i < (reg.StepsTotal / 2); i++)
            {
                int num17;
                int num18;
                int num19;
                bool flag = (i < 10) || (Math.Abs(num15) < Math.Abs(num14));
                bool flag2 = (i < 10) || (Math.Abs(num16) < Math.Abs(num2));
                if (flag)
                {
                    num18 = followBeg.Loc.X - loc2.X;
                    num19 = followBeg.Loc.Y - loc2.Y;
                    num14 = (num11 * num18) + (num12 * num19);
                    num15 = (num11 * num19) - (num12 * num18);
                    if ((num15 >= -768) && (num15 <= 0x300))
                    {
                        num17 = this.DistanceSquared(followBeg.Loc, loc);
                        if (num17 > num13)
                        {
                            b = followBeg.Loc;
                            num13 = num17;
                            line.StepPos = followBeg.Step;
                            line.LocPos = followBeg.Loc;
                            num5 = num3;
                            num6 = num4;
                        }
                    }
                    else
                    {
                        num3 = DmtxCommon.Min<int>(num3, num15);
                        num4 = DmtxCommon.Max<int>(num4, num15);
                    }
                }
                else if (!flag2)
                {
                    break;
                }
                if (flag2)
                {
                    num18 = follow.Loc.X - loc2.X;
                    num19 = follow.Loc.Y - loc2.Y;
                    num2 = (num11 * num18) + (num12 * num19);
                    num16 = (num11 * num19) - (num12 * num18);
                    if ((num16 >= -768) && (num16 < 0x300))
                    {
                        num17 = this.DistanceSquared(follow.Loc, b);
                        if (num17 > num13)
                        {
                            loc = follow.Loc;
                            num13 = num17;
                            line.StepNeg = follow.Step;
                            line.LocNeg = follow.Loc;
                            num9 = num7;
                            num10 = num8;
                        }
                    }
                    else
                    {
                        num7 = DmtxCommon.Min<int>(num7, num16);
                        num8 = DmtxCommon.Max<int>(num8, num16);
                    }
                }
                followBeg = this.FollowStep(reg, followBeg, 1);
                follow = this.FollowStep(reg, follow, -1);
            }
            line.Devn = (double)(DmtxCommon.Max<int>(num6 - num5, num10 - num9) / 0x100);
            line.DistSq = num13;
        }

        private DmtxFollow FollowSeek(DmtxRegion reg, int seek)
        {
            DmtxFollow followBeg = new DmtxFollow
            {
                Loc = reg.FlowBegin.Loc,
                Step = 0,
                Ptr = this._cache
            };
            followBeg.PtrIndex = this.DecodeGetCache(followBeg.Loc.X, followBeg.Loc.Y);
            int sign = (seek > 0) ? 1 : -1;
            for (int i = 0; i != seek; i += sign)
            {
                followBeg = this.FollowStep(reg, followBeg, sign);
                if (Math.Abs(followBeg.Step) > reg.StepsTotal)
                {
                    throw new Exception("Follow step count larger total step count!");
                }
            }
            return followBeg;
        }

        private DmtxFollow FollowSeekLoc(DmtxPixelLoc loc)
        {
            DmtxFollow follow = new DmtxFollow
            {
                Loc = loc,
                Step = 0,
                Ptr = this._cache
            };
            follow.PtrIndex = this.DecodeGetCache(follow.Loc.X, follow.Loc.Y);
            return follow;
        }

        private DmtxFollow FollowStep(DmtxRegion reg, DmtxFollow followBeg, int sign)
        {
            int num;
            DmtxFollow follow = new DmtxFollow();
            if (Math.Abs(sign) != 1)
            {
                throw new Exception("Invalid parameter 'sign', can only be -1 or +1");
            }
            int num2 = reg.StepsTotal + 1;
            if (sign > 0)
            {
                num = (num2 + (followBeg.Step % num2)) % num2;
            }
            else
            {
                num = (num2 - (followBeg.Step % num2)) % num2;
            }
            if ((sign > 0) && (num == reg.JumpToNeg))
            {
                follow.Loc = reg.FinalNeg;
            }
            else if ((sign < 0) && (num == reg.JumpToPos))
            {
                follow.Loc = reg.FinalPos;
            }
            else
            {
                int index = (sign < 0) ? (followBeg.Neighbor & 7) : ((followBeg.Neighbor & 0x38) >> 3);
                DmtxPixelLoc loc = new DmtxPixelLoc
                {
                    X = followBeg.Loc.X + DmtxConstants.DmtxPatternX[index],
                    Y = followBeg.Loc.Y + DmtxConstants.DmtxPatternY[index]
                };
                follow.Loc = loc;
            }
            follow.Step = followBeg.Step + sign;
            follow.Ptr = this._cache;
            follow.PtrIndex = this.DecodeGetCache(follow.Loc.X, follow.Loc.Y);
            return follow;
        }

        private DmtxFollow FollowStep2(DmtxFollow followBeg, int sign)
        {
            DmtxFollow follow = new DmtxFollow();
            if (Math.Abs(sign) != 1)
            {
                throw new Exception("Invalid parameter 'sign', can only be -1 or +1");
            }
            if ((followBeg.Neighbor & 0x40) == 0)
            {
                throw new Exception("Invalid value for neighbor!");
            }
            int index = (sign < 0) ? (followBeg.Neighbor & 7) : ((followBeg.Neighbor & 0x38) >> 3);
            DmtxPixelLoc loc = new DmtxPixelLoc
            {
                X = followBeg.Loc.X + DmtxConstants.DmtxPatternX[index],
                Y = followBeg.Loc.Y + DmtxConstants.DmtxPatternY[index]
            };
            follow.Loc = loc;
            follow.Step = followBeg.Step + sign;
            follow.Ptr = this._cache;
            follow.PtrIndex = this.DecodeGetCache(follow.Loc.X, follow.Loc.Y);
            return follow;
        }

        internal int GetCacheIndex(int x, int y)
        {
            if ((((x < 0) || (x >= this.Width)) || (y < 0)) || (y >= this.Height))
            {
                throw new ArgumentException("Error: x and/or y outside cache size");
            }
            return ((y * this.Width) + x);
        }

        private bool GetPixelValue(int x, int y, int channel, ref int value)
        {
            int num = x * this._scale;
            int num2 = y * this._scale;
            return this._image.GetPixelValue(num, num2, channel, ref value);
        }

        private DmtxPointFlow GetPointFlow(int colorPlane, DmtxPixelLoc loc, int arrive)
        {
            int[] numArray = new int[] { 0, 1, 2, 1, 0, -1, -2, -1 };
            int[] numArray2 = new int[4];
            int[] numArray3 = new int[8];
            DmtxPointFlow flow = new DmtxPointFlow();
            int index = 0;
            while (index < 8)
            {
                int x = loc.X + DmtxConstants.DmtxPatternX[index];
                int y = loc.Y + DmtxConstants.DmtxPatternY[index];
                if (!this.GetPixelValue(x, y, colorPlane, ref numArray3[index]))
                {
                    return DmtxConstants.DmtxBlankEdge;
                }
                index++;
            }
            int num5 = 0;
            for (int i = 0; i < 4; i++)
            {
                for (index = 0; index < 8; index++)
                {
                    int num6 = ((index - i) + 8) % 8;
                    if (numArray[num6] != 0)
                    {
                        int num7 = numArray3[index];
                        switch (numArray[num6])
                        {
                            case -2:
                                numArray2[i] -= 2 * num7;
                                break;

                            case -1:
                                numArray2[i] -= num7;
                                break;

                            case 1:
                                numArray2[i] += num7;
                                break;

                            case 2:
                                numArray2[i] += 2 * num7;
                                break;
                        }
                    }
                }
                if ((i != 0) && (Math.Abs(numArray2[i]) > Math.Abs(numArray2[num5])))
                {
                    num5 = i;
                }
            }
            flow.Plane = colorPlane;
            flow.Arrive = arrive;
            flow.Depart = (numArray2[num5] > 0) ? (num5 + 4) : num5;
            flow.Mag = Math.Abs(numArray2[num5]);
            flow.Loc = loc;
            return flow;
        }

        internal DmtxMessage MatrixRegion(DmtxRegion reg, int fix)
        {
            DmtxMessage msg = new DmtxMessage(reg.SizeIdx, DmtxFormat.Matrix);
            DmtxVector2 vector = new DmtxVector2();
            DmtxVector2 vector2 = new DmtxVector2();
            DmtxVector2 vector3 = new DmtxVector2();
            DmtxVector2 vector4 = new DmtxVector2();
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DmtxPixelLoc loc2 = new DmtxPixelLoc();
            DmtxPixelLoc loc3 = new DmtxPixelLoc();
            DmtxPixelLoc loc4 = new DmtxPixelLoc();
            if (!this.PopulateArrayFromMatrix(reg, msg))
            {
                throw new Exception("Populating Array from matrix failed!");
            }
            ModulePlacementEcc200(msg.Array, msg.Code, reg.SizeIdx, (DmtxConstants.DmtxModuleOnRed | DmtxConstants.DmtxModuleOnGreen) | DmtxConstants.DmtxModuleOnBlue);
            if (!DmtxCommon.DecodeCheckErrors(msg.Code, 0, reg.SizeIdx, fix))
            {
                return null;
            }
            vector.X = vector3.X = vector.Y = vector2.Y = -0.1;
            vector2.X = vector4.X = vector3.Y = vector4.Y = 1.1;
            vector *= reg.Fit2Raw;
            vector2 *= reg.Fit2Raw;
            vector3 *= reg.Fit2Raw;
            vector3 *= reg.Fit2Raw;
            loc.X = (int)(0.5 + vector.X);
            loc.Y = (int)(0.5 + vector.Y);
            loc3.X = (int)(0.5 + vector3.X);
            loc3.Y = (int)(0.5 + vector3.Y);
            loc2.X = (int)(0.5 + vector2.X);
            loc2.Y = (int)(0.5 + vector2.Y);
            loc4.X = (int)(0.5 + vector4.X);
            loc4.Y = (int)(0.5 + vector4.Y);
            this.CacheFillQuad(loc, loc2, loc4, loc3);
            msg.DecodeDataStream(reg.SizeIdx, null);
            return msg;
        }

        private bool MatrixRegionAlignCalibEdge(DmtxRegion reg, DmtxEdge edgeLoc)
        {
            int polarity;
            int angle;
            DmtxSymbolSize dmtxSymbolSquareAuto;
            DmtxFollow follow;
            DmtxVector2 vector = new DmtxVector2();
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DmtxPixelLoc locInside = new DmtxPixelLoc();
            vector.X = 0.0;
            vector.Y = 0.0;
            vector *= reg.Fit2Raw;
            locInside.X = (int)(vector.X + 0.5);
            locInside.Y = (int)(vector.Y + 0.5);
            if ((this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto) || ((this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol10x10) && (this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol144x144)))
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolSquareAuto;
            }
            else if ((this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto) || ((this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol8x18) && (this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol16x48)))
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolRectAuto;
            }
            else
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolShapeAuto;
            }
            if (edgeLoc == DmtxEdge.DmtxEdgeTop)
            {
                polarity = reg.Polarity * -1;
                angle = reg.LeftLine.Angle;
                follow = this.FollowSeekLoc(reg.LocT);
                vector.X = 0.8;
                vector.Y = (dmtxSymbolSquareAuto == DmtxSymbolSize.DmtxSymbolRectAuto) ? 0.2 : 0.6;
            }
            else
            {
                polarity = reg.Polarity;
                angle = reg.BottomLine.Angle;
                follow = this.FollowSeekLoc(reg.LocR);
                vector.X = (dmtxSymbolSquareAuto == DmtxSymbolSize.DmtxSymbolSquareAuto) ? 0.7 : 0.9;
                vector.Y = 0.8;
            }
            vector *= reg.Fit2Raw;
            loc.X = (int)(vector.X + 0.5);
            loc.Y = (int)(vector.Y + 0.5);
            DmtxPixelLoc loc3 = follow.Loc;
            DmtxBresLine line = new DmtxBresLine(loc3, loc, locInside);
            int tripSteps = this.TrailBlazeGapped(reg, line, polarity);
            DmtxBestLine line2 = this.FindBestSolidLine2(loc3, tripSteps, polarity, angle);
            if (edgeLoc == DmtxEdge.DmtxEdgeTop)
            {
                reg.TopKnown = 1;
                reg.TopAngle = line2.Angle;
                reg.TopLoc = line2.LocBeg;
            }
            else
            {
                reg.RightKnown = 1;
                reg.RightAngle = line2.Angle;
                reg.RightLoc = line2.LocBeg;
            }
            return true;
        }

        private bool MatrixRegionFindSize(DmtxRegion reg)
        {
            DmtxSymbolSize size;
            DmtxSymbolSize dmtxSymbolSquareCount;
            int num;
            DmtxSymbolSize dmtxSymbolShapeAuto = DmtxSymbolSize.DmtxSymbolShapeAuto;
            int num2 = 0;
            int num3 = num = 0;
            if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolShapeAuto)
            {
                size = DmtxSymbolSize.DmtxSymbol10x10;
                dmtxSymbolSquareCount = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
            }
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto)
            {
                size = DmtxSymbolSize.DmtxSymbol10x10;
                dmtxSymbolSquareCount = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
            }
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto)
            {
                size = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
                dmtxSymbolSquareCount = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
            }
            else
            {
                size = this._sizeIdxExpected;
                dmtxSymbolSquareCount = this._sizeIdxExpected + 1;
            }
            for (DmtxSymbolSize size3 = size; size3 < dmtxSymbolSquareCount; size3 += 1)
            {
                int num6;
                int num10;
                int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, size3);
                int num5 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, size3);
                int num7 = num6 = 0;
                int symbolRow = symbolAttribute - 1;
                int symbolCol = 0;
                while (symbolCol < num5)
                {
                    num10 = this.ReadModuleColor(reg, symbolRow, symbolCol, size3, reg.FlowBegin.Plane);
                    if ((symbolCol & 1) != 0)
                    {
                        num6 += num10;
                    }
                    else
                    {
                        num7 += num10;
                    }
                    symbolCol++;
                }
                symbolCol = num5 - 1;
                for (symbolRow = 0; symbolRow < symbolAttribute; symbolRow++)
                {
                    num10 = this.ReadModuleColor(reg, symbolRow, symbolCol, size3, reg.FlowBegin.Plane);
                    if ((symbolRow & 1) != 0)
                    {
                        num6 += num10;
                    }
                    else
                    {
                        num7 += num10;
                    }
                }
                num7 = (num7 * 2) / (symbolAttribute + num5);
                num6 = (num6 * 2) / (symbolAttribute + num5);
                int num11 = Math.Abs((int)(num7 - num6));
                if ((num11 >= 20) && (num11 > num2))
                {
                    num2 = num11;
                    dmtxSymbolShapeAuto = size3;
                    num3 = num7;
                    num = num6;
                }
            }
            if ((dmtxSymbolShapeAuto == DmtxSymbolSize.DmtxSymbolShapeAuto) || (num2 < 20))
            {
                return false;
            }
            reg.SizeIdx = dmtxSymbolShapeAuto;
            reg.OnColor = num3;
            reg.OffColor = num;
            reg.SymbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, reg.SizeIdx);
            reg.SymbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, reg.SizeIdx);
            reg.MappingRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, reg.SizeIdx);
            reg.MappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, reg.SizeIdx);
            int num12 = this.CountJumpTally(reg, 0, reg.SymbolRows - 1, DmtxDirection.DmtxDirRight);
            int num13 = Math.Abs((int)((1 + num12) - reg.SymbolCols));
            if ((num12 < 0) || (num13 > 2))
            {
                return false;
            }
            num12 = this.CountJumpTally(reg, reg.SymbolCols - 1, 0, DmtxDirection.DmtxDirUp);
            num13 = Math.Abs((int)((1 + num12) - reg.SymbolRows));
            if ((num12 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, 0, 0, DmtxDirection.DmtxDirRight);
            if ((num12 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, 0, 0, DmtxDirection.DmtxDirUp);
            if ((num13 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, 0, -1, DmtxDirection.DmtxDirRight);
            if ((num13 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, -1, 0, DmtxDirection.DmtxDirUp);
            if ((num13 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, 0, reg.SymbolRows, DmtxDirection.DmtxDirRight);
            if ((num13 < 0) || (num13 > 2))
            {
                return false;
            }
            num13 = this.CountJumpTally(reg, reg.SymbolCols, 0, DmtxDirection.DmtxDirUp);
            if ((num13 < 0) || (num13 > 2))
            {
                return false;
            }
            return true;
        }

        private bool MatrixRegionOrientation(DmtxRegion reg, DmtxPointFlow begin)
        {
            int num;
            DmtxSymbolSize dmtxSymbolSquareAuto;
            int dmtxUndefined;
            DmtxBestLine line;
            if ((this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto) || ((this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol10x10) && (this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol144x144)))
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolSquareAuto;
            }
            else if ((this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto) || ((this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol8x18) && (this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol16x48)))
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolRectAuto;
            }
            else
            {
                dmtxSymbolSquareAuto = DmtxSymbolSize.DmtxSymbolShapeAuto;
            }
            if (this._edgeMax != DmtxConstants.DmtxUndefined)
            {
                if (dmtxSymbolSquareAuto == DmtxSymbolSize.DmtxSymbolRectAuto)
                {
                    dmtxUndefined = (int)((1.23 * this._edgeMax) + 0.5);
                }
                else
                {
                    dmtxUndefined = (int)((1.56 * this._edgeMax) + 0.5);
                }
            }
            else
            {
                dmtxUndefined = DmtxConstants.DmtxUndefined;
            }
            if (!(this.TrailBlazeContinuous(reg, begin, dmtxUndefined) && (reg.StepsTotal >= 40)))
            {
                this.TrailClear(reg, 0x40);
                return false;
            }
            if (this._edgeMin != DmtxConstants.DmtxUndefined)
            {
                int num4;
                int num3 = this._scale;
                if (dmtxSymbolSquareAuto == DmtxSymbolSize.DmtxSymbolSquareAuto)
                {
                    num4 = (this._edgeMin * this._edgeMin) / (num3 * num3);
                }
                else
                {
                    num4 = ((2 * this._edgeMin) * this._edgeMin) / (num3 * num3);
                }
                if (((reg.BoundMax.X - reg.BoundMin.X) * (reg.BoundMax.Y - reg.BoundMin.Y)) < num4)
                {
                    this.TrailClear(reg, 0x40);
                    return false;
                }
            }
            DmtxBestLine line2 = this.FindBestSolidLine(reg, 0, 0, 1, DmtxConstants.DmtxUndefined);
            if (line2.Mag < 5)
            {
                this.TrailClear(reg, 0x40);
                return false;
            }
            this.FindTravelLimits(reg, ref line2);
            if ((line2.DistSq < 100) || ((line2.Devn * 10.0) >= Math.Sqrt((double)line2.DistSq)))
            {
                this.TrailClear(reg, 0x40);
                return false;
            }
            if (line2.StepPos < line2.StepNeg)
            {
                throw new Exception("Error calculating matrix region orientation");
            }
            DmtxFollow follow = this.FollowSeek(reg, line2.StepPos + 5);
            DmtxBestLine line3 = this.FindBestSolidLine(reg, follow.Step, line2.StepNeg, 1, line2.Angle);
            follow = this.FollowSeek(reg, line2.StepNeg - 5);
            DmtxBestLine line4 = this.FindBestSolidLine(reg, follow.Step, line2.StepPos, -1, line2.Angle);
            if (DmtxCommon.Max<int>(line3.Mag, line4.Mag) < 5)
            {
                return false;
            }
            if (line3.Mag > line4.Mag)
            {
                line = line3;
                this.FindTravelLimits(reg, ref line);
                if ((line.DistSq < 100) || ((line.Devn * 10.0) >= Math.Sqrt((double)line.DistSq)))
                {
                    return false;
                }
                num = ((line2.LocPos.X - line2.LocNeg.X) * (line.LocPos.Y - line.LocNeg.Y)) - ((line2.LocPos.Y - line2.LocNeg.Y) * (line.LocPos.X - line.LocNeg.X));
                if (num > 0)
                {
                    reg.Polarity = 1;
                    reg.LocR = line.LocPos;
                    reg.StepR = line.StepPos;
                    reg.LocT = line2.LocNeg;
                    reg.StepT = line2.StepNeg;
                    reg.LeftLoc = line2.LocBeg;
                    reg.LeftAngle = line2.Angle;
                    reg.BottomLoc = line.LocBeg;
                    reg.BottomAngle = line.Angle;
                    reg.LeftLine = line2;
                    reg.BottomLine = line;
                }
                else
                {
                    reg.Polarity = -1;
                    reg.LocR = line2.LocNeg;
                    reg.StepR = line2.StepNeg;
                    reg.LocT = line.LocPos;
                    reg.StepT = line.StepPos;
                    reg.LeftLoc = line.LocBeg;
                    reg.LeftAngle = line.Angle;
                    reg.BottomLoc = line2.LocBeg;
                    reg.BottomAngle = line2.Angle;
                    reg.LeftLine = line;
                    reg.BottomLine = line2;
                }
            }
            else
            {
                line = line4;
                this.FindTravelLimits(reg, ref line);
                if ((line.DistSq < 100) || ((line.Devn / Math.Sqrt((double)line.DistSq)) >= 0.1))
                {
                    return false;
                }
                num = ((line2.LocNeg.X - line2.LocPos.X) * (line.LocNeg.Y - line.LocPos.Y)) - ((line2.LocNeg.Y - line2.LocPos.Y) * (line.LocNeg.X - line.LocPos.X));
                if (num > 0)
                {
                    reg.Polarity = -1;
                    reg.LocR = line.LocNeg;
                    reg.StepR = line.StepNeg;
                    reg.LocT = line2.LocPos;
                    reg.StepT = line2.StepPos;
                    reg.LeftLoc = line2.LocBeg;
                    reg.LeftAngle = line2.Angle;
                    reg.BottomLoc = line.LocBeg;
                    reg.BottomAngle = line.Angle;
                    reg.LeftLine = line2;
                    reg.BottomLine = line;
                }
                else
                {
                    reg.Polarity = 1;
                    reg.LocR = line2.LocPos;
                    reg.StepR = line2.StepPos;
                    reg.LocT = line.LocNeg;
                    reg.StepT = line.StepNeg;
                    reg.LeftLoc = line.LocBeg;
                    reg.LeftAngle = line.Angle;
                    reg.BottomLoc = line2.LocBeg;
                    reg.BottomAngle = line2.Angle;
                    reg.LeftLine = line;
                    reg.BottomLine = line2;
                }
            }
            reg.LeftKnown = reg.BottomKnown = 1;
            return true;
        }

        private DmtxPointFlow MatrixRegionSeekEdge(DmtxPixelLoc loc)
        {
            DmtxPointFlow[] flowArray = new DmtxPointFlow[3];
            int channelCount = this._image.ChannelCount;
            int index = 0;
            for (int i = 0; i < channelCount; i++)
            {
                flowArray[i] = this.GetPointFlow(i, loc, DmtxConstants.DmtxNeighborNone);
                if ((i > 0) && (flowArray[i].Mag > flowArray[index].Mag))
                {
                    index = i;
                }
            }
            if (flowArray[index].Mag >= 10)
            {
                DmtxPointFlow center = flowArray[index];
                DmtxPointFlow flow2 = this.FindStrongestNeighbor(center, 1);
                DmtxPointFlow flow3 = this.FindStrongestNeighbor(center, -1);
                if ((flow2.Mag != 0) && (flow3.Mag != 0))
                {
                    DmtxPointFlow flow4 = this.FindStrongestNeighbor(flow2, -1);
                    DmtxPointFlow flow5 = this.FindStrongestNeighbor(flow3, 1);
                    if ((flow2.Arrive == ((flow4.Arrive + 4) % 8)) && (flow3.Arrive == ((flow5.Arrive + 4) % 8)))
                    {
                        center.Arrive = DmtxConstants.DmtxNeighborNone;
                        return center;
                    }
                }
            }
            return DmtxConstants.DmtxBlankEdge;
        }

        internal static int ModulePlacementEcc200(byte[] modules, byte[] codewords, DmtxSymbolSize sizeIdx, int moduleOnColor)
        {
            if ((moduleOnColor & ((DmtxConstants.DmtxModuleOnRed | DmtxConstants.DmtxModuleOnGreen) | DmtxConstants.DmtxModuleOnBlue)) == 0)
            {
                throw new Exception("Error with module placement ECC 200");
            }
            int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, sizeIdx);
            int mappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);
            int num3 = 0;
            int row = 4;
            int col = 0;
            do
            {
                if ((row == symbolAttribute) && (col == 0))
                {
                    PatternShapeSpecial1(modules, symbolAttribute, mappingCols, codewords, num3++, moduleOnColor);
                }
                else if (((row == (symbolAttribute - 2)) && (col == 0)) && ((mappingCols % 4) != 0))
                {
                    PatternShapeSpecial2(modules, symbolAttribute, mappingCols, codewords, num3++, moduleOnColor);
                }
                else if (((row == (symbolAttribute - 2)) && (col == 0)) && ((mappingCols % 8) == 4))
                {
                    PatternShapeSpecial3(modules, symbolAttribute, mappingCols, codewords, num3++, moduleOnColor);
                }
                else if (((row == (symbolAttribute + 4)) && (col == 2)) && ((mappingCols % 8) == 0))
                {
                    PatternShapeSpecial4(modules, symbolAttribute, mappingCols, codewords, num3++, moduleOnColor);
                }
                do
                {
                    if (((row < symbolAttribute) && (col >= 0)) && ((modules[(row * mappingCols) + col] & DmtxConstants.DmtxModuleVisited) == 0))
                    {
                        PatternShapeStandard(modules, symbolAttribute, mappingCols, row, col, codewords, num3++, moduleOnColor);
                    }
                    row -= 2;
                    col += 2;
                }
                while ((row >= 0) && (col < mappingCols));
                row++;
                col += 3;
                do
                {
                    if (((row >= 0) && (col < mappingCols)) && ((modules[(row * mappingCols) + col] & DmtxConstants.DmtxModuleVisited) == 0))
                    {
                        PatternShapeStandard(modules, symbolAttribute, mappingCols, row, col, codewords, num3++, moduleOnColor);
                    }
                    row += 2;
                    col -= 2;
                }
                while ((row < symbolAttribute) && (col >= 0));
                row += 3;
                col++;
            }
            while ((row < symbolAttribute) || (col < mappingCols));
            if ((modules[(symbolAttribute * mappingCols) - 1] & DmtxConstants.DmtxModuleVisited) == 0)
            {
                modules[(symbolAttribute * mappingCols) - 1] = (byte)(modules[(symbolAttribute * mappingCols) - 1] | ((byte)moduleOnColor));
                modules[((symbolAttribute * mappingCols) - mappingCols) - 2] = (byte)(modules[((symbolAttribute * mappingCols) - mappingCols) - 2] | ((byte)moduleOnColor));
            }
            return num3;
        }

        internal DmtxMessage MosaicRegion(DmtxRegion reg, int fix)
        {
            int num2;
            int plane = reg.FlowBegin.Plane;
            reg.FlowBegin.Plane = 0;
            DmtxMessage message = this.MatrixRegion(reg, fix);
            reg.FlowBegin.Plane = 1;
            DmtxMessage message2 = this.MatrixRegion(reg, fix);
            reg.FlowBegin.Plane = 2;
            DmtxMessage message3 = this.MatrixRegion(reg, fix);
            reg.FlowBegin.Plane = plane;
            DmtxMessage message4 = new DmtxMessage(reg.SizeIdx, DmtxFormat.Mosaic);
            List<byte> list = new List<byte>();
            for (num2 = 0; num2 < message3.OutputSize; num2++)
            {
                if (message3.Output[num2] == 0)
                {
                    break;
                }
                list.Add(message3.Output[num2]);
            }
            for (num2 = 0; num2 < message2.OutputSize; num2++)
            {
                if (message2.Output[num2] == 0)
                {
                    break;
                }
                list.Add(message2.Output[num2]);
            }
            for (num2 = 0; num2 < message.OutputSize; num2++)
            {
                if (message.Output[num2] == 0)
                {
                    break;
                }
                list.Add(message.Output[num2]);
            }
            list.Add(0);
            message4.Output = list.ToArray();
            return message4;
        }

        internal static void PatternShapeSpecial1(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 2, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 3, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial2(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 3, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 2, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 4, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial3(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 3, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 2, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 2, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 3, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial4(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeStandard(byte[] modules, int mappingRows, int mappingCols, int row, int col, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, row - 2, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 2, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PlaceModule(byte[] modules, int mappingRows, int mappingCols, int row, int col, byte[] codeword, int codeWordIndex, DmtxMaskBit mask, int moduleOnColor)
        {
            if (row < 0)
            {
                row += mappingRows;
                col += 4 - ((mappingRows + 4) % 8);
            }
            if (col < 0)
            {
                col += mappingCols;
                row += 4 - ((mappingCols + 4) % 8);
            }
            if ((modules[(row * mappingCols) + col] & DmtxConstants.DmtxModuleAssigned) != 0)
            {
                if ((modules[(row * mappingCols) + col] & moduleOnColor) != 0)
                {
                    codeword[codeWordIndex] = (byte)(codeword[codeWordIndex] | ((byte)mask));
                }
                else
                {
                    codeword[codeWordIndex] = (byte)(codeword[codeWordIndex] & ((byte)((DmtxMaskBit.DmtxMaskBit1 | DmtxMaskBit.DmtxMaskBit2 | DmtxMaskBit.DmtxMaskBit3 | DmtxMaskBit.DmtxMaskBit4 | DmtxMaskBit.DmtxMaskBit5 | DmtxMaskBit.DmtxMaskBit6 | DmtxMaskBit.DmtxMaskBit7 | DmtxMaskBit.DmtxMaskBit8) ^ mask)));
                }
            }
            else
            {
                if ((codeword[codeWordIndex] & ((byte)mask)) != 0)
                {
                    modules[(row * mappingCols) + col] = (byte)(modules[(row * mappingCols) + col] | ((byte)moduleOnColor));
                }
                modules[(row * mappingCols) + col] = (byte)(modules[(row * mappingCols) + col] | ((byte)DmtxConstants.DmtxModuleAssigned));
            }
            modules[(row * mappingCols) + col] = (byte)(modules[(row * mappingCols) + col] | ((byte)DmtxConstants.DmtxModuleVisited));
        }

        private bool PopulateArrayFromMatrix(DmtxRegion reg, DmtxMessage msg)
        {
            int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribHorizDataRegions, reg.SizeIdx);
            int num2 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribVertDataRegions, reg.SizeIdx);
            int mapWidth = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionCols, reg.SizeIdx);
            int mapHeight = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionRows, reg.SizeIdx);
            int num5 = 2 * ((mapHeight + mapWidth) + 2);
            if (num5 <= 0)
            {
                throw new ArgumentException("PopulateArrayFromMatrix error: Weight Factor must be greater 0");
            }
            for (int i = 0; i < num2; i++)
            {
                int yOrigin = (i * (mapHeight + 2)) + 1;
                for (int j = 0; j < symbolAttribute; j++)
                {
                    int[,] tally = new int[0x18, 0x18];
                    int xOrigin = (j * (mapWidth + 2)) + 1;
                    for (int k = 0; k < 0x18; k++)
                    {
                        for (int n = 0; n < 0x18; n++)
                        {
                            tally[k, n] = 0;
                        }
                    }
                    this.TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirUp);
                    this.TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirLeft);
                    this.TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirDown);
                    this.TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirRight);
                    for (int m = 0; m < mapHeight; m++)
                    {
                        for (int num13 = 0; num13 < mapWidth; num13++)
                        {
                            int num14 = (i * mapHeight) + m;
                            num14 = ((num2 * mapHeight) - num14) - 1;
                            int num15 = (j * mapWidth) + num13;
                            int index = ((num14 * symbolAttribute) * mapWidth) + num15;
                            if ((((double)tally[m, num13]) / ((double)num5)) >= 0.5)
                            {
                                msg.Array[index] = (byte)DmtxConstants.DmtxModuleOnRGB;
                            }
                            else
                            {
                                msg.Array[index] = (byte)DmtxConstants.DmtxModuleOff;
                            }
                            msg.Array[index] = (byte)(msg.Array[index] | ((byte)DmtxConstants.DmtxModuleAssigned));
                        }
                    }
                }
            }
            return true;
        }

        private int ReadModuleColor(DmtxRegion reg, int symbolRow, int symbolCol, DmtxSymbolSize sizeIdx, int colorPlane)
        {
            int num2;
            double[] numArray = new double[] { 0.5, 0.4, 0.5, 0.6, 0.5 };
            double[] numArray2 = new double[] { 0.5, 0.5, 0.4, 0.5, 0.6 };
            DmtxVector2 vector = new DmtxVector2();
            int symbolAttribute = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdx);
            int num4 = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, sizeIdx);
            int num5 = num2 = 0;
            for (int i = 0; i < 5; i++)
            {
                vector.X = (1.0 / ((double)num4)) * (symbolCol + numArray[i]);
                vector.Y = (1.0 / ((double)symbolAttribute)) * (symbolRow + numArray2[i]);
                vector *= reg.Fit2Raw;
                this.GetPixelValue((int)(vector.X + 0.5), (int)(vector.Y + 0.5), colorPlane, ref num5);
                num2 += num5;
            }
            return (num2 / 5);
        }

        internal DmtxRegion RegionFindNext(TimeSpan timeout)
        {
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DateTime now = DateTime.Now;
            do
            {
                if (this._grid.PopGridLocation(ref loc) == DmtxRange.DmtxRangeEnd)
                {
                    break;
                }
                DmtxRegion region = this.RegionScanPixel(loc.X, loc.Y);
                if (region != null)
                {
                    return region;
                }
            }
            while ((DateTime.Now - now) <= timeout);
            return null;
        }

        private DmtxRegion RegionScanPixel(int x, int y)
        {
            DmtxRegion reg = new DmtxRegion();
            DmtxPixelLoc loc = new DmtxPixelLoc
            {
                X = x,
                Y = y
            };
            int index = this.DecodeGetCache(loc.X, loc.Y);
            if (index == -1)
            {
                return null;
            }
            if (this._cache[index] != 0)
            {
                return null;
            }
            DmtxPointFlow begin = this.MatrixRegionSeekEdge(loc);
            if (begin.Mag < ((int)((this._edgeThresh * 7.65) + 0.5)))
            {
                return null;
            }
            if (!this.MatrixRegionOrientation(reg, begin))
            {
                return null;
            }
            if (!this.RegionUpdateXfrms(reg))
            {
                return null;
            }
            if (!this.MatrixRegionAlignCalibEdge(reg, DmtxEdge.DmtxEdgeTop))
            {
                return null;
            }
            if (!this.RegionUpdateXfrms(reg))
            {
                return null;
            }
            if (!this.MatrixRegionAlignCalibEdge(reg, DmtxEdge.DmtxEdgeRight))
            {
                return null;
            }
            if (!this.RegionUpdateXfrms(reg))
            {
                return null;
            }
            if (!this.MatrixRegionFindSize(reg))
            {
                return null;
            }
            return new DmtxRegion(reg);
        }

        private bool RegionUpdateCorners(DmtxRegion reg, DmtxVector2 p00, DmtxVector2 p10, DmtxVector2 p11, DmtxVector2 p01)
        {
            double num = this.Width - 1;
            double num2 = this.Height - 1;
            if ((((((p00.X < 0.0) || (p00.Y < 0.0)) || ((p00.X > num) || (p00.Y > num2))) || (((p01.X < 0.0) || (p01.Y < 0.0)) || ((p01.X > num) || (p01.Y > num2)))) || (((p10.X < 0.0) || (p10.Y < 0.0)) || (p10.X > num))) || (p10.Y > num2))
            {
                return false;
            }
            DmtxVector2 vector = p01 - p00;
            DmtxVector2 vector2 = p10 - p00;
            DmtxVector2 vector3 = p11 - p01;
            DmtxVector2 vector4 = p11 - p10;
            double num3 = vector.Mag();
            double num4 = vector2.Mag();
            double num5 = vector3.Mag();
            double num6 = vector4.Mag();
            if ((((num3 <= 8.0) || (num4 <= 8.0)) || (num5 <= 8.0)) || (num6 <= 8.0))
            {
                return false;
            }
            double num7 = num3 / num6;
            if ((num7 <= 0.5) || (num7 >= 2.0))
            {
                return false;
            }
            num7 = num4 / num5;
            if ((num7 <= 0.5) || (num7 >= 2.0))
            {
                return false;
            }
            if ((vector2.Cross(vector4) <= 0.0) || (vector.Cross(vector3) >= 0.0))
            {
                return false;
            }
            if (DmtxCommon.RightAngleTrueness(p00, p10, p11, 1.5707963267948966) <= this._squareDevn)
            {
                return false;
            }
            if (DmtxCommon.RightAngleTrueness(p10, p11, p01, 1.5707963267948966) <= this._squareDevn)
            {
                return false;
            }
            double tx = -1.0 * p00.X;
            double ty = -1.0 * p00.Y;
            DmtxMatrix3 matrix = DmtxMatrix3.Translate(tx, ty);
            double angle = Math.Atan2(vector.X, vector.Y);
            DmtxMatrix3 matrix2 = DmtxMatrix3.Rotate(angle);
            DmtxMatrix3 matrix3 = matrix * matrix2;
            DmtxVector2 vector5 = p10 * matrix3;
            double shy = -vector5.Y / vector5.X;
            DmtxMatrix3 matrix4 = DmtxMatrix3.Shear(0.0, shy);
            matrix3 *= matrix4;
            double sx = 1.0 / vector5.X;
            DmtxMatrix3 matrix5 = DmtxMatrix3.Scale(sx, 1.0);
            matrix3 *= matrix5;
            vector5 = p11 * matrix3;
            double sy = 1.0 / vector5.Y;
            DmtxMatrix3 matrix6 = DmtxMatrix3.Scale(1.0, sy);
            matrix3 *= matrix6;
            vector5 = p11 * matrix3;
            double x = vector5.X;
            DmtxMatrix3 matrix7 = DmtxMatrix3.LineSkewSide(1.0, x, 1.0);
            matrix3 *= matrix7;
            vector5 = p01 * matrix3;
            double y = vector5.Y;
            DmtxMatrix3 matrix8 = DmtxMatrix3.LineSkewTop(y, 1.0, 1.0);
            reg.Raw2Fit = matrix3 * matrix8;
            matrix8 = DmtxMatrix3.LineSkewTopInv(y, 1.0, 1.0);
            matrix7 = DmtxMatrix3.LineSkewSideInv(1.0, x, 1.0);
            matrix3 = matrix8 * matrix7;
            DmtxMatrix3 matrix9 = DmtxMatrix3.Scale(1.0 / sx, 1.0 / sy);
            matrix3 *= matrix9;
            matrix4 = DmtxMatrix3.Shear(0.0, -shy);
            matrix3 *= matrix4;
            matrix2 = DmtxMatrix3.Rotate(-angle);
            matrix3 *= matrix2;
            matrix = DmtxMatrix3.Translate(-tx, -ty);
            reg.Fit2Raw = matrix3 * matrix;
            return true;
        }

        private bool RegionUpdateXfrms(DmtxRegion reg)
        {
            DmtxRay2 ray = new DmtxRay2();
            DmtxRay2 ray2 = new DmtxRay2();
            DmtxRay2 ray3 = new DmtxRay2();
            DmtxRay2 ray4 = new DmtxRay2();
            DmtxVector2 vector = new DmtxVector2();
            DmtxVector2 vector2 = new DmtxVector2();
            DmtxVector2 vector3 = new DmtxVector2();
            DmtxVector2 vector4 = new DmtxVector2();
            if ((reg.LeftKnown == 0) || (reg.BottomKnown == 0))
            {
                throw new ArgumentException("Error updating Xfrms!");
            }
            ray.P.X = reg.LeftLoc.X;
            ray.P.Y = reg.LeftLoc.Y;
            double d = reg.LeftAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
            ray.V.X = Math.Cos(d);
            ray.V.Y = Math.Sin(d);
            ray.TMin = 0.0;
            ray.TMax = ray.V.Norm();
            ray2.P.X = reg.BottomLoc.X;
            ray2.P.Y = reg.BottomLoc.Y;
            d = reg.BottomAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
            ray2.V.X = Math.Cos(d);
            ray2.V.Y = Math.Sin(d);
            ray2.TMin = 0.0;
            ray2.TMax = ray2.V.Norm();
            if (reg.TopKnown != 0)
            {
                ray3.P.X = reg.TopLoc.X;
                ray3.P.Y = reg.TopLoc.Y;
                d = reg.TopAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
                ray3.V.X = Math.Cos(d);
                ray3.V.Y = Math.Sin(d);
                ray3.TMin = 0.0;
                ray3.TMax = ray3.V.Norm();
            }
            else
            {
                ray3.P.X = reg.LocT.X;
                ray3.P.Y = reg.LocT.Y;
                d = reg.BottomAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
                ray3.V.X = Math.Cos(d);
                ray3.V.Y = Math.Sin(d);
                ray3.TMin = 0.0;
                ray3.TMax = ray2.TMax;
            }
            if (reg.RightKnown != 0)
            {
                ray4.P.X = reg.RightLoc.X;
                ray4.P.Y = reg.RightLoc.Y;
                d = reg.RightAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
                ray4.V.X = Math.Cos(d);
                ray4.V.Y = Math.Sin(d);
                ray4.TMin = 0.0;
                ray4.TMax = ray4.V.Norm();
            }
            else
            {
                ray4.P.X = reg.LocR.X;
                ray4.P.Y = reg.LocR.Y;
                d = reg.LeftAngle * (3.1415926535897931 / ((double)DmtxConstants.DmtxHoughRes));
                ray4.V.X = Math.Cos(d);
                ray4.V.Y = Math.Sin(d);
                ray4.TMin = 0.0;
                ray4.TMax = ray.TMax;
            }
            if (!vector.Intersect(ray, ray2))
            {
                return false;
            }
            if (!vector2.Intersect(ray2, ray4))
            {
                return false;
            }
            if (!vector3.Intersect(ray4, ray3))
            {
                return false;
            }
            if (!vector4.Intersect(ray3, ray))
            {
                return false;
            }
            if (!this.RegionUpdateCorners(reg, vector, vector2, vector3, vector4))
            {
                return false;
            }
            return true;
        }

        private void TallyModuleJumps(DmtxRegion reg, int[,] tally, int xOrigin, int yOrigin, int mapWidth, int mapHeight, DmtxDirection dir)
        {
            if (dir != DmtxDirection.DmtxDirUp && dir != DmtxDirection.DmtxDirLeft && dir != DmtxDirection.DmtxDirDown && dir != DmtxDirection.DmtxDirRight)
            {
                throw new ArgumentException("Only orthogonal directions are allowed in tally module jumps!");
            }
            int num = (dir == DmtxDirection.DmtxDirUp || dir == DmtxDirection.DmtxDirRight) ? 1 : -1;
            bool flag = false;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            if ((dir & DmtxDirection.DmtxDirHorizontal) != DmtxDirection.DmtxDirNone)
            {
                flag = true;
                num2 = mapWidth;
                num3 = yOrigin;
                num4 = yOrigin + mapHeight;
                num5 = ((num == 1) ? (xOrigin - 1) : (xOrigin + mapWidth));
                num6 = ((num == 1) ? (xOrigin + mapWidth) : (xOrigin - 1));
            }
            else
            {
                num2 = mapHeight;
                num3 = xOrigin;
                num4 = xOrigin + mapWidth;
                num5 = ((num == 1) ? (yOrigin - 1) : (yOrigin + mapHeight));
                num6 = ((num == 1) ? (yOrigin + mapHeight) : (yOrigin - 1));
            }
            bool flag2 = reg.OffColor > reg.OnColor;
            int num7 = Math.Abs((int)(0.4 * (double)(reg.OffColor - reg.OnColor) + 0.5));
            if (num7 < 0)
            {
                throw new Exception("Negative jump threshold is not allowed in tally module jumps");
            }
            for (int i = num3; i < num4; i++)
            {
                int num8 = num5;
                int num9 = flag ? this.ReadModuleColor(reg, i, num8, reg.SizeIdx, reg.FlowBegin.Plane) : this.ReadModuleColor(reg, num8, i, reg.SizeIdx, reg.FlowBegin.Plane);
                int num10 = flag2 ? (reg.OffColor - num9) : (num9 - reg.OffColor);
                int num11 = (num == 1 || (i & 1) == 0) ? DmtxConstants.DmtxModuleOnRGB : DmtxConstants.DmtxModuleOff;
                int num12 = num2;
                while ((num8 += num) != num6)
                {
                    int num13 = num10;
                    int num14 = num11;
                    num9 = (flag ? this.ReadModuleColor(reg, i, num8, reg.SizeIdx, reg.FlowBegin.Plane) : this.ReadModuleColor(reg, num8, i, reg.SizeIdx, reg.FlowBegin.Plane));
                    num10 = (flag2 ? (reg.OffColor - num9) : (num9 - reg.OffColor));
                    if (num14 == DmtxConstants.DmtxModuleOnRGB)
                    {
                        num11 = ((num10 < num13 - num7) ? DmtxConstants.DmtxModuleOff : DmtxConstants.DmtxModuleOnRGB);
                    }
                    else if (num14 == DmtxConstants.DmtxModuleOff)
                    {
                        num11 = ((num10 > num13 + num7) ? DmtxConstants.DmtxModuleOnRGB : DmtxConstants.DmtxModuleOff);
                    }
                    int num15;
                    int num16;
                    if (flag)
                    {
                        num15 = i - yOrigin;
                        num16 = num8 - xOrigin;
                    }
                    else
                    {
                        num15 = num8 - yOrigin;
                        num16 = i - xOrigin;
                    }
                    if (num15 >= 24 || num16 >= 24)
                    {
                        throw new Exception("Tally module mump failed, index out of range!");
                    }
                    if (num11 == DmtxConstants.DmtxModuleOnRGB)
                    {
                        tally[num15, num16] += 2 * num12;
                    }
                    num12--;
                }
                if (num12 != 0)
                {
                    throw new Exception("Tally module jump failed, weight <> 0!");
                }
            }
        }

        private bool TrailBlazeContinuous(DmtxRegion reg, DmtxPointFlow flowBegin, int maxDiagonal)
        {
            int num;
            DmtxPixelLoc loc;
            DmtxPixelLoc loc2 = loc = flowBegin.Loc;
            int index = this.DecodeGetCache(flowBegin.Loc.X, flowBegin.Loc.Y);
            this._cache[index] = 0xc0;
            reg.FlowBegin = flowBegin;
            int num4 = num = 0;
            for (int i = 1; i >= -1; i -= 2)
            {
                bool flag2;
                DmtxPointFlow center = flowBegin;
                int num5 = index;
                int num6 = 0;
                goto Label_02A1;
            Label_0062:
                if ((maxDiagonal != DmtxConstants.DmtxUndefined) && (((loc.X - loc2.X) > maxDiagonal) || ((loc.Y - loc2.Y) > maxDiagonal)))
                {
                    goto Label_02A9;
                }
                DmtxPointFlow flow2 = this.FindStrongestNeighbor(center, i);
                if (flow2.Mag < 50)
                {
                    goto Label_02A9;
                }
                int num7 = this.DecodeGetCache(flow2.Loc.X, flow2.Loc.Y);
                if ((this._cache[num7] & 0x80) != 0)
                {
                    throw new Exception("Error creating Trail Blaze");
                }
                this._cache[num5] = (byte)(this._cache[num5] | ((i < 0) ? ((byte)flow2.Arrive) : ((byte)(flow2.Arrive << 3))));
                this._cache[num7] = (i < 0) ? ((byte)(((flow2.Arrive + 4) % 8) << 3)) : ((byte)((flow2.Arrive + 4) % 8));
                this._cache[num7] = (byte)(this._cache[num7] | 0xc0);
                if (i > 0)
                {
                    num4++;
                }
                else
                {
                    num++;
                }
                num5 = num7;
                center = flow2;
                if (center.Loc.X > loc.X)
                {
                    loc.X = center.Loc.X;
                }
                else if (center.Loc.X < loc2.X)
                {
                    loc2.X = center.Loc.X;
                }
                if (center.Loc.Y > loc.Y)
                {
                    loc.Y = center.Loc.Y;
                }
                else if (center.Loc.Y < loc2.Y)
                {
                    loc2.Y = center.Loc.Y;
                }
                num6++;
            Label_02A1:
                flag2 = true;
                goto Label_0062;
            Label_02A9:
                if (i > 0)
                {
                    reg.FinalPos = center.Loc;
                    reg.JumpToNeg = num6;
                }
                else
                {
                    reg.FinalNeg = center.Loc;
                    reg.JumpToPos = num6;
                }
            }
            reg.StepsTotal = reg.JumpToPos + reg.JumpToNeg;
            reg.BoundMin = loc2;
            reg.BoundMax = loc;
            int num8 = this.TrailClear(reg, 0x80);
            if ((num4 + num) != (num8 - 1))
            {
                throw new Exception("Error cleaning after trail blaze continuous");
            }
            if ((maxDiagonal != DmtxConstants.DmtxUndefined) && (((loc.X - loc2.X) > maxDiagonal) || ((loc.Y - loc2.Y) > maxDiagonal)))
            {
                return false;
            }
            return true;
        }

        private int TrailBlazeGapped(DmtxRegion reg, DmtxBresLine line, int streamDir)
        {
            int num;
            int travel = 0;
            int outward = 0;
            int[] numArray = new int[] { 0, 1, 2, 7, 8, 3, 6, 5, 4 };
            DmtxPixelLoc loc = line.Loc;
            DmtxPointFlow center = this.GetPointFlow(reg.FlowBegin.Plane, loc, DmtxConstants.DmtxNeighborNone);
            int num4 = (line.XDelta * line.XDelta) + (line.YDelta * line.YDelta);
            int num5 = 0;
            bool flag = true;
            DmtxPixelLoc loc2 = loc;
            int index = this.DecodeGetCache(loc.X, loc.Y);
            if (index == -1)
            {
                return 0;
            }
            this._cache[index] = 0;
            do
            {
                if (flag)
                {
                    DmtxPointFlow flow2 = this.FindStrongestNeighbor(center, streamDir);
                    if (flow2.Mag == DmtxConstants.DmtxUndefined)
                    {
                        return num5;
                    }
                    new DmtxBresLine(line).GetStep(flow2.Loc, ref travel, ref outward);
                    if (((flow2.Mag < 50) || (outward < 0)) || ((outward == 0) && (travel < 0)))
                    {
                        flag = false;
                    }
                    else
                    {
                        line.Step(travel, outward);
                        center = flow2;
                    }
                }
                if (!flag)
                {
                    line.Step(1, 0);
                    center = this.GetPointFlow(reg.FlowBegin.Plane, line.Loc, DmtxConstants.DmtxNeighborNone);
                    if (center.Mag > 50)
                    {
                        flag = true;
                    }
                }
                DmtxPixelLoc loc3 = line.Loc;
                int num7 = this.DecodeGetCache(loc3.X, loc3.Y);
                if (num7 == -1)
                {
                    return num5;
                }
                int num8 = loc3.X - loc2.X;
                int num9 = loc3.Y - loc2.Y;
                if ((Math.Abs(num8) > 1) || (Math.Abs(num9) > 1))
                {
                    throw new Exception("Invalid step directions!");
                }
                int num10 = numArray[((3 * num9) + num8) + 4];
                if (num10 == 8)
                {
                    throw new Exception("Invalid step direction!");
                }
                if (streamDir < 0)
                {
                    this._cache[index] = (byte)(this._cache[index] | ((byte)(0x40 | num10)));
                    this._cache[num7] = (byte)(((num10 + 4) % 8) << 3);
                }
                else
                {
                    this._cache[index] = (byte)(this._cache[index] | ((byte)(0x40 | (num10 << 3))));
                    this._cache[num7] = (byte)((num10 + 4) % 8);
                }
                int num11 = line.Loc.X - loc.X;
                int num12 = line.Loc.Y - loc.Y;
                num = (num11 * num11) + (num12 * num12);
                loc2 = line.Loc;
                index = num7;
                num5++;
            }
            while (num < num4);
            return num5;
        }

        private int TrailClear(DmtxRegion reg, int clearMask)
        {
            if ((clearMask | 0xff) != 0xff)
            {
                throw new Exception("TrailClear mask is invalid!");
            }
            int num = 0;
            DmtxFollow followBeg = this.FollowSeek(reg, 0);
            while (Math.Abs(followBeg.Step) <= reg.StepsTotal)
            {
                if ((followBeg.CurrentPtr & clearMask) == 0)
                {
                    throw new Exception("Error performing TrailClear");
                }
                followBeg.CurrentPtr = (byte)(followBeg.CurrentPtr & ((byte)(clearMask ^ 0xff)));
                followBeg = this.FollowStep(reg, followBeg, 1);
                num++;
            }
            return num;
        }

        private void ValidateSettingsAndInitScanGrid()
        {
            if ((this._squareDevn <= 0.0) || (this._squareDevn >= 1.0))
            {
                throw new ArgumentException("Invalid decode settings!");
            }
            if (this._scanGap < 1)
            {
                throw new ArgumentException("Invalid decode settings!");
            }
            if ((this._edgeThresh < 1) || (this._edgeThresh > 100))
            {
                throw new ArgumentException("Invalid decode settings!");
            }
            this._grid = new DmtxScanGrid(this);
        }

        internal byte[] Cache
        {
            get
            {
                return this._cache;
            }
            set
            {
                this._cache = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int EdgeMax
        {
            get
            {
                return this._edgeMax;
            }
            set
            {
                this._edgeMax = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int EdgeMin
        {
            get
            {
                return this._edgeMin;
            }
            set
            {
                this._edgeMin = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int EdgeThresh
        {
            get
            {
                return this._edgeThresh;
            }
            set
            {
                this._edgeThresh = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal DmtxScanGrid Grid
        {
            get
            {
                return this._grid;
            }
            set
            {
                this._grid = value;
            }
        }

        internal int Height
        {
            get
            {
                return (this._image.Height / this._scale);
            }
        }

        internal DmtxImage Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int Scale
        {
            get
            {
                return this._scale;
            }
            set
            {
                this._scale = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int ScanGap
        {
            get
            {
                return this._scanGap;
            }
            set
            {
                this._scanGap = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal DmtxSymbolSize SizeIdxExpected
        {
            get
            {
                return this._sizeIdxExpected;
            }
            set
            {
                this._sizeIdxExpected = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int SquareDevn
        {
            get
            {
                return (int)((Math.Acos(this._squareDevn) * 180.0) / 3.1415926535897931);
            }
            set
            {
                this._squareDevn = Math.Cos(value * 0.017453292519943295);
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int Width
        {
            get
            {
                return (this._image.Width / this._scale);
            }
        }

        internal int XMax
        {
            get
            {
                return this._xMax;
            }
            set
            {
                this._xMax = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int XMin
        {
            get
            {
                return this._xMin;
            }
            set
            {
                this._xMin = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int YMax
        {
            get
            {
                return this._yMax;
            }
            set
            {
                this._yMax = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }

        internal int YMin
        {
            get
            {
                return this._yMin;
            }
            set
            {
                this._yMin = value;
                this.ValidateSettingsAndInitScanGrid();
            }
        }
    }
}

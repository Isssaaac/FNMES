using System;

namespace DataMatrix.net
{
	// Token: 0x02000005 RID: 5
	internal class DmtxRegion
	{
		// Token: 0x06000027 RID: 39 RVA: 0x00002AD2 File Offset: 0x00000CD2
		internal DmtxRegion()
		{
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002AE0 File Offset: 0x00000CE0
		internal DmtxRegion(DmtxRegion src)
		{
			this.BottomAngle = src.BottomAngle;
			this.BottomKnown = src.BottomKnown;
			this.BottomLine = src.BottomLine;
			this.BottomLoc = src.BottomLoc;
			this.BoundMax = src.BoundMax;
			this.BoundMin = src.BoundMin;
			this.FinalNeg = src.FinalNeg;
			this.FinalPos = src.FinalPos;
			this.Fit2Raw = new DmtxMatrix3(src.Fit2Raw);
			this.FlowBegin = src.FlowBegin;
			this.JumpToNeg = src.JumpToNeg;
			this.JumpToPos = src.JumpToPos;
			this.LeftAngle = src.LeftAngle;
			this.LeftKnown = src.LeftKnown;
			this.LeftLine = src.LeftLine;
			this.LeftLoc = src.LeftLoc;
			this.LocR = src.LocR;
			this.LocT = src.LocT;
			this.MappingCols = src.MappingCols;
			this.MappingRows = src.MappingRows;
			this.OffColor = src.OffColor;
			this.OnColor = src.OnColor;
			this.Polarity = src.Polarity;
			this.Raw2Fit = new DmtxMatrix3(src.Raw2Fit);
			this.RightAngle = src.RightAngle;
			this.RightKnown = src.RightKnown;
			this.RightLoc = src.RightLoc;
			this.SizeIdx = src.SizeIdx;
			this.StepR = src.StepR;
			this.StepsTotal = src.StepsTotal;
			this.StepT = src.StepT;
			this.SymbolCols = src.SymbolCols;
			this.SymbolRows = src.SymbolRows;
			this.TopAngle = src.TopAngle;
			this.TopKnown = src.TopKnown;
			this.TopLoc = src.TopLoc;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002CD4 File Offset: 0x00000ED4
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002CEB File Offset: 0x00000EEB
		internal int JumpToPos { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002CF4 File Offset: 0x00000EF4
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002D0B File Offset: 0x00000F0B
		internal int JumpToNeg { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002D14 File Offset: 0x00000F14
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002D2B File Offset: 0x00000F2B
		internal int StepsTotal { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002D34 File Offset: 0x00000F34
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002D4B File Offset: 0x00000F4B
		internal DmtxPixelLoc FinalPos { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002D54 File Offset: 0x00000F54
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002D6B File Offset: 0x00000F6B
		internal DmtxPixelLoc FinalNeg { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002D74 File Offset: 0x00000F74
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002D8B File Offset: 0x00000F8B
		internal DmtxPixelLoc BoundMin { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002D94 File Offset: 0x00000F94
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002DAB File Offset: 0x00000FAB
		internal DmtxPixelLoc BoundMax { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002DB4 File Offset: 0x00000FB4
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002DCB File Offset: 0x00000FCB
		internal DmtxPointFlow FlowBegin { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002DD4 File Offset: 0x00000FD4
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002DEB File Offset: 0x00000FEB
		internal int Polarity { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002DF4 File Offset: 0x00000FF4
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002E0B File Offset: 0x0000100B
		internal int StepR { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002E14 File Offset: 0x00001014
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002E2B File Offset: 0x0000102B
		internal int StepT { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002E34 File Offset: 0x00001034
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002E4B File Offset: 0x0000104B
		internal DmtxPixelLoc LocR { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002E54 File Offset: 0x00001054
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002E6B File Offset: 0x0000106B
		internal DmtxPixelLoc LocT { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002E74 File Offset: 0x00001074
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002E8B File Offset: 0x0000108B
		internal int LeftKnown { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002E94 File Offset: 0x00001094
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002EAB File Offset: 0x000010AB
		internal int LeftAngle { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002EB4 File Offset: 0x000010B4
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002ECB File Offset: 0x000010CB
		internal DmtxPixelLoc LeftLoc { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002ED4 File Offset: 0x000010D4
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002EEB File Offset: 0x000010EB
		internal DmtxBestLine LeftLine { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002EF4 File Offset: 0x000010F4
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002F0B File Offset: 0x0000110B
		internal int BottomKnown { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002F14 File Offset: 0x00001114
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002F2B File Offset: 0x0000112B
		internal int BottomAngle { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002F34 File Offset: 0x00001134
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002F4B File Offset: 0x0000114B
		internal DmtxPixelLoc BottomLoc { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002F54 File Offset: 0x00001154
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002F6B File Offset: 0x0000116B
		internal DmtxBestLine BottomLine { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002F74 File Offset: 0x00001174
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00002F8B File Offset: 0x0000118B
		internal int TopKnown { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002F94 File Offset: 0x00001194
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002FAB File Offset: 0x000011AB
		internal int TopAngle { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002FB4 File Offset: 0x000011B4
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002FCB File Offset: 0x000011CB
		internal DmtxPixelLoc TopLoc { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002FD4 File Offset: 0x000011D4
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002FEB File Offset: 0x000011EB
		internal int RightKnown { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002FF4 File Offset: 0x000011F4
		// (set) Token: 0x0600005C RID: 92 RVA: 0x0000300B File Offset: 0x0000120B
		internal int RightAngle { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003014 File Offset: 0x00001214
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000302B File Offset: 0x0000122B
		internal DmtxPixelLoc RightLoc { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003034 File Offset: 0x00001234
		// (set) Token: 0x06000060 RID: 96 RVA: 0x0000304B File Offset: 0x0000124B
		internal int OnColor { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00003054 File Offset: 0x00001254
		// (set) Token: 0x06000062 RID: 98 RVA: 0x0000306B File Offset: 0x0000126B
		internal int OffColor { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00003074 File Offset: 0x00001274
		// (set) Token: 0x06000064 RID: 100 RVA: 0x0000308B File Offset: 0x0000128B
		internal DmtxSymbolSize SizeIdx { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00003094 File Offset: 0x00001294
		// (set) Token: 0x06000066 RID: 102 RVA: 0x000030AB File Offset: 0x000012AB
		internal int SymbolRows { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000030B4 File Offset: 0x000012B4
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000030CB File Offset: 0x000012CB
		internal int SymbolCols { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000030D4 File Offset: 0x000012D4
		// (set) Token: 0x0600006A RID: 106 RVA: 0x000030EB File Offset: 0x000012EB
		internal int MappingRows { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600006B RID: 107 RVA: 0x000030F4 File Offset: 0x000012F4
		// (set) Token: 0x0600006C RID: 108 RVA: 0x0000310B File Offset: 0x0000130B
		internal int MappingCols { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003114 File Offset: 0x00001314
		// (set) Token: 0x0600006E RID: 110 RVA: 0x0000312B File Offset: 0x0000132B
		internal DmtxMatrix3 Raw2Fit { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00003134 File Offset: 0x00001334
		// (set) Token: 0x06000070 RID: 112 RVA: 0x0000314B File Offset: 0x0000134B
		internal DmtxMatrix3 Fit2Raw { get; set; }
	}
}

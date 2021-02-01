using System;

namespace Spyder.Console.Modules
{
	[Serializable]
	public class SegmentConfig : ICloneable
	{
		public int PortID { get; set; }	
		public SubControllerTypes Type { get; set; } = SubControllerTypes.SRCE;

		public int ConfigB { get; set; }
		public int ConfigC { get; set; }
		public uint FlagsA { get; set; }
		public uint ModesA { get; set; }

		public SegmentConfig()
		{
		}
		public SegmentConfig(int portID, SubControllerTypes type, int configB, int configC, uint flagsA)
		{
			this.PortID = portID;
			this.Type = type;
			this.ConfigB = configB;
			this.ConfigC = configC;
			this.FlagsA = flagsA;			
		}
		public SegmentConfig(Segment seg)
		{
			this.PortID = seg.PortID;
			this.Type = (SubControllerTypes)seg.ConfigA;
			this.ConfigB = seg.ConfigB;
			this.ConfigC = seg.ConfigC;
			this.FlagsA = seg.FlagsA;
			this.ModesA = seg.ModesA;
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}
}

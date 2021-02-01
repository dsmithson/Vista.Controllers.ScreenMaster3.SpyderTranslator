using System;

namespace Spyder.Console.Modules
{
	public class SegmentEncoder : Segment
	{
		public SegmentEncoder(int portID) : base(portID, SegmentType.Encoder)
		{
			display.Startup(40, 2);
			this.display.DisplayText = "Encoder";
		}
	}
}

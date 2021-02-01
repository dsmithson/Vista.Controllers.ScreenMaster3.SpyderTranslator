using System;

namespace Spyder.Console.Modules
{
	/// <summary>
	/// Summary description for Segment1x4RD.
	/// </summary>
	public class Segment1x4RD : Segment
	{
		public Segment1x4RD(int portID) : base(portID, SegmentType.Encoder)
		{
			display.Startup(40, 2);
			display.DisplayText = "01234567890123456789012345678901234567890123456789012345678901234567890123456789 ";
		}
	}
}

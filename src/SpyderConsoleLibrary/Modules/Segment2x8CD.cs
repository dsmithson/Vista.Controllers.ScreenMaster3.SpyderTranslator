using System;
using System.Collections;

namespace Spyder.Console.Modules
{
	public class Segment2x8CD : Segment
	{
		public Segment2x8CD(int portID) : base(portID, SegmentType.Segment2x8)
		{
			display.Startup(40, 2);
			this.display.DisplayText = "AAAA BBBB CCCC DDDD EEEE FFFF GGGG HHHH IIII JJJJ KKKK LLLL MMMM NNNN OOOO PPPP ";
		}
	}
}

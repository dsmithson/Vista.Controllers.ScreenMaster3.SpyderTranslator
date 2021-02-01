using System;
using System.Collections;

namespace Spyder.Console.Modules
{
	public class Segment4x4CD : Segment
	{
		public Segment4x4CD(int portID) : base(portID, SegmentType.Segment4x4)
		{
			display.Startup(20, 4);
			this.display.DisplayText = "AAAA BBBB CCCC DDDD EEEE FFFF GGGG HHHH IIII JJJJ KKKK LLLL MMMM NNNN OOOO PPPP ";
		}
    }
}

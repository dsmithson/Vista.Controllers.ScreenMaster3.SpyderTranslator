using System;
using Spyder.Console.Modules;
using System.Text;
using System.Collections.Generic;

namespace Spyder.Console.Packets
{
	public class IPRefreshPacket : IPPacket
	{
		public const int SIZE = 0x78;
		public const int START = 0x4;


		public byte Revision
		{
			get { return Data[1]; }
			set { Data[1] = value; }
		}

		public IPRefreshPacket() : base(964)
		{
		}
		public IPRefreshPacket(byte[] data)
		{
			this.Data = data;
		}
		public void AddPortRefresh(Segment seg, int pktIndex)
		{
			AddPortRefresh(seg, pktIndex, seg.PortID);
		}
		public void AddPortRefresh(Segment seg, int pktIndex, int targetPortID)
		{
			int pos = START + (SIZE * pktIndex);

			Data[pos] = (byte)targetPortID;
			pos += 8;
			for (int i = 0; i < 16; i++)
			{
				if (seg.ColorButtons[i] is ColorButton btn)
				{
					ushort val = (ushort)(btn.BlinkColor | (btn.Color << 4));
					if (btn.Blink2x)
						val |= (ushort)(1 << 8);
					if (btn.Dim)
						val |= (ushort)(1 << 9);


					Data[pos++] = (byte)((val >> 8) & 0xff);
					Data[pos++] = (byte)(val & 0xff);
				}
			}

			pos = START + (SIZE * pktIndex) + 0x28;
			char[] text = seg.GetDisplayText().ToCharArray();
			for (int i = 0; i < text.Length; i++)
				Data[pos++] = (byte)text[i];
		}

		public Dictionary<int, Segment> GetPortRefresh()
		{
			const int segmentsPerPacket = 8;
			var response = new Dictionary<int, Segment>();

			for (int i = 0; i < segmentsPerPacket; i++)
			{
				int pos = START + (SIZE * i);
				int portID = Data[pos];
				pos += 8;

				Segment2x8CD segment = new Segment2x8CD(portID)
				{
					FlagsA = 1
				};

				for (int j = 0; j < 16; j++)
				{
					// 16 bit word loaded sequentially for clarity
					ushort val = (ushort)((ushort)Data[pos++] << 8);
					val |= (ushort)(Data[pos++]);

					// first 8 bits are color and blink color (4 bits each)
					segment.ColorButtons[j] = new ColorButton(j)
					{
						Color = val & 0xf,
						BlinkColor = (val >> 4) & 0xf,
						Blink2x = (val & (1 << 8)) > 0,
						Dim = (val & (1 << 9)) > 0
					};
				}

				StringBuilder sb = new StringBuilder();
				pos = START + (SIZE * i) + 0x28;
				while (pos < (START + (SIZE * (i + 1))))
					sb.Append((char)Data[pos++]);

				string text = sb.ToString();
				segment.SetDisplay(text);

				response.Add(segment.PortID, segment);
			}
			return response;
		}

		//public void PortRefreshSegments(Vista.Controller.Console con)
		//{
		//    // 8 segments per packet
		//    for (int i = 0; i < 8; i++)
		//    {
		//        int pos = START + (SIZE * i);

		//        int portID = Data[pos++];
		//        if (con.Segments.ContainsKey(portID))
		//        {
		//            Segment seg = con.Segments[portID] as Segment;
		//            if (seg == null)
		//                continue;

		//            for (int j = 0; j < 16; j++)
		//            {
		//                // 16 bit word loaded sequentially for clarity
		//                byte msb = Data[pos++];
		//                byte lsb = Data[pos++];
		//                ushort val = (ushort)(lsb | (msb << 8));

		//                ColorButton btn = seg.ColorButtons[j] as ColorButton;

		//                // first 8 bits are color and blink color (4 bits each)
		//                btn.Color = val & 0xf;
		//                btn.BlinkColor = val >> 4;
		//                btn.Blink2x = (val & (1 << 8)) > 0;
		//                btn.Dim = (val & (1 << 9)) > 0;

		//            }
		//        }
		//    }
		//}
	}
}

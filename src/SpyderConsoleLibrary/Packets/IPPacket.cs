using System;

namespace Spyder.Console.Packets
{
	public enum IPPacketType 
	{
		Refresh = 0xc1, 
		Config = 0xc2,
		Status = 0xc3
	};
	public enum PortStatus 
	{
		Disabled = 0, 
		Mod_2x8D = 1, 
		Mod_4x4D = 2, 
		Mod_4x4 = 3, 
		Mod_EncAnalogD = 4,
		LCD_Timeouts = 0xfc,
		ChkSumErrors = 0xfd,
		NoResponse = 0xfe,
		Configuring = 0xff
	}

	public class IPPacket
	{
		public IPPacketType Type 
		{
			get { return (IPPacketType)Data[0]; } 
			set { Data[0] = (byte)value; }
		}

		public byte[] Data { get; set; }
	 
		public IPPacket(int size)
		{
			Data = new byte[size];
		}
		public IPPacket() : this(128)
		{
		}

		public ushort this[int index]
		{
			get 
			{
				return (ushort)(Data[index + 1] | Data[index] << 8);
			}
			set
			{
				Data[index + 1] = (byte)(value & 0xff);
				Data[index] = (byte)(value >> 8);
			}
		}

	}
}

using System;

namespace Spyder.Console.Packets
{
	public enum TestResult_LXT972A {Pass, Fail};
	public enum TestResult_I2CProm {Pass, BusNotReady, BusReadyNoAck, BusNotReadyNoAck}; 

	public class IPStatusPacket : IPPacket
	{
		public const int UICMD_START = 0x112;
		public const int SIZE = 0x212;

		public byte PktType {get {return Data[0];} set {Data[0] = value;}}
		public byte CalPromChkSumError {get {return Data[1];} set {Data[1] = value;}}

		public ushort SerialNum 
		{
			get 
			{
				return (ushort)(Data[3] | (Data[2] << 8));
			} 
			set 
			{
				Data[3] = (byte)(value & 0xff); 
				Data[2] = (byte)((value >> 8) & 0xff);
			}
		}
		public ushort ModuleType 
		{
			get 
			{
				return (ushort)(Data[5] | (Data[4] << 8));
			} 
			set 
			{
				Data[5] = (byte)(value & 0xff); 
				Data[4] = (byte)((value >> 8) & 0xff);
			}
		}
		public ushort HWVersion 
		{
			get 
			{
				return (ushort)(Data[7] | (Data[6] << 8));
			} 
			set 
			{
				Data[7] = (byte)(value & 0xff); 
				Data[6] = (byte)((value >> 8) & 0xff);
			}
		}
		public ushort VHWVersion 
		{
			get 
			{
				return (ushort)(Data[9] | (Data[8] << 8));
			} 
			set 
			{
				Data[9] = (byte)(value & 0xff); 
				Data[8] = (byte)((value >> 8) & 0xff);
			}
		}
		public ushort SWVersion 
		{
			get 
			{
				return (ushort)(Data[11] | (Data[10] << 8));
			} 
			set 
			{
				Data[11] = (byte)(value & 0xff); 
				Data[10] = (byte)((value >> 8) & 0xff);
			}
		}
		public byte Revision {get {return Data[0x110];} set {Data[0x110] = value;}}
		public byte UICmdCount {get {return Data[0x111];} set {Data[0x111] = value;}}

		public IPStatusPacket() : base(SIZE)
		{
		}

		public IPStatusPacket(byte[] data)
		{
			this.Data = data;
		}

		public IPPortStatus GetPortStatus(int portID)
		{
			return new IPPortStatus(Data, portID);
		}

		public UICueCmd GetUICmd(int index)
		{
			return new UICueCmd(Data, index);
		}

		public void SetUICmd(int index, int portID, int controlID, int val)
		{
			int pos = UICueCmd.START + (UICueCmd.LENGTH * index);
			Data[pos++] = (byte)portID;
			Data[pos++] = (byte)controlID;
			Data[pos++] = (byte)((val >> 8) & 0xff);
			Data[pos] = (byte)(val & 0xff);
		}

		public void SetBarAndJoystick(int tBar, int joyX, int joyY, int joyZ)
        {
			byte joyXByte = joyX >= 0 ? (byte)joyX : (byte)(0x80 | (byte)Math.Abs(joyX));
			byte joyYByte = joyY >= 0 ? (byte)joyY : (byte)(0x80 | (byte)Math.Abs(joyY));
			byte joyZByte = joyZ >= 0 ? (byte)joyZ : (byte)(0x80 | (byte)Math.Abs(joyZ));

			int pos = IPPortStatus.START + (IPPortStatus.LENGTH * ConsoleSimClient.ENCODER_PORT);
			Data[pos++] = (byte)PortStatus.Mod_EncAnalogD;
			Data[pos++] = 0x01; //KeyCtrlVHWVer
			Data[pos++] = 0x00; //SwitchState (LSB)
			Data[pos++] = 0x00; //SwitchState (MSB)
			Data[pos++] = (byte)tBar;
			Data[pos++] = (byte)joyX;
			Data[pos++] = (byte)(joyY > 0 ? 0-joyY : Math.Abs(joyY));
			Data[pos++] = (byte)joyZ;
		}
	}

	public class UICueCmd
	{
		public const int START = 0x112;
		public const int LENGTH = 0x04;
		public byte PortID { get; set; }
		public byte ControlID { get; set; }
		public ushort ControlValue { get; set; }

		public UICueCmd(byte[] data, int index)
		{
			int pos = START + (LENGTH * index);
			PortID = data[pos++];
			ControlID = data[pos++];
			byte msb = data[pos++];
			byte lsb = data[pos];
			ControlValue = (ushort)(lsb | msb << 8);
		}
	}
	public class IPPortStatus
	{
		public const int START = 0x10;
		public const int LENGTH = 0x08;

		public PortStatus Status;
		public byte KeyCtrlVHWVer;
		public ushort SwitchState;
		public byte TBar;
		public byte JoyX;
		public byte JoyY;
		public byte JoyZ;

		public IPPortStatus(byte[] data, int portID)
		{
			int pos = START + (LENGTH * portID);
			Status = (PortStatus)data[pos++];
			KeyCtrlVHWVer = data[pos++];
			SwitchState = data[pos++];
			SwitchState |= (ushort)(data[pos++] << 8);
			TBar = data[pos++];
			JoyX = data[pos++];
			JoyY = data[pos++];
			JoyZ = data[pos];
		}
	}
}

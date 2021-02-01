using System;

namespace Spyder.Console.Modules
{

	/// <summary>
	/// Summary description for Display2x40.
	/// </summary>
	public class CharDisplay
	{
												//	0123456789012345678901234567890123456789
		public int Width { get; private set; } = 40;
		public int Height { get; private set; } = 2;
		public string DisplayText { get; set; } = string.Empty;

		public CharDisplay()
		{
			Clear();
		}
		public CharDisplay(int width, int height)
		{
			Width = width;
			Height = height;
			Clear();
		}
		public void Startup(int width, int height)
		{
			Width = width;
			Height = height;
			Clear();
		}

		public void Clear()
		{
			DisplayText = new string(' ', Width * Height);
		}

		public void InsertText(int row, int col, string val)
		{
			DisplayText.Insert((row * Width) + col, val);
		}
		public void OverwriteText(int row, int startPos, string val)
		{
			int len = val.Length;
			try
			{
				string s = DisplayText.Remove(startPos + (row * Width), len);
				DisplayText = s.Insert(startPos + (row * Width), val);
			}
			catch{}
		}
	}
}

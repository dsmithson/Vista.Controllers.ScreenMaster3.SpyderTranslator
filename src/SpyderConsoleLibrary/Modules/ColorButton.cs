using System;
using System.Drawing;

namespace Spyder.Console.Modules
{
	/// <summary>
	/// Summary description for ColorButton.
	/// </summary>
	public class ColorButton
	{
		public int ControlID { get; set; }
		public bool Dim { get; set; }
		public int Color { get; set; }
		public int BlinkColor { get; set; }
		public string Text { get; set; } = string.Empty;
		public int Value { get; set; } = 0;
		public bool Blink2x { get; set; } = false;

		public ColorButton()
		{
		}
		
		public ColorButton(int control, int color, int blinkColor, bool dim, string text)
		{
			this.ControlID = control;
			this.Color = color;
			this.BlinkColor = blinkColor;
			this.Dim = dim;
			this.Text = text;
		}
		public ColorButton(int control)
		{
			SetButton(control, 0, 0, false, "");
		}
		public void SetButton(int control, int color, int blinkColor, bool dim, string text)
		{
			SetButton(control, color, blinkColor, dim, text, 0);
		}
		public void SetButton(int control, int color, int blinkColor, bool dim, string text, int val)
		{
			this.ControlID = control;
			this.Color = color;
			this.BlinkColor = blinkColor;
			this.Dim = dim;
			this.Text = text;
			this.Value = val;
		}	

		public void Clear()
		{
			SetButton(ControlID, 0, 0, false, "");
		}
		//public void SetButton(int control, int color)
		//{
			
		//}
	}
}

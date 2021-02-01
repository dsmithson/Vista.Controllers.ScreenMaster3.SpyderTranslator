using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Spyder.Console.Modules
{
	public enum SegmentType
	{
		Other,
		Segment2x8,
		Segment4x4,
		Encoder
	}

	/// <summary>
	/// Summary description for Segment.
	/// </summary>
	public class Segment
	{
		protected readonly CharDisplay display = null;
		protected readonly List<ColorButton> buttons = new List<ColorButton>();

		/// <summary>
		/// Specifies the type of the current segment
		/// </summary>
		public SegmentType Type { get; set; }

		public List<ColorButton> ColorButtons
		{
			get { return buttons; }
		}
		public int ConfigA { get; set; } = 0;
		public int ConfigB { get; set; } = 0;
		public int ConfigC { get; set; } = 0;
		public uint FlagsA { get; set; } = 1;
		public uint ModesA { get; set; } = 0;

		public int PortID { get; private set; } = 0;
	
		public ushort Pressed { get; set; } = 0;

		public Segment(int portID, SegmentType type)
		{
            this.Type = type;
			this.PortID = portID;

			for (int i = 0; i < 16; i++)
			{
				buttons.Add(new ColorButton(i, 0, 0, true, string.Empty));
			}
			
			display = new CharDisplay();
		}
		public virtual void Clear()
		{
            foreach (ColorButton button in buttons)
			{
                button.Clear();
			}
			display.Clear();
		}

		public virtual void Refresh()
        {

        }

		public virtual void ButtonAction(int port, int control, bool pressed)
        {

        }

		public void SetButton(int control, string text, int color, int blinkColor, bool dim)
		{
			if(control >= buttons.Count)
				return;

			ColorButton button = buttons[control] as ColorButton;	
			button.SetButton(control, color, blinkColor, dim, text);
		}
		
		public void SetButton(int control, string text, int color, bool dim)
		{
			if(control >= buttons.Count)
				return;

			ColorButton button = buttons[control] as ColorButton;
			button.SetButton(control, (int)color, (int)color, dim, text);
		}

        public void SetDisplay(string allText)
        {
            if(!string.IsNullOrEmpty(allText))
                display.DisplayText = allText.ClipOrPad(80);
        }

		public void SetDisplay(string line1, string line2)
		{
            SetDisplay(40, line1, line2);
		}
		public void SetDisplay(string line1, string line2, string line3, string line4)
		{
            SetDisplay(20, line1, line2, line3, line4);
		}
        protected void SetDisplay(int widthPerLine, params string[] lines)
        {
            StringBuilder builder = new StringBuilder(lines.Length * widthPerLine);
            foreach (string line in lines)
                builder.Append(line.ClipOrPad(widthPerLine));

            display.DisplayText = builder.ToString();
        }

		public bool IsPressed(int control)
		{
			return (Pressed & (1 << control)) > 0;
		}
		public void SetDisplayFromButtons()
		{
            StringBuilder builder = new StringBuilder(display.Width * display.Height);
            foreach (ColorButton button in buttons)
            {
                if (button.Text == null)
                    button.Text = string.Empty;

                builder.Append(button.Text.ClipOrPad(4));
                builder.Append(' ');
            }
            display.DisplayText = builder.ToString();
		}

		public void OverwriteDisplayText(int row, int startPos, string text)
		{
			display.OverwriteText(row, startPos, text);
		}
	
		public ColorButton GetButton(int control)
		{
			if(control >= buttons.Count)
				return null;

			return (ColorButton)buttons[control];
		}

		public string GetDisplayText()
		{
			return display.DisplayText;
		}
	}
}

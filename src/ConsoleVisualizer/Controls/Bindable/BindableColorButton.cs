using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using Spyder.Console;
using Spyder.Console.Controls;

namespace Spyder.Console.Controls.Bindable
{
    public class BindableColorButton : BindableControlBase
    {
        private static readonly ColorPalette spyderColorPalette = new ColorPalette();

        private bool isPressed;
        public bool IsPressed
        {
            get { return isPressed; }
            set
            {
                if (isPressed != value)
                {
                    isPressed = value;
                    OnPropertyChanged("IsPressed");
                }
            }
        }

        private ButtonFunction function;
        public ButtonFunction Function
        {
            get { return function; }
            set
            {
                if (function != value)
                {
                    function = value;
                    OnPropertyChanged("Function");
                }
            }
        }

        private int colorIndex;
        public int ColorIndex
        {
            get { return colorIndex; }
            set
            {
                if (colorIndex != value)
                {
                    colorIndex = value;
                    OnPropertyChanged("ColorIndex");
                    OnPropertyChanged("Color");
                }
            }
        }

        public Color Color
        {
            get
            {
                if (colorIndex < spyderColorPalette.Colors.Count)
                    return spyderColorPalette.Colors[colorIndex];
                else
                    return Colors.Red;
            }
        }

        public Color AlternateColor
        {
            get
            {
                if (alternateColorIndex < spyderColorPalette.Colors.Count)
                    return spyderColorPalette.Colors[alternateColorIndex];
                else
                    return Colors.Red;
            }
        }

        private int alternateColorIndex;
        public int AlternateColorIndex
        {
            get { return alternateColorIndex; }
            set
            {
                if (alternateColorIndex != value)
                {
                    alternateColorIndex = value;
                    OnPropertyChanged("AlternateColorIndex");
                    OnPropertyChanged("AlternateColor");
                }
            }
        }


        private bool dim;
        public bool Dim
        {
            get { return dim; }
            set
            {
                if (dim != value)
                {
                    dim = value;
                    OnPropertyChanged("Dim");
                }
            }
        }

        private bool blink2x;
        public bool Blink2x
        {
            get { return blink2x; }
            set
            {
                if (blink2x != value)
                {
                    blink2x = value;
                    OnPropertyChanged("Blink2x");
                }
            }
        }

        private int buttonValue;
        public int Value
        {
            get { return buttonValue; }
            set
            {
                if (buttonValue != value)
                {
                    buttonValue = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public override void RenderControl(BindableSegment segment)
        {
            //This class is the base level control at the segment.  No work is required
        }

        public void SetButton(string text, FrontPanelColors color)
        {
            SetButton(text, color, !IsPressed, ButtonFunction.None, -1);
        }

        public void SetButton(string text, FrontPanelColors color, bool dim)
        {
            SetButton(text, color, dim, ButtonFunction.None, -1);
        }

        public void SetButton(string text, FrontPanelColors color, bool dim, ButtonFunction function)
        {
            SetButton(text, color, dim, function, -1);
        }

        public void SetButton(string text, FrontPanelColors color, bool dim, ButtonFunction function, int value)
        {
            Text = text;
            ColorIndex = (int)color;
            AlternateColorIndex = (int)color;
            Dim = dim;
            Function = function;
            Value = value;
        }

        public void Clear()
        {
            SetButton(string.Empty, FrontPanelColors.Off, true, ButtonFunction.None, -1);
        }
    }
}

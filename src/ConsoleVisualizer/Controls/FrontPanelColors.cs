using System;

namespace Spyder.Console.Controls
{
    public class FrontPanelColors : IFormattable, IComparable
    {
        public static readonly FrontPanelColors Off = new FrontPanelColors(0);
        public static readonly FrontPanelColors On = new FrontPanelColors(14);
        public static readonly FrontPanelColors PageScroll = new FrontPanelColors(6);

        public static readonly FrontPanelColors Black = new FrontPanelColors(0);
        public static readonly FrontPanelColors Blue1 = new FrontPanelColors(1);
        public static readonly FrontPanelColors Red = new FrontPanelColors(2);
        public static readonly FrontPanelColors Orange1 = new FrontPanelColors(3);
        public static readonly FrontPanelColors Purple = new FrontPanelColors(4);
        public static readonly FrontPanelColors Green1 = new FrontPanelColors(5);
        public static readonly FrontPanelColors Orange2 = new FrontPanelColors(6);
        public static readonly FrontPanelColors Green2 = new FrontPanelColors(7);
        public static readonly FrontPanelColors White = new FrontPanelColors(8);
        public static readonly FrontPanelColors Pink = new FrontPanelColors(9);
        public static readonly FrontPanelColors OrangePeach = new FrontPanelColors(10);
        public static readonly FrontPanelColors Blue2 = new FrontPanelColors(11);
        public static readonly FrontPanelColors Aqua = new FrontPanelColors(12);
        public static readonly FrontPanelColors Blue3 = new FrontPanelColors(13);
        public static readonly FrontPanelColors PurpleWhite = new FrontPanelColors(14);
        public static readonly FrontPanelColors Green3 = new FrontPanelColors(15);

        public int Value { get; private set; }

        protected FrontPanelColors(int value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format, Value);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is FrontPanelColors)
                return this.Value.CompareTo(((FrontPanelColors)obj).Value);
            else
                return this.Value.CompareTo(obj);
        }

        #endregion

        public static implicit operator int(FrontPanelColors color)
        {
            if (color == null)
                return -1;
            else
                return color.Value;
        }

        public static implicit operator FrontPanelColors(int value)
        {
            return new FrontPanelColors(value);
        }

        public static implicit operator FrontPanelColors(PaletteColors value)
        {
            if (value == PaletteColors.Off)
                return FrontPanelColors.Off;
            else if (value == PaletteColors.On)
                return FrontPanelColors.On;


            int newValue = (int)value;
            if (value == PaletteColors.CmdKeys)
                newValue = 3;
            else if (value == PaletteColors.LayerOff)
                newValue = 5;
            else if (value == PaletteColors.Preview)
                newValue = 4;
            else if (value == PaletteColors.Program)
                newValue = 2;
            else if (value == PaletteColors.PropertyLayer)
                newValue = 9;
            else if (value == PaletteColors.Source)
                newValue = 11;
            else if (value == PaletteColors.FuncKey)
                newValue = 14;
            else if (value == PaletteColors.LayerSC)
                newValue = 13;
            else if (value == PaletteColors.Effect)
                newValue = 7;

            return new FrontPanelColors(newValue);
        }
    }
}

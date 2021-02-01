using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Console.Controls
{
    public class ButtonFunction
    {
        public static readonly ButtonFunction None = new ButtonFunction();
        public static readonly ButtonFunction Source = new ButtonFunction();
        public static readonly ButtonFunction Learn = new ButtonFunction();
        public static readonly ButtonFunction Clear = new ButtonFunction();
        public static readonly ButtonFunction Keyboard = new ButtonFunction();
        public static readonly ButtonFunction PropertyPanelValueAdjust = new ButtonFunction();

        protected ButtonFunction()
        {
        }
    }
}

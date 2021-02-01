using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Timers;

namespace Spyder.Console.Controls
{
    public partial class ConsoleButton : System.Windows.Controls.ContentControl
    {
        public static readonly DependencyProperty PrimaryColorProperty = DependencyProperty.Register("PrimaryColor", typeof(Color), typeof(ConsoleButton), new FrameworkPropertyMetadata(Colors.Blue, onColorChanged));
        public static readonly DependencyProperty AlternateColorProperty = DependencyProperty.Register("AlternateColor", typeof(Color), typeof(ConsoleButton), new FrameworkPropertyMetadata(Colors.PowderBlue, onColorChanged));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(ConsoleButton));
        protected static readonly DependencyProperty CurrentColorProperty = DependencyProperty.Register("CurrentColor", typeof(Color), typeof(ConsoleButton), new FrameworkPropertyMetadata(Colors.Blue));

        private static bool blinkOn;
        private static Timer blinkTimer;
        private static List<ConsoleButton> blinkingButtons = new List<ConsoleButton>();

        static void onColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((e.Property == PrimaryColorProperty && !blinkOn) || (e.Property == AlternateColorProperty && blinkOn))
                ((ConsoleButton)sender).CurrentColor = (Color)e.NewValue;
        }

        static ConsoleButton()
        {
            //if (ApplicationState.Global.InDesignMode)
            //    return;

            blinkTimer = new Timer();
            blinkTimer.Interval = 500;
            blinkTimer.Elapsed += new ElapsedEventHandler(blinkTimer_Elapsed);
            blinkTimer.Start();
        }

        static void blinkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            blinkOn = !blinkOn;
            ConsoleButton firstButton = blinkingButtons.FirstOrDefault();
            if (firstButton == null)
                return;

            blinkTimer.Stop();

            firstButton.Dispatcher.Invoke((System.Threading.ThreadStart)(() =>
                {
                    blinkingButtons.ForEach(button => button.CurrentColor = (blinkOn ? button.PrimaryColor : button.AlternateColor));
                }));

            blinkTimer.Start();
        }

        protected Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }

        public object Label
        {
            get { return GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public Color PrimaryColor
        {
            get { return (Color)this.GetValue(PrimaryColorProperty); }
            set { this.SetValue(PrimaryColorProperty, value); }
        }

        public Color AlternateColor
        {
            get { return (Color)this.GetValue(AlternateColorProperty); }
            set { this.SetValue(AlternateColorProperty, value); }
        }
        

        public ConsoleButton()
        {
            InitializeComponent();

            //Add to static collection of blinking buttons to allow my button colors to blink
            this.Loaded += (e, args) => blinkingButtons.Add(this);
            this.Unloaded += (e, args) => blinkingButtons.Remove(this);
        }
    }
}

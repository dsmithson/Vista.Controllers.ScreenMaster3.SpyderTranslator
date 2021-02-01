using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Spyder.Console.Controls.Bindable;

namespace Spyder.Console.Controls
{
    public delegate void ButtonPressHandler(object sender, SegmentPressEventArgs e);

    public class SegmentBase : UserControl, INotifyPropertyChanged
    {
        public static readonly RoutedEvent ButtonPressedEvent = EventManager.RegisterRoutedEvent("ButtonPressed", RoutingStrategy.Bubble, typeof(ButtonPressHandler), typeof(SegmentBase));

        public event ButtonPressHandler ButtonPressed
        {
            add { AddHandler(ButtonPressedEvent, value); }
            remove { RemoveHandler(ButtonPressedEvent, value); }
        }

        private ObservableCollection<object> buttonLabels = new ObservableCollection<object>();
        public ObservableCollection<object> ButtonLabels
        {
            get { return buttonLabels; }
            set
            {
                buttonLabels = value;
                OnPropertyChanged("ButtonLabels");
            }
        }

        public BindableSegment Segment
        {
            get { return this.DataContext as BindableSegment; }
        }

        private void OnButtonPressed(int portID, int buttonID, bool pressed)
        {
            SegmentPressEventArgs args = new SegmentPressEventArgs(ButtonPressedEvent, portID, buttonID, pressed);
            RaiseEvent(args);
        }

        protected void mouse_down(object sender, MouseButtonEventArgs e)
        {
            handleButtonAction(sender, true);
            e.Handled = true;
        }

        protected void mouse_up(object sender, MouseButtonEventArgs e)
        {
            handleButtonAction(sender, false);
            e.Handled = true;
        }

        protected void touch_down(object sender, TouchEventArgs e)
        {
            handleButtonAction(sender, true);
            e.Handled = true;
        }

        protected void touch_up(object sender, TouchEventArgs e)
        {
            handleButtonAction(sender, false);
            e.Handled = true;
        }

        private void handleButtonAction(object sender, bool pressed)
        {
            int portID, buttonID;
            if (getPortAndButtonID(sender, out portID, out buttonID))
                OnButtonPressed(portID, buttonID, pressed);
        }

        private bool getPortAndButtonID(object sender, out int port, out int buttonID)
        {
            port = -1;
            buttonID = -1;

            FrameworkElement button = sender as FrameworkElement;
            if (button == null || !(button.DataContext is BindableColorButton colorButton) || this.DataContext is not BindableSegment segment)
                return false;

            port = segment.PortID;
            buttonID = colorButton.ButtonIndex;
            return true;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class SegmentPressEventArgs : RoutedEventArgs
    {
        public int Port { get; set; }
        public int ButtonID { get; set; }
        public bool Pressed { get; set; }

        public SegmentPressEventArgs(RoutedEvent routedEvent, int port, int buttonID, bool pressed) : base(routedEvent)
        {
            this.Port = port;
            this.ButtonID = buttonID;
            this.Pressed = pressed;
        }
    }
}

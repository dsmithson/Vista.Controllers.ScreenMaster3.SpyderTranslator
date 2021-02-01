using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Spyder.Console.Controls.Bindable
{
    [DefaultEvent("ValueChanged"), DefaultProperty("Label")]
    public abstract class BindableControlBase : INotifyPropertyChanged
    {
        private int buttonIndex;
        public int ButtonIndex
        {
            get { return buttonIndex; }
            set
            {
                if (buttonIndex != value)
                {
                    buttonIndex = value;
                    OnPropertyChanged("RenderStartIndex");
                }
            }
        }

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                if (label != value)
                {
                    label = value;
                    OnPropertyChanged("Label");
                }
            }
        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        public abstract void RenderControl(BindableSegment segment);
        
        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

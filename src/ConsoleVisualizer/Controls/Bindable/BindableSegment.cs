using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Spyder.Console.Modules;
using Knightware.Collections;
using Spyder.Console;

namespace Spyder.Console.Controls.Bindable
{
    public enum LcdDisplayMode { Buttons, Segment }

    public class BindableSegment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ButtonUpdated;
        protected void OnButtonUpdated(BindableColorButton button, string propertyName)
        {
            if (ButtonUpdated != null)
                ButtonUpdated(button, new PropertyChangedEventArgs(propertyName));
        }

        private LcdDisplayMode lcdMode = LcdDisplayMode.Buttons;
        public LcdDisplayMode LcdMode
        {
            get { return lcdMode; }
            set
            {
                if (lcdMode != value)
                {
                    lcdMode = value;
                    OnPropertyChanged("LcdMode");
                    UpdateTextLines();
                }
            }
        }

        private SegmentType segmentType = SegmentType.Segment4x4;
        public SegmentType SegmentType
        {
            get { return segmentType; }
            set
            {
                if (segmentType != value)
                {
                    segmentType = value;
                    OnPropertyChanged("SegmentType");
                    UpdateTextLines();
                }
            }
        }

        private int portID;
        public int PortID
        {
            get { return portID; }
            set
            {
                if (portID != value)
                {
                    portID = value;
                    OnPropertyChanged("PortID");
                }
            }
        }

        private NotifyingObservableCollection<BindableColorButton> buttons = new NotifyingObservableCollection<BindableColorButton>();
        public NotifyingObservableCollection<BindableColorButton> Buttons
        {
            get { return buttons; }
            set
            {
                if (buttons != value)
                {
                    buttons = value;
                    OnPropertyChanged("Buttons");
                    UpdateTextLines();
                }
            }
        }

        private string textLine1;
        public string TextLine1
        {
            get { return textLine1; }
            set
            {
                if (textLine1 != value)
                {
                    textLine1 = value;
                    OnPropertyChanged("TextLine1");
                }
            }
        }

        private string textLine2;
        public string TextLine2
        {
            get { return textLine2; }
            set
            {
                if (textLine2 != value)
                {
                    textLine2 = value;
                    OnPropertyChanged("TextLine2");
                }
            }
        }

        private string textLine3;
        public string TextLine3
        {
            get { return textLine3; }
            set
            {
                if (textLine3 != value)
                {
                    textLine3 = value;
                    OnPropertyChanged("TextLine3");
                }
            }
        }
        
        private string textLine4;
        public string TextLine4
        {
            get { return textLine4; }
            set
            {
                if (textLine4 != value)
                {
                    textLine4 = value;
                    OnPropertyChanged("TextLine4");
                }
            }
        }
        

        public BindableColorButton GetButton(int buttonIndex)
        {
            if (buttons != null)
            {
                foreach (BindableColorButton button in buttons)
                    if (button.ButtonIndex == buttonIndex)
                        return button;
            }
            return null;
        }

        public BindableColorButton SetButton(int buttonIndex, string text, FrontPanelColors color)
        {
            return SetButton(buttonIndex, text, color, !Buttons[buttonIndex].IsPressed, ButtonFunction.None, -1);
        }

        public BindableColorButton SetButton(int buttonIndex, string text, FrontPanelColors color, bool dim)
        {
            return SetButton(buttonIndex, text, color, dim, ButtonFunction.None, -1);
        }

        public BindableColorButton SetButton(int buttonIndex, string text, FrontPanelColors color, bool dim, ButtonFunction function)
        {
            return SetButton(buttonIndex, text, color, dim, function, -1);
        }

        public BindableColorButton SetButton(int buttonIndex, string text, FrontPanelColors color, bool dim, ButtonFunction function, int value)
        {
            BindableColorButton button = GetButton(buttonIndex);
            if (button != null)
                button.SetButton(text, color, dim, function, value);

            return button;
        }

        public void ClearButton(int buttonIndex)
        {
            BindableColorButton button = GetButton(buttonIndex);
            if (button != null)
                button.Clear();
        }

        public void UpdateTextLines()
        {
            if (LcdMode == LcdDisplayMode.Buttons)
            {
                TextLine1 = generateTextLine(0);
                TextLine2 = generateTextLine(1);
                TextLine3 = generateTextLine(2);
                TextLine4 = generateTextLine(3);
            }
        }

        public void CopyFrom(Segment segment)
        {
            CopyFrom(segment, LcdDisplayMode.Buttons);
        }

        public void CopyFrom(Segment segment, LcdDisplayMode lcdTextSource)
        {
            if (segment == null)
                return;

            this.SegmentType = segment.Type;
            this.PortID = segment.PortID;
            this.LcdMode = lcdTextSource;

            if (lcdMode == LcdDisplayMode.Segment)
            {
                int lineWidth = 40;
                int lineCount = 2;
                
                if (segmentType == SegmentType.Segment4x4)
                {
                    lineWidth = 20;
                    lineCount = 4;
                }

                string text = segment.GetDisplayText();

                //Sanity check on text field length
                int totalWidth = lineWidth * lineCount;
                if (text.Length < totalWidth)
                    text = text.ClipOrPad(totalWidth);

                for (int i = 0; i < lineCount; i++)
                {
                    int startIndex = lineWidth * i;
                    string subString = text.Substring(startIndex, lineWidth);
                    setTextLine(i, subString);
                }
            }            
            
            //Update buttons
            foreach (ColorButton button in segment.ColorButtons)
            {
                BindableColorButton thisButton = GetButton(button.ControlID);
                thisButton.AlternateColorIndex = button.BlinkColor;
                thisButton.Blink2x = button.Blink2x;
                thisButton.ColorIndex = button.Color;
                thisButton.Dim = button.Dim;
                thisButton.Text = button.Text;
                thisButton.Value = button.Value;
            }
        }

        private void setTextLine(int lineNumber, string text)
        {
            if (lineNumber == 0)
                TextLine1 = text;
            else if (lineNumber == 1)
                TextLine2 = text;
            else if (lineNumber == 2)
                TextLine3 = text;
            else if (lineNumber == 3)
                TextLine4 = text;
        }

        private string generateTextLine(int lineNumber)
        {
            const int charsPerButton = 4;
            int buttonsPerLine = (SegmentType == SegmentType.Segment2x8 ? 8 : 4);
            int startIndex = (buttonsPerLine * lineNumber);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buttonsPerLine; i++)
            {
                BindableColorButton button = GetButton(startIndex + i);
                string rawText;
                if (button == null || string.IsNullOrEmpty(button.Text))
                    rawText = string.Empty;
                else
                    rawText = button.Text;

                builder.Append(rawText.ClipOrPad(charsPerButton));

                //One character space between buttons
                builder.Append(' ');
            }
            return builder.ToString();
        }

        public BindableSegment()
        {
            for (int i = 0; i < 16; i++)
            {
                BindableColorButton button = new BindableColorButton() { ButtonIndex = i };
                button.PropertyChanged += new PropertyChangedEventHandler(button_PropertyChanged);
                buttons.Add(button);
            }

            buttons.CollectionItemChanged += Buttons_CollectionItemChanged;
            UpdateTextLines();
        }

        void button_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnButtonUpdated(sender as BindableColorButton, e.PropertyName);
        }

        private void Buttons_CollectionItemChanged(object sender, NotifyCollectionItemChangedEventArgs e)
        {
            //Update LCD Text if button text has changed
            if (string.Compare("Text", e.PropertyName, true) == 0 && LcdMode == LcdDisplayMode.Buttons)
                UpdateTextLines();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Clear()
        {
            foreach (BindableColorButton button in this.buttons)
                button.Clear();

            TextLine1 = string.Empty;
            TextLine2 = string.Empty;
            TextLine3 = string.Empty;
            TextLine4 = string.Empty;
        }

        public Segment ToSegment()
        {
            Segment response = new Segment(this.portID, this.segmentType);
            foreach (BindableColorButton button in buttons)
                response.SetButton(button.ButtonIndex, button.Text, button.ColorIndex, button.AlternateColorIndex, button.Dim);

            if (this.segmentType == SegmentType.Segment2x8)
                response.SetDisplay(textLine1, textLine2);
            else
                response.SetDisplay(textLine1, textLine2, textLine3, textLine4);

            return response;
        }
    }
}

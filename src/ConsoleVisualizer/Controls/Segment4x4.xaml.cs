using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Spyder.Console.Controls
{
    public enum Segment4x4ButtonLabels { Custom, CricketFrontPanel, X20ConsoleKeyFrameTop, X20ConsoleKeyFrameBottom, X20ConsoleSelectorTop, X20ConsoleSelectorBottom }
    public partial class Segment4x4 : SegmentBase
    {
        public static readonly DependencyProperty DisplayVisibleProperty = DependencyProperty.Register("DisplayVisible", typeof(bool), typeof(Segment4x4), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ButtonLabelTemplateProperty = DependencyProperty.Register("ButtonLabelTemplate", typeof(Segment4x4ButtonLabels), typeof(Segment4x4), new FrameworkPropertyMetadata(Segment4x4ButtonLabels.Custom, onButtonLabelTemplateChanged));

        private static void onButtonLabelTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Segment4x4ButtonLabels templateType = (Segment4x4ButtonLabels)e.NewValue;
            Segment4x4 control = (Segment4x4)sender;
            control.setButtonLabels(templateType);
        }

        public bool DisplayVisible
        {
            get { return (bool)GetValue(DisplayVisibleProperty); }
            set { SetValue(DisplayVisibleProperty, value); }
        }

        public Segment4x4ButtonLabels ButtonLabelTemplate
        {
            get { return (Segment4x4ButtonLabels)GetValue(ButtonLabelTemplateProperty); }
            set { SetValue(ButtonLabelTemplateProperty, value); }
        }

        public Segment4x4()
        {
            InitializeComponent();
        }

        private void setButtonLabels(Segment4x4ButtonLabels templateType)
        {
            if (templateType == Segment4x4ButtonLabels.Custom)
                return;

            if (templateType == Segment4x4ButtonLabels.CricketFrontPanel)
            {
                ButtonLabels = new ObservableCollection<object>()
                {
                    null, null, null, null,
                    "Home", buildStackPanel("T/L", "-"), buildRotatedArrow(0), buildStackPanel("B/R", "+"),
                    "Config", buildRotatedArrow(270), "Auto", buildRotatedArrow(90),
                    "Health", buildStackPanel("Undo", "Cancel"), buildRotatedArrow(180), buildStackPanel("Save", "Ok")
                };
            }
            else if (templateType == Segment4x4ButtonLabels.X20ConsoleKeyFrameTop)
            {
                ButtonLabels = new ObservableCollection<object>()
                {
                    null, "OFF", "PVW", "PGM",
                    buildStackPanel("Size", "Pos"), buildStackPanel("Border", "Size/", "Color"), buildStackPanel("Border", "Type"), "Shadow",
                    "Crop", null, null, null,
                    "AutoSync", "Levels", buildStackPanel("Color", "Key"), buildStackPanel("Luma", "Key")
                };
            }
            else if (templateType == Segment4x4ButtonLabels.X20ConsoleKeyFrameBottom)
            {
                ButtonLabels = new ObservableCollection<object>()
                {
                    "OSD", buildStackPanel("Input", "Config"), buildStackPanel("Aspect", "Ratio"), buildStackPanel("Script", "Preview"),
                    buildRotatedArrow(180), buildRotatedArrow(0), "-", "+",
                    null, null, null, null,
                    null, null, null, null
                };
            }
            else if (templateType == Segment4x4ButtonLabels.X20ConsoleSelectorTop)
            {
                ButtonLabels = new ObservableCollection<object>()
                {
                    null, null, null, null,
                    "Effect", buildStackPanel("DDR", "VTR"), "Layout", "Trans",
                    buildStackPanel("CMD", "Key"), "Treat", "Source", buildStackPanel("Func", "Key"),
                    "Store", "Bank", "Edit", "Delete"
                };
            }
            else if (templateType == Segment4x4ButtonLabels.X20ConsoleSelectorBottom)
            {
                ButtonLabels = new ObservableCollection<object>()
                {
                    buildStackPanel("7 <<", "INS"), buildStackPanel("8 #", "MOD"), buildStackPanel("9 >>", "DEL"), "Undo",
                    buildStackPanel("4 <", "HTM"), buildStackPanel("5 $", "TR TM"), buildStackPanel("6 >", "TRAJ"), "Redo",
                    buildStackPanel("1 -", "<-"), buildStackPanel("2 V", "VIEW"), buildStackPanel("3 +", "->"), buildStackPanel("Program", "Take"),
                    buildStackPanel("Clear", "FREE"), buildStackPanel("0 S", "LOOP"), buildStackPanel("Enter", "NEXT"), buildStackPanel("Preview", "Take")
                };
            }
        }

        private StackPanel buildStackPanel(params string[] textLines)
        {
            StackPanel stack = new StackPanel();
            foreach (string line in textLines)
                stack.Children.Add(new TextBlock() { Text = line });

            return stack;
        }

        private UIElement buildRotatedArrow(double angle)
        {
            TextBlock text = new TextBlock() { Text = "<" };
            text.LayoutTransform = new RotateTransform(angle + 90);
            return text;
        }
    }
}

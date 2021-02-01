using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Spyder.Console.ValueConverters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invert = false;
            Visibility hideVisibility = Visibility.Collapsed;
            if (parameter != null)
            {
                string[] parts = parameter.ToString().Split('|');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (string.Compare("Hide", parts[i], true) == 0)
                        hideVisibility = Visibility.Hidden;
                    else
                        bool.TryParse(parameter.ToString(), out invert);
                }
            }

            float transparencyValue;
            bool isVisible;
            if (value == null)
                isVisible = false;
            else if (float.TryParse(value.ToString(), out transparencyValue))
                isVisible = transparencyValue > 0f;
            else
                bool.TryParse(value.ToString(), out isVisible); //handles string or bool comparisons

            if (invert)
                isVisible = !isVisible;


            //Format response based on target type
            if (targetType == typeof(Visibility))
            {
                return (isVisible ? Visibility.Visible : hideVisibility);
            }
            else if (targetType == typeof(bool))
            {
                return isVisible;
            }
            else
                throw new NotSupportedException("specified type is not supported");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Spyder.Console.ValueConverters
{
    public class EnumToFriendlyNameConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //TODO:  Fill this code in for real
            if (value == null)
                return "<Null Name>";

            StringBuilder response = new StringBuilder();
            string rawText = value.ToString();
            int length = rawText.Length;
            for (int i = 0; i < length; i++)
            {
                char c = rawText[i];
                if (c == '_')
                {
                    response.Append(' ');
                }
                else
                {
                    if (i > 0 && char.IsUpper(c))
                        response.Append(' ');

                    response.Append(c);
                }
            }
            return response;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Simmakers.Assesment
{
    class MultiBooleanToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = 0; i < values.Length; i++)
            {
                bool value = (bool)values[i];
                if (value == true)
                {
                    return new SolidColorBrush(Colors.LightBlue);
                }
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace ENBManager.Core.ValueConverters
{
    public class EnumToCollectionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

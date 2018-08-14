using System;
using System.Globalization;
using System.Windows.Data;

namespace Talgat.Magafurov.Converters
{
    internal class ApplicationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string applicationType = value.ToString();
            switch (applicationType)
            {
                case "application":
                    return "Service";

                case "application-set":
                    return "Group";

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
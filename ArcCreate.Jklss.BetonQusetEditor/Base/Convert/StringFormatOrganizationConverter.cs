using System;
using System.Globalization;
using System.Windows.Data;

namespace ArcCreate.Jklss.BetonQusetEditor
{
    public class StringFormatOrganizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.TrimStart(' ').Replace(' ', '_');
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.TrimStart(' ').Replace(' ', '_');
            }

            return null;
        }
    }

    public class StringFormatIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                try
                {
                    return System.Convert.ToInt32(text);
                }
                catch
                {
                    return 1;
                }
                
                
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                try
                {
                    return System.Convert.ToInt32(text);
                }
                catch
                {
                    return 1;
                }


            }

            return null;
        }
    }

    public class StringAddConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text+"%";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text + "%";
            }

            return null;
        }
    }
}

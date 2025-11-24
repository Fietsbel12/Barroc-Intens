using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace BarrocIntens.View
{
    public class RolDisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string rol = value as string;
            string target = parameter as string;

            // Als rol gelijk is aan de target ("Eigenaar") → verberg de knop
            return rol == target ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}

using System;
using System.Globalization;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    class OptionalDateConverter : IValueConverter
    {
        // from the view-model to the view
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime ? User.OptionalDateTime((DateTime)value) : "unexpected type";
        }

        // from the view to the view-model
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
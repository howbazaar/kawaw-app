using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kawaw.Framework
{
    class NavigationConverter : IValueConverter
    {
        // from the view-model to the view
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Navigation is a one way to source, so don't need this.
            throw new NotImplementedException();
        }

        // from the view to the view-model
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value represents the base view navigation property
            var navigation = (INavigation) value;
            return new ViewModelNavigation(navigation);
        }
    }
}
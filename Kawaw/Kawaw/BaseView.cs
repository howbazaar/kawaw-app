using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class BaseView : ContentPage
    {
        public BaseView()
        {
            // Navigation is the name of the property in the view-model instance
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");
        }
    }
}
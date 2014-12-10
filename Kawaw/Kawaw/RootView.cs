using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class RootView : MasterDetailPage
    {
        public RootView()
        {
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");
 
        }
    }
}
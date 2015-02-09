using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class NavigationView : BaseView
    {
        // This is the view that contains the buttons that take the user to the different
        // main page elements, connections, events, profile (master bit of the root view).
        public NavigationView()
        {
            Title = "kawaw";
            // Icon = "kawaw.png";
            var list = new ListView();
            list.SetBinding(ListView.ItemsSourceProperty, "Items");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(TextCell));
            list.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
            list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Description");
            Content = list;

        }
    }
}
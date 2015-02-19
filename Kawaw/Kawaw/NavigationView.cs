using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class NavigationView : BaseView
    {
        class NavigationCell : ViewCell
        {
            public NavigationCell()
            {
                var header = new CellHeaderLabel();
                header.SetBinding(Label.TextProperty, "Name");
                var description = new Label();
                description.SetBinding(Label.TextProperty, "Description");
                var line = new BoxView
                {
                    Color = Color.Gray,
                    HeightRequest = 1,
                    Opacity = 0.5,
                };

                var viewLayout = new StackLayout()
                {
                    Children =
                    {
                        new StackLayout
                        {
                            Spacing = 3,
                            Padding = new Thickness(10, 10, 10, 9),
                            Children = {header, description},
                        },
                        line
                    }
                };
                View = viewLayout;
            }
        }


        // This is the view that contains the buttons that take the user to the different
        // main page elements, connections, events, profile (master bit of the root view).
        public NavigationView()
        {
            Title = "kawaw";
            Icon = "navigation.png";
            Padding = new Thickness(0, Device.OnPlatform(20,0,0),0,0);
            var list = new ListView
            {
                HasUnevenRows = true
            };
            list.SetBinding(ListView.ItemsSourceProperty, "Items");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(NavigationCell));
            Content = list;
        }
    }
}
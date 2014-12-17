using Xamarin.Forms;

namespace Kawaw
{
    class ConnectionsView : BaseView
    {
        class ConnectionCell : ViewCell
        {
            public ConnectionCell()
            {
                var name = new Label();
                name.SetBinding(Label.TextProperty, "Name");
                var org = new Label();
                org.SetBinding(Label.TextProperty, "Organisation");
                // I think this is causing VS to go into an infinite loop.
                var status = new Label() { HorizontalOptions = LayoutOptions.EndAndExpand};
                ///var status = new Label();
                status.SetBinding(Label.TextProperty, "Status");

                var viewLayout = new StackLayout()
                {
                    Children =
                    {
                        new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                name,
                                status,   
                            }
                        },
                        org,
                    }
                };

                View = new ContentView()
                {
                    Padding = 10,
                    Content = new Frame()
                    {
                        Padding = 5,
                        Content = viewLayout,
                        OutlineColor = Color.Silver,
                        HasShadow = true
                    }
                };

            }
        }

        public ConnectionsView()
        {
            Title = "Connections";
            Icon = "kawaw.png";

            var list = new ListView()
            {
                RowHeight = 70,
                
            };

            	
//var grid = new Grid {
//Children = { listView, defaultView }
//listView.Opacity = 0;

            list.SetBinding(ListView.ItemsSourceProperty, "Connections");
            //list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            // TODO: change this...
            list.ItemTemplate = new DataTemplate(typeof(ConnectionCell));
            //list.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
            //list.ItemTemplate.SetBinding(TextCell.DetailProperty, "Organisation");

            Content = list;

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));
        }
    }
}
using Kawaw.Controls;
using Xamarin.Forms;

namespace Kawaw
{
    class EventsView : PrimaryView
    {
        class LinkCell : ViewCell
        {
            public LinkCell()
            {
                var title = new Label();
                title.SetBinding(Label.TextProperty, "Title");
                var members = new Label();
                members.SetBinding(Label.TextProperty, "Members");

                var viewLayout = new StackLayout
                {
                    Children = {title, new ContentView{Padding = new Thickness(10,0), Content = members}},
                };

                View = new ContentView()
                {
                    Padding = new Thickness(10, 0),
                    Content = viewLayout,
                };
            }
        }
        class EventCell : ViewCell
        {
            public EventCell()
            {
                var time = new Label
                {
                    TextColor = Color.White
                };
                time.SetBinding(Label.TextProperty, "DateAndTime");
                var type = new Label
                {
                    FontAttributes = FontAttributes.Bold
                };
                type.SetBinding(Label.TextProperty, "Type");

                // TODO: work out how to have the labels not take up space in the layout if they are empty.
                var location = new Label();
                location.SetBinding(Label.TextProperty, "Location");
                var address = new Label();
                address.SetBinding(Label.TextProperty, "Address");

                var links = new RepeaterView<LinkViewModel>
                {
                    ItemTemplate = new DataTemplate(typeof (LinkCell))
                };
                links.SetBinding(RepeaterView<LinkViewModel>.ItemsSourceProperty, "Links");

                View = new StackLayout()
                {
                    Children =
                    {
                        new ContentView
                        {
                            BackgroundColor = Color.FromHex("#10558d"),
                            Padding = 10,
                            Content = time,
                        },
                        new ContentView
                        {
                            Padding = 10,
                            Content = new StackLayout
                            {
                                Children =
                                {
                                    type,
                                    links,
                                    location,
                                    address,
                                }
                            }
                        }
                    }
                };

            }
        }
        public EventsView()
        {
            Title = "Events";
            Content = new Label { Text = "events view" };


            var list = new ListView()
            {
                HasUnevenRows = true
            };

            var emptyText = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            emptyText.SetBinding(Label.TextProperty, "EmptyText");

            var empty = new ContentView
            {
                Padding = new Thickness(20, 20, 20, 80),
                Content = emptyText
            };
            empty.SetBinding(OpacityProperty, "EmptyOpacity");
            list.SetBinding(OpacityProperty, "ListOpacity");

            var grid = new Grid
            {
                Children = { list, empty }
            };

            list.SetBinding(ListView.ItemsSourceProperty, "Events");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(EventCell));

            Content = grid;
        }
    }
}
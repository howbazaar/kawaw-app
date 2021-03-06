using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
    class ConnectionsView : PrimaryView
    {
        class StatusColor : IValueConverter
        {
            // from the view-model to the view
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string)
                {
                    var status = value as string;
                    status = status.ToLower().Trim();
                    switch (status)
                    {
                        case "accepted":
                            return Color.FromHex("#5cb85c");
                        case "rejected":
                            return Color.FromHex("#d9534f");
                    }
                }
                return Color.FromHex("#999");
            }

            // from the view to the view-model
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        class ConnectionCell : ViewCell
        {
            public ConnectionCell()
            {
                var name = new Label();
                name.SetBinding(Label.TextProperty, "Name");
                var org = new Label();
                org.SetBinding(Label.TextProperty, "Organisation");
                var statusText = new Label()
                {
                    TextColor = Color.White
                };
                statusText.SetBinding(Label.TextProperty, "Status");
                var status = new Frame()
                {
                    Content = statusText,
                    Padding = new Thickness(5,0),
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    HasShadow = false,
                };
                status.SetBinding(BackgroundColorProperty, "Status", BindingMode.Default, new StatusColor());

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
                    Content = viewLayout,
                };

            }
        }

        public ConnectionsView()
        {
            Title = "Connections";

            var list = new ListView()
            {
                HasUnevenRows = true, // Actually means, go work out the hight of the rows.
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
                Children = {list, empty}
            };

            list.SetBinding(ListView.ItemsSourceProperty, "Connections");
            list.SetBinding(ListView.SelectedItemProperty, "SelectedItem", BindingMode.TwoWay);
            list.ItemTemplate = new DataTemplate(typeof(ConnectionCell));

            Content = grid;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("appearing, so subscribe {0}", this.Id);
            // SubscribeAlert<ConnectionsViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (ConnectionsViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });

            MessagingCenter.Subscribe(this, "show-options", async (ConnectionsViewModel model, ConnectionActionOptions options) =>
            {
                var textOptions = from tuple in options.Options select tuple.Item2;
                var heading = string.Format("{0} at {1}", options.Connection.Name, options.Connection.Organisation);
                var action = await DisplayActionSheet(heading, "Cancel", null, textOptions.ToArray());
                // action here is the long name, and we want the short one.
                if (action == null || action == "Cancel")
                    return;
                var result = from tuple in options.Options where tuple.Item2 == action select tuple.Item1;
                MessagingCenter.Send((object)this, "connection-action", new ConnectionAction
                {
                    Connection = options.Connection,
                    Name = result.Single(),
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("disappearing, so unsubscribe {0}", this.Id);
            // UnsubscribeAlert<ConnectionsViewModel>();
            MessagingCenter.Unsubscribe<ConnectionsViewModel, Alert>(this, "alert");
            MessagingCenter.Unsubscribe<ConnectionsViewModel, ConnectionActionOptions>(this, "show-options");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Kawaw.Controls;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    class NotificationsView : PrimaryView
    {
        class ResponceCell : ViewCell
        {
            public ResponceCell()
            {
                var grid = new Grid
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    RowSpacing = 0,
                    RowDefinitions =
                    {
                        new RowDefinition {Height = new GridLength(1, GridUnitType.Absolute)},
                        new RowDefinition {Height = 5},
                        new RowDefinition {Height = GridLength.Auto},
                        new RowDefinition {Height = 5},
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = 10},
                        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                        new ColumnDefinition {Width = 10},
                        new ColumnDefinition {Width = GridLength.Auto},
                        new ColumnDefinition {Width = 10},
                    }
                };
                grid.Children.Add(new BoxView
                {
                    Color = Color.Gray,
                    HeightRequest = 1,
                    Opacity = 0.5,
                }, 0,5,0,1);

                var name = new Label();
                name.SetBinding(Label.TextProperty, "Name");
                grid.Children.Add(name, 1, 2);

                var pending = new Label();
                pending.SetBinding(Label.TextProperty, "Status");
                pending.SetBinding(Label.TextColorProperty, "StatusColor");
                grid.Children.Add(pending, 3, 2);

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "TapCommand");
                grid.GestureRecognizers.Add(tapGestureRecognizer);

                View = grid;
            }
        }

        class NotificationCell : ViewCell
        {
            public NotificationCell()
            {
                var name = new Label();
                name.SetBinding(Label.TextProperty, "Type");
                name.SetBinding(Label.TextColorProperty, "ForegroundColor");
                var activity = new Label();
                activity.SetBinding(Label.TextProperty, "Activity");
                activity.SetBinding(Label.TextColorProperty, "ForegroundColor");

                var headerLayout = new StackLayout
                {
                    Padding = 10,
                    Children = { name, activity },
                };

                var description = new Label();
                description.SetBinding(Label.TextProperty, "Description");

                headerLayout.SetBinding(BackgroundColorProperty, "BackgroundColor");
                var descriptionView = new ContentView
                {
                    Padding = 20,
                    Content = description,
                };
                descriptionView.SetBinding(IsVisibleProperty, "DescriptionVisible");
                var members = new RepeaterView<NotificationResponseViewModel>
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    ItemTemplate = new DataTemplate(typeof(ResponceCell))
                };
                members.SetBinding(RepeaterView<NotificationResponseViewModel>.ItemsSourceProperty, "Responses");

                var bodyLayout = new StackLayout
                {
                    BackgroundColor = App.BackgroundColor,
                    Children = {descriptionView, members},
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                var frame = new Frame
                {
                    Padding = 2,
                    Content = new StackLayout
                    {
                        Children = { headerLayout, bodyLayout }
                    }
                };
                frame.SetBinding(BackgroundColorProperty, "BackgroundColor");
                frame.SetBinding(Frame.OutlineColorProperty, "BackgroundColor");

                View = new ContentView()
                {
                    Padding = 10,
                    Content = frame,
                };
            }
        }

        public NotificationsView()
        {
            Title = "Notifications";

            var main = new ScrollView();

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
            main.SetBinding(OpacityProperty, "ListOpacity");

            var grid = new Grid
            {
                Children = { main, empty }
            };

            var notifications = new RepeaterView<NotificationViewModel>
            {
                ItemTemplate = new DataTemplate(typeof(NotificationCell))
            };
            notifications.SetBinding(RepeaterView<NotificationViewModel>.ItemsSourceProperty, "Notifications");
            main.Content = notifications;

            Content = grid;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubscribeAlert<NotificationsViewModel>();
            MessagingCenter.Subscribe(this, "show-options", async (NotificationResponseViewModel model, NotificationResponseOptions options) =>
            {
                var textOptions = options.Options.Select(v => v.Item2);
                var heading = options.Response.Notification.Activity + " for " + options.Response.Name;
                var action = await DisplayActionSheet(heading, "Close", null, textOptions.ToArray());
                // action here is the long name, and we want the short one.
                if (action == null || action == "Close")
                    return;
                var result = from tuple in options.Options where tuple.Item2 == action select tuple.Item1;
                MessagingCenter.Send((object)this, "notification-action", new NotificationResponseAction
                {
                    Response = options.Response,
                    Action = result.Single(),
                });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeAlert<NotificationsViewModel>();
            MessagingCenter.Unsubscribe<NotificationResponseViewModel, NotificationResponseOptions>(this, "show-options");
        }

    }
}
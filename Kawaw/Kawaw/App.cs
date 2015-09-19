using System.Collections.Generic;
using System.Diagnostics;
using Kawaw.Database;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;


namespace Kawaw
{
    public class App : Application
    {
        private readonly RootViewModel _rootViewModel;
        // var accentColor = Color.FromHex("59C2FF");
        public static Color AccentColor = Color.FromHex("10558d");
        public static Color BackgroundColor = AccentColor.WithLuminosity(0.99);
        public static Color ForegroundColor = new Color(0.1);

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
#if DEBUG
            TestDB();
#endif
            DependencyService.Get<INotificationRegisration>().Register();

            GenerateKawawStyle();
            ViewModelNavigation.Register<LoginViewModel, LoginView>();
            ViewModelNavigation.Register<RegisterViewModel, RegisterView>();
            ViewModelNavigation.Register<EventsViewModel, EventsView>();
            ViewModelNavigation.Register<ConnectionsViewModel, ConnectionsView>();
            ViewModelNavigation.Register<NotificationsViewModel, NotificationsView>();
            ViewModelNavigation.Register<ProfileViewModel, ProfileView>();
            ViewModelNavigation.Register<ChangeDetailsViewModel, ChangeDetailsView>();
            ViewModelNavigation.Register<NavigationViewModel, NavigationView>();
            ViewModelNavigation.Register<AddEmailViewModel, AddEmailView>();

            User = new User();

            var page = RootViewModel.Profile;
            if (Properties.ContainsKey("Page"))
            {
                page = Properties["Page"] as string;
                Debug.WriteLine("Last page: {0}", page);
            }

            _rootViewModel = new RootViewModel(User);
            var rootView = new RootView
            {
                BindingContext = _rootViewModel,
                Master = ViewModelNavigation.GetPageForViewModel(_rootViewModel.NavigationModel),
            };
            _rootViewModel.SetDetails(page);

            MainPage = rootView;

            MessagingCenter.Subscribe(this, "show-page", (RootViewModel sender, BaseViewModel model) =>
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    Properties["Page"] = model.Name;
                }
            });
        }

        private void GenerateKawawStyle()
        {
            Resources = new ResourceDictionary();

            var labelStyle = new Style(typeof (Label))
            {
                Setters =
                {
                    new Setter {Property = Label.TextColorProperty, Value = ForegroundColor},
                    new Setter {Property = Label.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Medium, typeof(Label))},
                }
            };
            // no Key specified, becomes an implicit style for ALL labels
            Resources.Add(labelStyle);
            var cellHeaderLabelStyle = new Style(typeof (CellHeaderLabel))
            {
                BasedOn = labelStyle,
                Setters =
                {
                    new Setter {Property = Label.TextColorProperty, Value = AccentColor},
                }
            };
            Resources.Add(cellHeaderLabelStyle);

            var contentPageStyle = new Style(typeof (ContentPage))
            {
                Setters =
                {
                    new Setter {Property = VisualElement.BackgroundColorProperty, Value = BackgroundColor}
                }
            };
            Resources.Add("BaseViewStyle", contentPageStyle);
            var buttonStyle = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter {Property = VisualElement.BackgroundColorProperty, Value = BackgroundColor.AddLuminosity(-0.05)},
                    new Setter {Property = Button.BorderColorProperty, Value = AccentColor},
                    new Setter {Property = Button.BorderWidthProperty, Value = 2},
                    new Setter {Property = Button.BorderRadiusProperty, Value = 1},
                    new Setter {Property = Button.TextColorProperty, Value = ForegroundColor},
                    new Setter {Property = Button.FontAttributesProperty, Value = FontAttributes.Bold},
                    new Setter {Property = Button.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Large, typeof(Button))},
                }
            };
            Resources.Add(buttonStyle);

            var textCellStyle = new Style(typeof(TextCell))
            {
                Setters =
                {
                    new Setter {Property = TextCell.TextColorProperty, Value = AccentColor},
                    new Setter {Property = TextCell.DetailColorProperty, Value = ForegroundColor.AddLuminosity(0.1)}
                }
            };
            // no Key specified, becomes an implicit style for ALL labels
            Resources.Add(textCellStyle);

        }

#if DEBUG
        // ReSharper disable once InconsistentNaming
        private async void TestDB()
        {
            var db = DependencyService.Get<IDatabase>();
            // Hacky test for the notification code.
            var token = await db.NotificationToken();
            var old = await db.OldNotificationTokens();
            if (token == null && old.Count == 0)
            {
                Debug.WriteLine("Running rudimentary test for notification handling.");
                Debug.WriteLine("Set first token");
                await db.SetNotificationToken("first-token");

                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Set second token");
                await db.SetNotificationToken("second-token");
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Set third token");
                await db.SetNotificationToken("third-token");
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Remove first token");
                await db.RemoveOldNotificationToken("first-token");
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Remove second token");
                await db.RemoveOldNotificationToken("second-token");
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Set <null> token");
                await db.SetNotificationToken(null);
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));

                Debug.WriteLine("Remove third token");
                await db.RemoveOldNotificationToken("third-token");
                token = await db.NotificationToken();
                old = await db.OldNotificationTokens();
                Debug.WriteLine(token ?? "<null>");
                Debug.WriteLine(ListAsString(old));
            }
        }

        private static string ListAsString(List<string> values)
        {
            if (values == null) return "<null>";
            return "List<string>{" + string.Join(", ", values) + "}";
        }
#endif

        protected override void OnResume()
        {
            base.OnResume();
            if (User.Authenticated)
                _rootViewModel.RefreshUser(true);
        }

        public User User { get; private set; }

        public static void OnNotification(object sender, string tag, int id)
        {
            switch (tag)
            {
                case "event":
                    MessagingCenter.Send(sender, "show event", id);
                    break;
                case "notification":
                    MessagingCenter.Send(sender, "show notification", id);
                    break;
                case "connection":
                    MessagingCenter.Send(sender, "show connections");
                    break;
            }
        }
    }

}

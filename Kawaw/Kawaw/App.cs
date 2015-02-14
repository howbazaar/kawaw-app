using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    interface IApp
    {
        IRemoteSite Remote { get; }
        User User { get; set; }
    }

    public class App : Application, IApp
    {
        private User _user;
        private readonly RootViewModel _rootViewModel;
        // var accentColor = Color.FromHex("59C2FF");
        public static Color AccentColor = Color.FromHex("10558d");
        public static Color BackgroundColor = AccentColor.WithLuminosity(0.99);
        public static Color ForegroundColor = new Color(0.1);

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
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

            Remote = CreateRemoteSite();
            // Look to see if we have a logged in person.
            if (Properties.ContainsKey("User"))
            {
                User = Properties["User"] as User;
                // ReSharper disable once PossibleNullReferenceException
                Debug.WriteLine("User found in properties: {0}", User.FullName);
            }
            else
            {
                Debug.WriteLine("User not found in properties, login needed");
            }
            var page = RootViewModel.Profile;
            if (Properties.ContainsKey("Page") && User != null)
            {
                page = Properties["Page"] as string;
                Debug.WriteLine("Last page: {0}", page);
            }

            _rootViewModel = new RootViewModel(this);
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
                    new Setter {Property = ContentPage.BackgroundColorProperty, Value = BackgroundColor}
                }
            };
            Resources.Add("BaseViewStyle", contentPageStyle);
            var buttonStyle = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter {Property = Button.BackgroundColorProperty, Value = BackgroundColor.AddLuminosity(-0.05)},
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

        private RemoteSite CreateRemoteSite()
        {
            string csrftoken = null;
            string sessionid = null;
            string baseurl = null;
            if (Properties.ContainsKey("CSRFToken"))
                csrftoken = Properties["CSRFToken"] as string;
            if (Properties.ContainsKey("SessionId"))
                sessionid = Properties["SessionId"] as string;
            if (Properties.ContainsKey("BaseUrl"))
                baseurl = Properties["BaseUrl"] as string;

            if (string.IsNullOrEmpty(baseurl))
            {
                baseurl = "https://kawaw.com";
            }
            Debug.WriteLine("BaseUrl = '{0}', CSRFTkoen = '{1}', SessionID = '{2}'", baseurl, csrftoken, sessionid);
            var remote = new RemoteSite(baseurl, csrftoken, sessionid);

            remote.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "BaseUrl":
                        SetProperty("BaseUrl", remote.BaseUrl);
                        break;
                    case "CSRFToken":
                        SetProperty("CSRFToken", remote.CSRFToken);
                        break;
                    case "SessionId":
                        SetProperty("SessionId", remote.SessionId);
                        break;
                }
            };

            return remote;
        }

        private void SetProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.WriteLine("Removing property {0}", name);
                Properties.Remove(name);
            }
            else
            {
                Debug.WriteLine("Setting property {0}: '{1}'", name, value);
                Properties[name] = value;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (User != null)
                _rootViewModel.RefreshUser(true);
        }

        public IRemoteSite Remote { get; private set; }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                // should also work with nil...
                if (_user != null)
                {
                    Properties["User"] = _user;
                }
                else
                {
                    Properties.Remove("User");
                }
            }
        }
    }

}

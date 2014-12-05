using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xamarin.Forms;

namespace Kawaw
{
    class MyMainPage : ContentPage
    {
        public MyMainPage(UserProfile profile)
        {
            Title = "Connections";
            BackgroundColor = Color.Blue;
            BindingContext = profile;

            var list = new ListView
            {
                IsGroupingEnabled = true,
                RowHeight = 100,
            };

            //list.SetBinding(ListView.ItemsSourceProperty, "Data");

            Content = list;					
        }
    }

    class NavigationViewModel : BaseViewModel
    {
        private NavigationItem _selectedItem;

        public class NavigationItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public NavigationItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    MessagingCenter.Send(this, "show-page", _selectedItem.Name);
                    SelectedItem = null;
                }
            }
        }

        public IList Items { get; private set; }
        public NavigationViewModel()
        {
            Items = new ObservableCollection<object>
            {
                new NavigationItem{Name="Events", Description="events description"},
                new NavigationItem{Name="Connections", Description="connections description"},
                new NavigationItem{Name="Logout", Description="logout description"},
            };

        }
    }
    // This is the view that contains the buttons that take the user to the different
    // main page elements, connections, events, profile (master bit of the root view).
    class NavigationView : BaseView
    {
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

    class EventsView : BaseView
    {
        public EventsView()
        {
            Title = "Events";
            // Icon = "kawaw.png";
            Content = new Label { Text = "events view" };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () =>
            {
                Debug.WriteLine("logout");
            }, ToolbarItemOrder.Secondary));
        }
    }

    class ConnectionsView : BaseView
    {
        public ConnectionsView()
        {
            Title = "Connections";
            // Icon = "kawaw.png";
            Content = new Label { Text = "connections view" };
        }
    }

    class RootViewModel : BaseViewModel
    {
        private ViewModelNavigation _navigation;

        public override ViewModelNavigation Navigation
        {
            get { return _navigation; }
            set { _navigation = value; Init(); }
        }

        private void Init()
        {
            // check to see if logged in, and if not pushe the login page
            _navigation.PushModalAsync(new LoginViewModel());
        }

        public RootViewModel()
        {
            // not logged in so push the login page
            
        }
    }
    class RootView : MasterDetailPage
    {
        public RootView()
        {
            this.SetBinding(NavigationProperty, new Binding("Navigation", converter: new NavigationConverter()));
            this.SetBinding(IsBusyProperty, "IsBusy");
 
            var navigationModel = new NavigationViewModel();
            var eventModel = new EventsViewModel();
            Master = ViewModelNavigation.GetPageForViewModel(navigationModel);
            Detail = ViewModelNavigation.GetPageForViewModel(eventModel);

            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) =>
            {
                Debug.WriteLine("show-page: {0}", page);
            });
        }
    }

    class ConnectionsViewModel : BaseViewModel
    {
    }

    class EventsViewModel : BaseViewModel
    {
    }

    public class App : Application
    {
        public UserProfile Profile;

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
            ViewModelNavigation.Register<LoginViewModel, LoginView>();
            ViewModelNavigation.Register<RegisterViewModel, RegisterView>();
            ViewModelNavigation.Register<EventsViewModel, EventsView>();
            ViewModelNavigation.Register<ConnectionsViewModel, ConnectionsView>();
            ViewModelNavigation.Register<NavigationViewModel, NavigationView>();

            var rootModel = new RootViewModel();
            var rootView = new RootView {BindingContext = rootModel};

            Profile = GetProfile();
            
            // if not logged in, push the login page
//            master.Navigation.PushModalAsync(new NavigationPage(new LoginView()));

            MainPage = rootView;
        }

        private UserProfile GetProfile()
        {
            if (!Properties.ContainsKey("UserProfile"))
            {
                Properties["UserProfile"] = new UserProfile();
            }

            return (UserProfile)Properties["UserProfile"];
        }

        private void SetProfile()
        {
            Properties["UserProfile"] = Profile;
        }

        protected override void OnResume()
        {
            Profile.Note("OnResume");
            //Console.WriteLine("OnResume");
            base.OnResume();
        }

        protected override void OnSleep()
        {
            Profile.Note("OnSleep");
            //Console.WriteLine("OnSleep");
            base.OnSleep();
        }

        protected override void OnStart()
        {
            Profile.Note("OnStart");
            //Console.WriteLine("OnStart");
            base.OnStart();
        }

    }

}

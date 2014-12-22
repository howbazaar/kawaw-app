using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Kawaw
{
    interface IApp
    {
        IRemoteSite Remote { get; }
        RemoteUser User { get; set; }
    }

    public class App : Application, IApp
    {
        private RemoteUser _user;

        // chevron is the rootview.master.icon
        // android is the page icon
        public App()
        {
            ViewModelNavigation.Register<LoginViewModel, LoginView>();
            ViewModelNavigation.Register<RegisterViewModel, RegisterView>();
            ViewModelNavigation.Register<EventsViewModel, EventsView>();
            ViewModelNavigation.Register<ConnectionsViewModel, ConnectionsView>();
            ViewModelNavigation.Register<ProfileViewModel, ProfileView>();
            ViewModelNavigation.Register<ChangeDetailsViewModel, ChangeDetailsView>();
            ViewModelNavigation.Register<NavigationViewModel, NavigationView>();
            ViewModelNavigation.Register<AddEmailViewModel, AddEmailView>();

            // Look to see if we have a logged in person.
            string csrftoken = null;
            string sessionid = null;
            string page = RootViewModel.Profile;
            if (Properties.ContainsKey("User"))
            {
                User = Properties["User"] as RemoteUser;
                Debug.WriteLine("User found in properties: {0}", User.FullName);
                csrftoken = User.CSRFToken;
                sessionid = User.SessionId;
            }
            else
            {
                Debug.WriteLine("User not found in properties, login needed");
            }
            if (Properties.ContainsKey("Page"))
            {
                page = Properties["Page"] as string;
                Debug.WriteLine("Last page: {0}", page);
            }

            Remote = new RemoteSite(csrftoken, sessionid);

            var rootModel = new RootViewModel(this);
            var rootView = new RootView
            {
                BindingContext = rootModel,
                Master = ViewModelNavigation.GetPageForViewModel(rootModel.NavigationModel),
            };
            rootModel.SetDetails(page);

            MainPage = rootView;

            MessagingCenter.Subscribe(this, "show-page", (RootViewModel sender, BaseViewModel model) =>
            {
                if (model.Name != "")
                {
                    Properties["Page"] = model.Name;
                }
            });

        }
        
        protected override void OnResume()
        {
            base.OnResume();
            if (User != null)
                User.Refresh(Remote);
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public IRemoteSite Remote { get; private set; }

        public RemoteUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                // should also work with nil...
                if (_user != null)
                {
                    Properties["User"] = _user;
#if DEBUG
                    var x = new DataContractSerializer(typeof(RemoteUser));
                    var buf = new MemoryStream();
                    x.WriteObject(buf, _user);
                    buf.Seek(0, SeekOrigin.Begin);
                    var obj = x.ReadObject(buf);
                    var test = obj as RemoteUser;
                    Debug.WriteLine("Serialisation of user {0} passed", test.FullName);
#endif
                }
                else
                {
                    Properties.Remove("User");
                }
            }
        }
    }

}

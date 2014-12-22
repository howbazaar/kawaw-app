using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;
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

            Remote = CreateRemoteSite();
            // Look to see if we have a logged in person.
            string page = RootViewModel.Profile;
            if (Properties.ContainsKey("User"))
            {
                User = Properties["User"] as RemoteUser;
                Debug.WriteLine("User found in properties: {0}", User.FullName);
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
                if (!string.IsNullOrEmpty(model.Name))
                {
                    Properties["Page"] = model.Name;
                }
            });

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
                User.Refresh(Remote);
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

using System;
using System.Diagnostics;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Kawaw.JSON;
using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;
using Xamarin.Forms;

namespace Kawaw
{

    class RootViewModel : BaseViewModel
    {
        public const string Events = "Events";
        public const string Connections = "Connections";
        public const string Notifications = "Notifications";
        public const string Profile = "Profile";
        public const string Login = "Login";

        private ViewModelNavigation _navigation;
        private BaseViewModel _contentModel;

        public override ViewModelNavigation Navigation
        {
            get { return _navigation; }
            set { _navigation = value; Init(); }
        }

        public NavigationViewModel NavigationModel { get; private set; }

        private void Init()
        {
            if (App.User == null)
            {
                ShowLogin();
            }
            else
            {
                RefreshUser(true);
            }
        }

        public RootViewModel(IApp app) : base(app)
        {
            NavigationModel = new NavigationViewModel(app);

            // not logged in so push the login page
            MessagingCenter.Subscribe(this, "show-page", (NavigationViewModel sender, string page) => SetDetails(page));
            MessagingCenter.Subscribe(this, "logout", (object sender) =>
            {
                CrossPushNotification.Current.Unregister();

                App.Remote.Logout();
                App.User = null;
                ShowLogin();
            });
            MessagingCenter.Subscribe(this, "refresh", (object sender) => RefreshUser());
            MessagingCenter.Subscribe(this, "action-started", (object sender) =>
            {
                IsBusy = true;
                if (_contentModel != null)
                    _contentModel.IsBusy = true;
            });
            MessagingCenter.Subscribe(this, "action-stopped", (object sender) =>
            {
                IsBusy = false;
                if (_contentModel != null)
                {
                    _contentModel.IsBusy = false;
                    Debug.WriteLine("busy is false");                    
                }
            });
            MessagingCenter.Subscribe(this, "session-expired", (object sender) =>
            {
                    App.Remote.Logout();
                    App.User = null;
                    ShowLogin();
            });
            MessagingCenter.Subscribe(this, "user-refreshed", (object sender) =>
            {
                Debug.WriteLine("user-refreshed");
                var user = App.User;
                if (user == null)
                {
                    Debug.WriteLine("shouldn't be possible.");
                    return;
                }
                var appReg = App.DeviceRegistration;
                if (appReg == null)
                {
                    Debug.WriteLine("no device registration");
                    return;
                }
                if (appReg.UnregToken != "" || (appReg.Token != "" && !appReg.Registered))
                {
                    HandleDeviceRegistration(appReg, user);
                }
            });
            MessagingCenter.Subscribe(this, "unregister-device", async (object sender, DeviceType deviceType) =>
            {
                Debug.WriteLine("unregister-device");

                var appReg = App.DeviceRegistration;
                if (appReg == null || string.IsNullOrEmpty(appReg.Token))
                {
                    Debug.WriteLine("no existing registration to remove");
                    return;
                }
                var user = App.User;
                if (user != null)
                {
                    try
                    {
                        var success = await user.UnregisterDevice(appReg.Token, appReg.DeviceType);
                        if (success)
                        {
                            appReg.Token = null;
                            return;
                        }
                    }
                    catch
                    {
                        // store to remove it later, same as no user.
                    }
                }
                appReg.UnregToken = appReg.Token;
                appReg.Token = null;
            });
            MessagingCenter.Subscribe(this, "register-device", (object sender, DeviceRegistration registration) =>
            {
                Debug.WriteLine("register-device: {0}, {1}, {2}, {3}", registration.DeviceType, registration.Registered, registration.Token, registration.UnregToken);
                var appReg = App.DeviceRegistration;
                if (appReg == null)
                {
                    Debug.WriteLine("set appReg = registration");
                    appReg = registration;
                }
                else
                {
                    if (!string.IsNullOrEmpty(appReg.Token))
                    {
                        if (appReg.Token == registration.Token)
                        {
                            Debug.WriteLine("Same token as last time.");
                            return;
                        }
                        appReg.UnregToken = appReg.Token;
                        appReg.Registered = false;
                    }
                    appReg.Token = registration.Token;
                    appReg.DeviceType = registration.DeviceType;
                }
                App.DeviceRegistration = appReg;
                if (App.User != null)
                {
                    Debug.WriteLine("Have App.User");
                    HandleDeviceRegistration(appReg, App.User);
                }
            });
        }

        private static async void HandleDeviceRegistration(DeviceRegistration appReg, Models.User user)
        {
            Debug.WriteLine("handle device registration");
            if (!string.IsNullOrEmpty(appReg.UnregToken))
            {
                Debug.WriteLine("unregister {0}", appReg.UnregToken);
                try
                {
                    var success = await user.UnregisterDevice(appReg.UnregToken, appReg.DeviceType);
                    if (success)
                    {
                        appReg.Token = null;
                    }
                }
                catch
                {
                    // store to remove it later, same as no user.
                }
            }
            if (appReg.Registered || string.IsNullOrEmpty(appReg.Token)) return;
            try
            {
                appReg.Registered = await user.RegisterDevice(appReg.Token, appReg.DeviceType);
            }
            catch
            {
                // store to remove it later, same as no user.
            }
        }
        public async void RefreshUser(bool silent = false)
        {
            try
            {
                App.User.Remote = App.Remote;
                await App.User.Refresh();
            }
            catch (SessionExpiredException)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Session Expired",
                    Text = "Your session has expired. Please log in again.",
                    Callback = new Command(() => MessagingCenter.Send((object)this, "session-expired")),
                });
            }
            catch (NetworkDownException ne)
            {
                if (!silent)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Refresh Failed",
                        Text = ne.Message
                    });
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(this, "alert", new Alert
                {
                    Title = "Refresh Failed",
#if DEBUG
                    Text = e.Message
#else
                    Text = "Something went wrong, please try again later"
#endif
                });
            }            
        }


        public void SetDetails(string page)
        {
            _contentModel = CreateViewModel(page);
            _contentModel.IsBusy = IsBusy;
            MessagingCenter.Send(this, "show-page", _contentModel);
        }

        private async void ShowLogin()
        {
            SetDetails(Profile);
            var loginModel = new LoginViewModel(App);
            await Navigation.PushLoginAsync(loginModel);
        }

        private BaseViewModel CreateViewModel(string name)
        {
            switch (name)
            {
                case Events:
                    return new EventsViewModel(App);
                case Connections:
                    return new ConnectionsViewModel(App);
                case Notifications:
                    return new NotificationsViewModel(App);
                case Profile:
                    return new ProfileViewModel(App);
                case Login:
                    return new LoginViewModel(App);
            }
            return new ProfileViewModel(App);
            // throw new Exception("Unknown model name " + name);
        }

    }
}
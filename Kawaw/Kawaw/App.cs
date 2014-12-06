using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kawaw
{
    namespace JSON
    {
        [DataContract]
        public class User
        {
            [DataMember(Name = "date_of_birth")]
            public string DateOfBirth { get; set; }

            [DataMember(Name = "first_name")]
            public string FirstName { get; set; }

            [DataMember(Name = "last_name")]
            public string LastName { get; set; }

            [DataMember(Name = "token")]
            public string CSRFToken { get; set; }

            [DataMember(Name = "full_name")]
            public string FullName { get; set; }

            [DataMember(Name = "address")]
            public string Address { get; set; }

            // There is a social thing, but need to remember what it does.
            [DataMember(Name = "email")]
            public string PrimaryEmail { get; set; }

            [DataMember(Name = "emails")]
            public Email[] Emails { get; set; }
        }

        [DataContract]
        public class Email
        {
            [DataMember(Name = "verified")]
            public bool Verified { get; set; }

            [DataMember(Name = "email")]
            public string Address { get; set; }

            [DataMember(Name = "primary")]
            public bool Primary { get; set; }
        }
    }


    interface IRemoteSite
    {
        Task<bool> Login(string username, string password);


    }

    class RemoteSite : IRemoteSite
    {
        private string _remote;
        private HttpClient _client;
        private string _csrf_token;
        private CookieContainer _cookies;
        public RemoteSite()
        {
            _cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = _cookies;

            // _remote = "https://kawaw.com";
            _remote = "http://192.168.1.7:8080";
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(_remote);
            // Fake it for the all auth plugin.
            _client.DefaultRequestHeaders.Add("X_REQUESTED_WITH", "XMLHttpRequest");
        }

        private async Task<string> GetCSRFToken()
        {
            if (_csrf_token != null)
                return _csrf_token;

            await Get("+login-form/");

            var cookie = _cookies.GetCookies(new Uri(_remote))["csrftoken"];
            _csrf_token=cookie.Value;
            return _csrf_token;
        }

        private async Task<HttpResponseMessage> Get(string path)
        {
            var response = await _client.GetAsync(path);
            return response;
        }

        private async Task<HttpResponseMessage> Post(string path, Dictionary<string, string> formAttributes)
        {
            var csrfToken = await GetCSRFToken();
            var content = new FormUrlEncodedContent(formAttributes);
            content.Headers.Add("X-CSRFToken", csrfToken);
            //content.Headers.Add("HTTP_X_REQUESTED_WITH", "XMLHttpRequest");
            var response = await _client.PostAsync(path, content);
            return response;
        }

        public async Task<JSON.User> GetUserDetails()
        {
            var response = await Get("+user/");
            var jsonSerializer = new DataContractJsonSerializer(typeof(JSON.User));
            // TODO: throw a known error for Forbidden.
            var stream = await response.Content.ReadAsStreamAsync();
            var objResponse = jsonSerializer.ReadObject(stream);
            return objResponse as JSON.User;
        }

        public async Task<bool> Login(string username, string password)
        {
            var values = new Dictionary<string, string>();
            values["login"] = username;
            values["password"] = password;
            try
            {
                var response = await Post("accounts/login/", values);
                Debug.WriteLine(response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
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
            base.OnResume();
        }

        protected override void OnSleep()
        {
            Profile.Note("OnSleep");
            base.OnSleep();
        }

        protected override void OnStart()
        {
            Profile.Note("OnStart");
            base.OnStart();
        }

    }

}

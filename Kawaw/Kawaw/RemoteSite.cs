using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Kawaw.Database;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using User = Kawaw.Models.User;

namespace Kawaw
{
    public class RemoteSite : BaseProperties
    {
        // ReSharper disable once InconsistentNaming
        public string CSRFToken
        {
            get { return _csrfToken; }
            private set { SetProperty(ref _csrfToken, value); }
        }

        public string SessionId
        {
            get { return _sessionId; }
            private set { SetProperty(ref _sessionId, value); }
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            private set { SetProperty(ref _baseUrl, value); }
        }

        private HttpClient _client;
        private CookieContainer _cookies;
        private string _csrfToken;
        private string _sessionId;
        private string _baseUrl;

        public RemoteSite(string baseUrl, string token, string sessionId)
        {
            BaseUrl = baseUrl;
            CSRFToken = token;
            SessionId = sessionId;
            CreateClient();

            MessagingCenter.Subscribe(this, "remote-baseurl-change",
                (object sender, string baseurl) =>
                {
                    CSRFToken = null;
                    SessionId = null;
                    BaseUrl = baseUrl;
                    CreateClient();
                });

        }

        private void CreateClient()
        {
            _cookies = new CookieContainer();
            var uri = new Uri(BaseUrl);
            if (!string.IsNullOrEmpty(SessionId))
            {
                _cookies.Add(uri, new Cookie("csrftoken", CSRFToken));
                _cookies.Add(uri, new Cookie("sessionid", SessionId));
            }

            var handler = new HttpClientHandler { CookieContainer = _cookies };
            _client = new HttpClient(handler) { BaseAddress = uri };
            // Fake it for the all auth plugin.
            _client.DefaultRequestHeaders.Add("X-REQUESTED-WITH", "XMLHttpRequest");
            _client.DefaultRequestHeaders.Add("REFERER", BaseUrl);
        }

        // ReSharper disable once InconsistentNaming
        private async Task<string> GetCSRFToken()
        {
            await Get("+login-form/").ConfigureAwait(false);
            var cookie = _cookies.GetCookies(new Uri(BaseUrl))["csrftoken"];
            return cookie.Value;
        }

        private async Task<HttpResponseMessage> Get(string path)
        {
            try
            {
                return await _client.GetAsync(path).ConfigureAwait(false);
            }
            catch (WebException)
            {
                throw new NetworkDownException();
            }
        }

        private async Task<HttpResponseMessage> Post(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(CSRFToken))
                {
                    CSRFToken = await GetCSRFToken();
                }
                // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
                // this it will come back to the thread that initially called await, which will always be the ui thread
                var content = new ByteArrayContent(new byte[0]);
                content.Headers.Add("X-CSRFToken", CSRFToken);
                return await _client.PostAsync(path, content).ConfigureAwait(false);
            }
            catch (WebException)
            {
                throw new NetworkDownException();
            }
        }

        private async Task<HttpResponseMessage> Post(string path, Dictionary<string, string> formAttributes)
        {
            if (string.IsNullOrEmpty(CSRFToken))
            {
                CSRFToken = await GetCSRFToken();
            }
            // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
            // this it will come back to the thread that initially called await, which will always be the ui thread
            var cookies = _cookies.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine(cookie.ToString());
            }

            var content = new FormUrlEncodedContent(formAttributes);
            content.Headers.Add("X-CSRFToken", CSRFToken);
            return await _client.PostAsync(path, content).ConfigureAwait(false);
        }

        private static async Task<TResponse> ReadFromResponse<TResponse>(HttpResponseMessage response)
            where TResponse : class
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(TResponse));
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var objResponse = jsonSerializer.ReadObject(stream);
            return objResponse as TResponse;
        }

        private static async Task<TResponse[]> ReadArrayFromResponse<TResponse>(HttpResponseMessage response)
            where TResponse : class
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<TResponse>));
#if DEBUG
            // ReSharper disable once UnusedVariable
            var content = await response.Content.ReadAsStringAsync();
#endif
            var stream = await response.Content.ReadAsStreamAsync();
            var objResponse = jsonSerializer.ReadObject(stream);
            var list = objResponse as List<TResponse>;
            return list != null ? list.ToArray() : null;
        }

        private static async Task<JSON.User> ReadUserFromContent(HttpResponseMessage response)
        {
            return await ReadFromResponse<JSON.User>(response).ConfigureAwait(false);
        }

        public async Task<JSON.User> GetUserDetails()
        {
            var response = await Get("+user/").ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await ReadUserFromContent(response).ConfigureAwait(false);
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SessionExpiredException();
            throw new UnexpectedStatusException(response.StatusCode);
        }

        private async Task<TResponse[]> GetArrayType<TResponse>(string url)
            where TResponse : class
        {
            var response = await Get(url).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await ReadArrayFromResponse<TResponse>(response).ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SessionExpiredException();
            throw new UnexpectedStatusException(response.StatusCode);
        }

        public Task<JSON.Event[]> GetEvents()
        {
            return GetArrayType<JSON.Event>("+events/");
        }

        public Task<JSON.Connection[]> GetConnections()
        {
            return GetArrayType<JSON.Connection>("+connections/");
        }

        public Task<JSON.Notification[]> GetNotifications()
        {
            return GetArrayType<JSON.Notification>("+notifications/");
        }

        private void SetValuesFromCookies()
        {
            var cookies = _cookies.GetCookies(new Uri(BaseUrl));
#if DEBUG
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine("{0}: '{1}'", cookie.Name, cookie.Value);
            }
#endif
            CSRFToken = cookies["csrftoken"].Value;
            SessionId = cookies["sessionid"].Value;

            MessagingCenter.Send<object, Remote>(this, "remote-session-change", new Remote
            {
                CSRFToken = CSRFToken,
                SessionId = SessionId
            });
        }

        public async Task Login(string username, string password)
        {
            var values = new Dictionary<string, string>();
            values["login"] = username;
            values["password"] = password;
            values["remember"] = "True";
            var response = await Post("accounts/login/", values);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                SetValuesFromCookies();
                return;
            }
            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task Register(string email, string password)
        {
            var values = new Dictionary<string, string>();
            values["email"] = email;
            values["password1"] = password;
            values["password2"] = password;
            var response = await Post("accounts/signup/", values);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                SetValuesFromCookies();
                return;
            }

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task Logout()
        {
            // Try to logout and clear the session, but if it doesn't work, no biggie.
            try
            {
                await Post("accounts/logout/").ConfigureAwait(false);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // swallow the exception, there is not much we can do here.
            }
            SessionId = null;
            CSRFToken = null;
            var cookies = _cookies.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
            MessagingCenter.Send<object, Remote>(this, "remote-session-change", new Remote());
        }

        public async Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address,
            DateTime dateOfBirth)
        {
            var values = new Dictionary<string, string>();
            values["first_name"] = firstName;
            values["last_name"] = lastName;
            values["address"] = address;
            if (dateOfBirth == new DateTime(0) || dateOfBirth == User.MinDateOfBirthValue)
            {
                values["date_of_birth"] = "";
            }
            else
            {
                values["date_of_birth"] = dateOfBirth.ToString("yyyy-MM-dd");
            }
            var response = await Post("+update-details/", values).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
                return await ReadUserFromContent(response).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SessionExpiredException();

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        [DataContract]
        public class EmailActionResponse
        {
            [DataMember(Name = "user")]
            public JSON.User User { get; set; }
        }

        public async Task<JSON.User> AddEmail(string address)
        {
            var values = new Dictionary<string, string>();
            values["email"] = address;
            var response = await Post("+add-email/", values).ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await ReadUserFromContent(response).ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SessionExpiredException();

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task<JSON.User> EmailAction(string action, string address)
        {
            var values = new Dictionary<string, string>();
            values["action"] = action;
            values["email"] = address;
            var response = await Post("+email-action/", values).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var emailResponse = await ReadFromResponse<EmailActionResponse>(response).ConfigureAwait(false);
                return emailResponse.User;
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new SessionExpiredException();

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task NotificationAction(uint notificationId, uint memberId, bool accepted)
        {
            var values = new Dictionary<string, string>();
            values["id"] = memberId.ToString();
            values["accepted"] = accepted ? "True" : "False";
            var url = "+registration-response/" + notificationId + "/";
            var response = await Post(url, values).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new InconsistentStateException();

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task<JSON.Connection> ConnectionAction(uint id, bool accept)
        {
            var values = new Dictionary<string, string>();
            values["accepted"] = accept ? "True" : "False";
            var url = string.Format("+update-connection/{0}/", id);
            var response = await Post(url, values).ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await ReadFromResponse<JSON.Connection>(response);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new InconsistentStateException();

            await ProcessFormError(response);
            throw new UnexpectedException("unreachable");
        }

        public async Task<bool> RegisterDevice(string token)
        {
            Debug.WriteLine("RemoteSite::RegisterDevice");

            var values = new Dictionary<string, string>();
            values["token"] = token;
            values["device"] = Device.OnPlatform("a", "g", "w");
            var response = await Post("+add-endpoint/", values);
#if DEBUG
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Add endpoint: {0}", content);
#endif
            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> UnregisterDevice(string token)
        {
            Debug.WriteLine("RemoteSite::UnregisterDevice");
            var values = new Dictionary<string, string>();
            values["token"] = token;
            values["device"] = Device.OnPlatform("a", "g", "w");
            var response = await Post("+remove-endpoint/", values);
#if DEBUG
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Remove endpoint: {0}", content);
#endif
            return response.StatusCode == HttpStatusCode.OK;
        }

        private static async Task ProcessFormError(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.BadRequest)
                throw new UnexpectedStatusException(response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(content);
            var errors = parsed["form_errors"];
            if (errors != null)
            {
                throw new FormErrorsException(errors.Value<JObject>());
            }
            throw new UnexpectedException(content);
        }
    }
}
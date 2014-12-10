using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Kawaw
{
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

        private Task<HttpResponseMessage> Get(string path)
        {
            return _client.GetAsync(path);
        }

        private async Task<HttpResponseMessage> Post(string path)
        {
            // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
            // this it will come back to the thread that initially called await, which will always be the ui thread
            var csrfToken = await GetCSRFToken().ConfigureAwait(false);
            var content = new ByteArrayContent(new byte[0]);
            content.Headers.Add("X-CSRFToken", csrfToken);
            return await _client.PostAsync(path, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> Post(string path, Dictionary<string, string> formAttributes)
        {
            // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
            // this it will come back to the thread that initially called await, which will always be the ui thread
            var csrfToken = await GetCSRFToken().ConfigureAwait(false);
            var content = new FormUrlEncodedContent(formAttributes);
            content.Headers.Add("X-CSRFToken", csrfToken);
            return await _client.PostAsync(path, content).ConfigureAwait(false);
        }

        public async Task<JSON.User> GetUserDetails()
        {
            var response = await Get("+user/").ConfigureAwait(false);
            var jsonSerializer = new DataContractJsonSerializer(typeof(JSON.User));
            // TODO: throw a known error for Forbidden.
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var objResponse = jsonSerializer.ReadObject(stream);
            return objResponse as JSON.User;
        }

        public async Task<bool> Login(string username, string password)
        {
            var values = new Dictionary<string, string>();
            values["login"] = username;
            values["password"] = password;
            values["remember"] = "True";
            try
            {
                var response = await Post("accounts/login/", values).ConfigureAwait(false);
                Debug.WriteLine(response.StatusCode);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Debug.WriteLine(content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public async void Logout()
        {
            var response = await Post("accounts/logout/").ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(response.IsSuccessStatusCode);
        }
    }
}
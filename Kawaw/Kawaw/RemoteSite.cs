﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Kawaw.JSON;
using Xamarin.Forms;

namespace Kawaw
{
    class RemoteSite : IRemoteSite
    {
        public string CSRFToken { get; private set; }
        public string SessionId { get; private set; }

        private string _remote;
        private HttpClient _client;
        private CookieContainer _cookies;

        public RemoteSite(string token, string sessionId)
        {
            CSRFToken = token;
            SessionId = sessionId;

            // _remote = "https://kawaw.com";
            _remote = "http://192.168.1.7:8080";
            _cookies = new CookieContainer();
            var uri = new Uri(_remote);
            if (!string.IsNullOrEmpty(SessionId))
            {
                _cookies.Add(uri, new Cookie("csrftoken", CSRFToken));
                _cookies.Add(uri, new Cookie("sessionid", SessionId));
            }

            var handler = new HttpClientHandler {CookieContainer = _cookies};
            _client = new HttpClient(handler) {BaseAddress = uri};
            // Fake it for the all auth plugin.
            _client.DefaultRequestHeaders.Add("X_REQUESTED_WITH", "XMLHttpRequest");
        }

        private async Task<string> GetCSRFToken()
        {
            await Get("+login-form/").ConfigureAwait(false);
            var cookie = _cookies.GetCookies(new Uri(_remote))["csrftoken"];
            return cookie.Value;
        }

        private Task<HttpResponseMessage> Get(string path)
        {
            return _client.GetAsync(path);
        }

        private async Task<HttpResponseMessage> Post(string path)
        {
            // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
            // this it will come back to the thread that initially called await, which will always be the ui thread
            var content = new ByteArrayContent(new byte[0]);
            content.Headers.Add("X-CSRFToken", CSRFToken);
            return await _client.PostAsync(path, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> Post(string path, Dictionary<string, string> formAttributes)
        {
            // ConfigureAwait(false) says just run on the thread you came back on, if we don't do
            // this it will come back to the thread that initially called await, which will always be the ui thread
            var cookies = _cookies.GetCookies(new Uri(_remote));
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine(cookie.ToString());
            }

            var content = new FormUrlEncodedContent(formAttributes);
            content.Headers.Add("X-CSRFToken", CSRFToken);
            return await _client.PostAsync(path, content).ConfigureAwait(false);
        }

        private async Task<JSON.User> readUserFromContent(HttpResponseMessage response)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(JSON.User));
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var objResponse = jsonSerializer.ReadObject(stream);
            return objResponse as JSON.User;
        }

        public async Task<JSON.User> GetUserDetails()
        {
            var response = await Get("+user/").ConfigureAwait(false);
            // TODO: throw a known error for Forbidden.
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("not ok... sort it out");
            }
            return await readUserFromContent(response).ConfigureAwait(false);
        }

        public async Task<bool> Login(string username, string password)
        {
            CSRFToken = await GetCSRFToken();
            var values = new Dictionary<string, string>();
            values["login"] = username;
            values["password"] = password;
            values["remember"] = "True";
            try
            {
                var response = await Post("accounts/login/", values).ConfigureAwait(false);
                Debug.WriteLine(response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                // TODO: handle different error codes.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var cookies = _cookies.GetCookies(new Uri(_remote));
                    CSRFToken = cookies["csrftoken"].Value;
                    SessionId = cookies["sessionid"].Value;
                }
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
            SessionId = null;
            CSRFToken = null;
            var cookies = _cookies.GetCookies(new Uri(_remote));
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        public async Task<User> UpdateUserDetails(string firstName, string lastName, string address,
            DateTime dateOfBirth)
        {
            var values = new Dictionary<string, string>();
            values["first_name"] = firstName;
            values["last_name"] = lastName;
            values["address"] = address;
            values["date_of_birth"] = dateOfBirth.ToString("yyyy-MM-dd");
            var response = await Post("+update-details/", values).ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            // TODO: on 404 throw site down....
            //       on 403 unauthorized - need to login again
            //       on 400 extract json error from content
            //       on 200 extract user from json
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("not ok... sort it out");
            }
            return await readUserFromContent(response).ConfigureAwait(false);
        }
    }
}
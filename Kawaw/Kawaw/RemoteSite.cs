﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Kawaw
{
    public class RemoteSite : BaseProperties, IRemoteSite
    {
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

            // _remote = "https://kawaw.com";
            // _remote = "http://192.168.1.7:8080";
            CreateClient();
        }

        public void SetBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
            CSRFToken = null;
            SessionId = null;
            CreateClient();
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
        }

        private async Task<string> GetCSRFToken()
        {
            await Get("+login-form/").ConfigureAwait(false);
            var cookie = _cookies.GetCookies(new Uri(BaseUrl))["csrftoken"];
            return cookie.Value;
        }

        private Task<HttpResponseMessage> Get(string path)
        {
            return _client.GetAsync(path);
        }

        private async Task<HttpResponseMessage> Post(string path)
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

        private async Task<TResponse> readFromResponse<TResponse>(HttpResponseMessage response)
            where TResponse : class
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(TResponse));
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var objResponse = jsonSerializer.ReadObject(stream);
            return objResponse as TResponse;
        }

        private async Task<TResponse[]> readArrayFromResponse<TResponse>(HttpResponseMessage response)
            where TResponse : class
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<TResponse>));
            var content = await response.Content.ReadAsStringAsync();
            var stream = await response.Content.ReadAsStreamAsync(); // .ConfigureAwait(false);
            try
            {
                var objResponse = jsonSerializer.ReadObject(stream);
                var list = objResponse as List<TResponse>;
                if (list != null) return list.ToArray();
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        private async Task<JSON.User> readUserFromContent(HttpResponseMessage response)
        {
            return await readFromResponse<JSON.User>(response).ConfigureAwait(false);
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

        private void SetValuesFromCookies()
        {
            var cookies = _cookies.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine("{0}: '{1}'", cookie.Name, cookie.Value);
            }
            CSRFToken = cookies["csrftoken"].Value;
            SessionId = cookies["sessionid"].Value;
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
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                // TODO: handle different error codes.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SetValuesFromCookies();
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public async Task<bool> Register(string username, string email, string password, string password2)
        {
            CSRFToken = await GetCSRFToken();
            var values = new Dictionary<string, string>();
            //values["name"] = username;
            values["email"] = email;
            values["password1"] = password;
            values["password2"] = password2;
            try
            {
                var response = await Post("accounts/signup/", values).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync();
                // TODO: handle different error codes.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SetValuesFromCookies();
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
            var cookies = _cookies.GetCookies(new Uri(BaseUrl));
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        public async Task<JSON.User> UpdateUserDetails(string firstName, string lastName, string address,
            DateTime dateOfBirth)
        {
            var values = new Dictionary<string, string>();
            values["first_name"] = firstName;
            values["last_name"] = lastName;
            values["address"] = address;
            if (dateOfBirth == new DateTime(0) || dateOfBirth == RemoteUser.MinDateOfBirthValue)
            {
                values["date_of_birth"] = "";
            }
            else
            {
                values["date_of_birth"] = dateOfBirth.ToString("yyyy-MM-dd");
            }
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
            // TODO: on 404 throw site down....
            //       on 403 unauthorized - need to login again
            //       on 400 extract json error from content
            //       on 200 extract user from json
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await readUserFromContent(response).ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // get the message out...
                // raise a nicer exception
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                // raise login required
            }
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("Response: {0}\nContent: {1}", response.StatusCode, content);
            throw new Exception("not ok... sort it out");
            
        }

        public async Task<JSON.User> EmailAction(string action, string address)
        {
            var values = new Dictionary<string, string>();
            values["action"] = action;
            values["email"] = address;
            var response = await Post("+email-action/", values).ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            // TODO: on 404 throw site down....
            //       on 403 unauthorized - need to login again
            //       on 400 extract json error from content
            //       on 200 extract user from json
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var emailResponse = await readFromResponse<EmailActionResponse>(response).ConfigureAwait(false);
                return emailResponse.User;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // get the message out...
                // raise a nicer exception
            } else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                // raise login required
            }
            throw new Exception("not ok... sort it out");
        }

        public async Task<JSON.Connection[]> GetConnections()
        {
            var response = await Get("+connections/").ConfigureAwait(false);
            // TODO: throw a known error for Forbidden.
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("not ok... sort it out");
            }
            var connections = await readArrayFromResponse<JSON.Connection>(response).ConfigureAwait(false);
            return connections;
        }

        public async Task<JSON.Connection> ConnectionAction(uint id, bool accept)
        {
            var values = new Dictionary<string, string>();
            values["accepted"] = accept ? "True" : "False";
            var url = string.Format("+update-connection/{0}/", id);
            var response = await Post(url, values).ConfigureAwait(false);
            Debug.WriteLine(response.StatusCode);
            // TODO: on 404 throw site down....
            //       on 403 unauthorized - need to login again
            //       on 400 extract json error from content
            //       on 200 extract connection from json
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await readFromResponse<JSON.Connection>(response);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // get the message out...
                // raise a nicer exception
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                // raise login required
            }
            throw new Exception("not ok... sort it out: rc " + response.StatusCode);
            
        }
    }
}
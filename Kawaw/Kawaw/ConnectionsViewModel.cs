using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    struct ConnectionAction
    {
        public Connection Connection;
        public string Name;
    }

    struct ConnectionActionOptions
    {
        public Connection Connection;
        public List<Tuple<string, string>> Options;
    }

    class ConnectionsViewModel : BaseViewModel
    {
        private IList<Connection> _connections;
        private Connection _selectedItem;
        private string _emptyText;

        public double EmptyOpacity { get { return Connections.Count == 0 ? 1.0 : 0.0;  } }
        public double ListOpacity { get { return Connections.Count > 0 ? 1.0 : 0.0; } }

        public string EmptyText
        {
            get { return _emptyText; }
            private set { SetProperty(ref _emptyText, value); }
        }

        public IList<Connection> Connections
        {
            get { return _connections; }
            private set { SetProperty(ref _connections, value); }
        }

        public Connection SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var changed = SetProperty(ref _selectedItem, value);
                if (value == null || !changed) return;

                SelectedItem = null;

                var options = new ConnectionActionOptions
                {
                    Connection= value,
                    Options = new List<Tuple<string, string>>(),
                };
                if (value.Accepted || value.Pending)
                {
                    options.Options.Add(new Tuple<string, string>("reject", "Reject connection"));
                }
                if (!value.Accepted || value.Pending)
                {
                    options.Options.Add(new Tuple<string, string>("accept", "Accept connection"));
                }
                MessagingCenter.Send(this, "show-options", options);
            }
        }

        public ConnectionsViewModel(User user)
            : base(user, RootViewModel.Connections)
        {
            Connections = new ObservableCollection<Connection>();
            UpdateFromUser(User);

            MessagingCenter.Subscribe<User>(this, "initialized", delegate
            {
                Debug.WriteLine("Update because user initialized");
                if (IsPageVisible)
                {
                    UpdateFromUser(User);
                }
            });
            MessagingCenter.Subscribe<object>(this, "connections-updated", delegate
            {
                if (IsPageVisible)
                {
                    UpdateFromUser(User);
                }
            });
            MessagingCenter.Subscribe(this, "connection-action", async (object sender, ConnectionAction action) =>
            {
                try
                {
                    Debug.WriteLine("{0} for {1}", action.Name, action.Connection.Name);
                    await User.ConnectionAction(action.Connection, action.Name == "accept");
                }
                catch (InconsistentStateException)
                {
                    MessagingCenter.Send((object)this, "refresh");
                }
                catch (Exception e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Update Failed",
                        Text = e.Message
                    });
                }
            });
        }

        private async void UpdateFromUser(User user)
        {
            if (!user.Initialized || !user.Authenticated)
            {
                Debug.WriteLine("ConnectionViewModel.UpdateFromUser, skipping due to initialized = {0}, authenticated = {1}", user.Initialized, user.Authenticated);
                Connections.Clear();
                return;
            }

            var connections = await user.Connections();

            var updated = new ObservableCollection<Connection>();
            foreach (var connection in connections.OrderByDescending(connection => connection.Pending)
                    .ThenByDescending(connection => connection.Accepted)
                    .ThenBy(connection => connection.Organisation)
                    .ThenBy(connection => connection.Name))
            {
                updated.Add(connection);
            }

            var empty = Connections.Count == 0;
            if (!Connections.SequenceEqual(updated))
            {
                Connections.Clear();
                foreach (var connection in updated)
                {
                    Connections.Add(connection);
                }
            }

            if (user.HasVerifiedEmail)
            {
                EmptyText =
                    "No connections yet.\n\n" +
                    "As your verified email addresses are added as contact email addresses for people in " +
                    "shools, clubs, or other organisations that use kawaw, connections will show up here.";
            }
            else
            {
                EmptyText =
                    "Connections are only made using verified email addreses.\n\n" +
                    "To see any existing connections you need to verify your email " +
                    "addresses by clicking on the link in the email sent to that address.";
            }

            if (empty == (Connections.Count == 0)) return;

            OnPropertyChanged("EmptyOpacity");
            OnPropertyChanged("ListOpacity");
        }

    }
}
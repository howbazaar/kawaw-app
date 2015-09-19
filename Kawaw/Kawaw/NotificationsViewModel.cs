using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    struct NotificationResponseAction
    {
        public NotificationResponse Response;
        public string Action;
    }

    struct NotificationResponseOptions
    {
        public NotificationResponse Response;
        public List<Tuple<string, string>> Options;
    }


    class NotificationResponseViewModel : BaseProperties
    {
        private readonly NotificationResponse _response;
        public Command TapCommand { get; private set; }

        public NotificationResponseViewModel(NotificationResponse response)
        {
            _response = response;
            TapCommand = new Command(() =>
            {
                var options = new NotificationResponseOptions
                {
                    Response = response,
                    Options = new List<Tuple<string, string>>(),
                };
                if (response.Choice != true)
                {
                    options.Options.Add(new Tuple<string, string>("yes", "Yes register"));
                }
                if (response.Choice != false)
                {
                    options.Options.Add(new Tuple<string, string>("no", "No thanks"));
                }
                MessagingCenter.Send(this, "show-options", options);
            });
        }

        public string Status
        {
            get
            {
                if (_response.Choice == null)
                    return "pending";
                return _response.Choice == true ? "registered" : "not interested";
            }
        }
        public uint Id { get { return _response.Id; } }
        public Color StatusColor { get { return Pending ? App.AccentColor : App.ForegroundColor;  } }
        public bool Pending { get { return _response.Choice == null; } }
        public string Name { get { return _response.Name;  }}

        public void SetChoice(bool choice)
        {
            _response.Choice = choice;
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusColor");
        }
    }

    class NotificationViewModel : BaseProperties
    {
        private readonly Notification _notification;

        public NotificationViewModel(Notification note)
        {
            _notification= note;
            Responses = new ObservableCollection<NotificationResponseViewModel>(
                   from response in _notification.Responses
                   orderby response.Name
                   select new NotificationResponseViewModel(response));
        }

        public Color BackgroundColor { get { return _notification.Pending ? App.AccentColor : Color.FromHex("cccccc");  } }
        public Color ForegroundColor { get { return _notification.Pending ? Color.White : App.ForegroundColor; } }
        public string Type { get { return (_notification.Pending ? "Unactioned " : "Completed ") + _notification.Type; } }

        public string Activity
        {
            get
            {
                var result = _notification.Organisation;
                if (!String.IsNullOrEmpty(_notification.Session))
                {
                    result = _notification.Session + " - " + result;
                }
                if (!String.IsNullOrEmpty(_notification.Activity))
                {
                    result = _notification.Activity + " - " + result;
                }
                return result;
            }
        }

        public uint Id { get { return _notification.Id; } }
        public string Description { get { return _notification.Description; } }
        public bool DescriptionVisible { get { return !string.IsNullOrEmpty(_notification.Description);  } }
        public IList<NotificationResponseViewModel> Responses { get; private set; }

        public void SetMemberResponse(uint memberId, bool accepted)
        {
            var response = Responses.Single(r => r.Id == memberId);
            response.SetChoice(accepted);
            // Update current pending, and notifiy change.
            var pending = _notification.Responses.Any(r => r.Choice == null);
            if (pending) Debug.WriteLine("still some pending");
            _notification.Pending = pending;
            OnPropertyChanged("Type");
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("ForegroundColor");
        }

    }

    class NotificationsViewModel : BaseViewModel
    {
        private IList<NotificationViewModel> _notifications;

        public double EmptyOpacity { get { return Notifications.Count == 0 ? 1.0 : 0.0; } }
        public double ListOpacity { get { return Notifications.Count > 0 ? 1.0 : 0.0; } }

        public string EmptyText { get; private set; }

        public IList<NotificationViewModel> Notifications
        {
            get { return _notifications; }
            private set { SetProperty(ref _notifications, value); }
        }

        public NotificationsViewModel(User user)
            : base(user, RootViewModel.Notifications)
        {
            Notifications = new ObservableCollection<NotificationViewModel>();
            UpdateFromUser(User, false);

            MessagingCenter.Subscribe<User>(this, "initialized", obj => UpdateFromUser(User, true));
            MessagingCenter.Subscribe<object>(this, "notifications-updated", obj => UpdateFromUser(User, true));
            MessagingCenter.Subscribe(this, "notification-action", async (object sender, NotificationResponseAction action) =>
            {
                try
                {
                    var memberId = action.Response.Id;
                    var notifiationId = action.Response.Notification.Id;
                    var accepted = action.Action == "yes";
                    await User.NotificationAction(notifiationId, memberId, accepted);

                    var viewModel = _notifications.Single(n => n.Id == notifiationId);
                    viewModel.SetMemberResponse(memberId, accepted);
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

        private async void UpdateFromUser(User user, bool checkVisible)
        {
            if (checkVisible && !IsPageVisible) return;

            if (!user.Initialized || !user.Authenticated)
            {
                Notifications.Clear();
                return;
            }

            var notifications = await user.Notifications();

            var empty = Notifications.Count == 0;
            Notifications.Clear();
            foreach (var note in notifications.OrderByDescending(note => note.Pending)
                    .ThenBy(note => note.ClosingDate)
                    .ThenBy(note => note.Organisation)
                    .ThenBy(note => note.Activity))
            {
                Notifications.Add(new NotificationViewModel(note));
            }

            if (user.HasVerifiedEmail)
            {
                EmptyText =
                    "No current notifications.\n\n" +
                    "As notifications for registration are added by your associated " +
                    "shools, clubs, or other organisations they will show up here.";
            }
            else
            {
                EmptyText =
                    "All associations to schools, clubs or other organisations using " +
                    "kawaw are only made using verified email addreses.\n\n" +
                    "To see any existing notifications you need to verify your email " +
                    "addresses by clicking on the link in the email sent to that address.";
            }

            if (empty == (Notifications.Count == 0)) return;

            OnPropertyChanged("EmptyOpacity");
            OnPropertyChanged("ListOpacity");
        }

    }
}
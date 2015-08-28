using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;
using System.Linq;

namespace Kawaw
{
    class LinkViewModel : BaseProperties
    {
        private readonly Link _link;

        public LinkViewModel(Link link)
        {
            _link = link;
        }

        public string Title
        {
            get
            {
                var result = _link.Organisation.Name;
                if (_link.Type == "team")
                {
                    result = _link.Team.Name + " - " + result;
                }
                if (_link.Type != "organisation")
                {
                    result = _link.Activity.Name + " - " + result;
                }
                return result;
            }
        }
        public string Members { get { return _link.Members;  } }
    }

    class EventViewModel : BaseProperties
    {
        private readonly Event _event;

        public EventViewModel(Event e)
        {
            _event = e;
        }

        public string DateAndTime
        {
            get
            {
                var result = _event.Start.ToString("ddd, d MMM") + ", " + _event.Start.ToString("h:mm tt").ToLower() + " to ";
                if (!_event.SameDay)
                {
                    result = result + _event.Finish.ToString("ddd, d MMM") + ", ";
                }
                result = result  + _event.Finish.ToString("h:mm tt").ToLower();
                return result;
            }
        }
        public string Type { get { return _event.Type; } }

        public string Address { get { return _event.Venue.Address; } }

        public string Location
        {
            get
            {
                var result = _event.Venue.Name;
                if (!string.IsNullOrEmpty(_event.Location))
                {
                    result = result + ", " + _event.Location;
                }
                return result;
            }
        }

        public IList<LinkViewModel> Links
        {
            get
            {
                return new ObservableCollection<LinkViewModel>(
                   from link in _event.Links
                   orderby link.Members.Length descending, link.Organisation.Name, link.Team.Name
                   select new LinkViewModel(link));
            }
        }
    }

    class EventsViewModel : BaseViewModel
    {
        private IList<EventViewModel> _events;
        private EventViewModel _selectedItem;
        private string _emptyText;

        public double EmptyOpacity { get { return Events.Count == 0 ? 1.0 : 0.0; } }
        public double ListOpacity { get { return Events.Count > 0 ? 1.0 : 0.0; } }

        public IList<EventViewModel> Events
        {
            get { return _events; }
            private set { SetProperty(ref _events, value); }
        }

        public string EmptyText
        {
            get { return _emptyText; }
            private set { SetProperty(ref _emptyText, value); }
        }

        public EventViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var changed = SetProperty(ref _selectedItem, value);
                if (value == null || !changed) return;

                SelectedItem = null;

                // Later do something with the Event.
            }
        }

        public EventsViewModel(IApp app)
            : base(app, RootViewModel.Events)
        {
            Events = new ObservableCollection<EventViewModel>();
            UpdateFromUser(app.User);

            MessagingCenter.Subscribe<User>(this, "initialized", delegate
            {
                Debug.WriteLine("Update because user initialized");
                if (IsPageVisible)
                {
                    UpdateFromUser(app.User);
                }
            });
            MessagingCenter.Subscribe<object>(this, "events-updated", delegate
            {
                if (IsPageVisible)
                {
                    UpdateFromUser(app.User);
                }
            });
        }

        private async void UpdateFromUser(User user)
        {
            Events.Clear();
            if (!user.Initialized || !user.Authenticated) return;

            var evs = await user.Events();

            foreach (var e in evs.OrderBy(e => e.Start))
            {
                Events.Add(new EventViewModel(e));
            }

            EmptyText =
                "No events yet.\n\n" +
                "As events are added by shools, clubs, or other organisations " +
                "that you are connected to, they will show up here.";
        }
    }
}
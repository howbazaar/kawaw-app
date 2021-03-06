﻿using System;
using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    public class OptionalDatePicker : DatePicker
    {

    };
    class ChangeDetailsView : BaseView
    {
        public ChangeDetailsView()
        {
            Title = "Change Details";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var size = Device.GetNamedSize(NamedSize.Medium, typeof (Label));

            var firstNameEntry = new Entry
            {
                Placeholder = "First name"
            };
            firstNameEntry.SetBinding(Entry.TextProperty, "FirstName");
            var lastNameEntry = new Entry
            {
                Placeholder = "Last name"
            };
            lastNameEntry.SetBinding(Entry.TextProperty, "LastName");

            var addressEdit = new Editor
            {
                HeightRequest = 120
            };
            addressEdit.SetBinding(Editor.TextProperty, "Address");

            var saveButton = new Button
            {
                Text = "Save"
            };
            saveButton.SetBinding(Button.CommandProperty, "SaveCommand");

            var cancelButton = new Button
            {
                Text = "Cancel"
            };
            cancelButton.Command = new Command(async() => await Navigation.PopAsync());

            var clearDOB = new Button
            {
                Text = "Clear"
            };
            clearDOB.SetBinding(Button.CommandProperty, "ClearDateOfBirthCommand ");

            var datepicker = new OptionalDatePicker()
            {
                Format = "dd MMM yyyy",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            datepicker.SetBinding(DatePicker.DateProperty, "DateOfBirth");
            datepicker.Focused += (sender, args) =>
            {
                if (datepicker.Date == datepicker.MinimumDate)
                {
                    datepicker.Date = DateTime.Today;
                }
            };
            datepicker.Unfocused += (sender, args) =>
            {
                if (datepicker.Date == DateTime.Today)
                {
                    datepicker.Date = datepicker.MinimumDate;
                }
            };

            var view = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    firstNameEntry,
                    lastNameEntry,
                    new Label {Text = "Address:", FontSize = size},
                    addressEdit,
                    new Label{Text = "Date of Birth: ", FontSize = size},
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            datepicker,
                            clearDOB,
                        }
                    },

                    new Grid
                    {
                        Padding = new Thickness(0, 15),
                        Children =
                        {
                            {cancelButton,0,0},
                            {saveButton,1,0},
                        }
                    }
                }
            };
            Content = new ScrollView
            {
                Content = view
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // SubscribeAlert<ChangeDetailsViewModel>();
            MessagingCenter.Subscribe(this, "alert", async (ChangeDetailsViewModel model, Alert alert) =>
            {
                await DisplayAlert(alert.Title, alert.Text, "OK");
                if (alert.Callback != null)
                {
                    alert.Callback.Execute(this);
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // UnsubscribeAlert<ChangeDetailsViewModel>();
            MessagingCenter.Unsubscribe<ChangeDetailsViewModel, Alert>(this, "alert");
        }

    }
}
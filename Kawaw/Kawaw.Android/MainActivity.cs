using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DatePicker = Xamarin.Forms.DatePicker;

[assembly:ExportRenderer(typeof(Kawaw.OptionalDatePicker), typeof(Kawaw.Droid.OptionalDatePickerRenderer))]
namespace Kawaw.Droid
{
    [Activity(Label = "Kawaw",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask,
        Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : FormsApplicationActivity
    {
        private static bool _insightsInitialized = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            if (!_insightsInitialized)
            {
#if DEBUG
            //Insights.Initialize(Insights.DebugModeKey, this);
            Insights.HasPendingCrashReport += (sender, isStartupCrash) => Insights.PurgePendingCrashReports().Wait();
            Insights.Initialize(Constants.InsightsApiKey, this);
#else
            Insights.Initialize(Constants.InsightsApiKey, this);
#endif
                _insightsInitialized = true;
            }
            LoadApplication(new App());
            SendMessageInResponseToIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            SendMessageInResponseToIntent(intent);
        }

        private void SendMessageInResponseToIntent(Intent intent)
        {
            var tag = intent.GetStringExtra("tag");
            var id = intent.GetIntExtra("id", 0);
            App.OnNotification(this, tag, id);
        }
    }
    
    public class OptionalDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            this.Control.Hint = "Date of Birth";
            SetText();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Date" || e.PropertyName == DatePicker.FormatProperty.PropertyName)
            {
                SetText();
            }
        }

        void SetText()
        {
            // date currently set on the optional date picker (date known to model)
            var date = Element.Date;
            if (date == Element.MinimumDate) 
                Control.Text = "";
        }
    }
}


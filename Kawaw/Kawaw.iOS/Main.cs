using System.ComponentModel;

using MonoTouch.UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Kawaw.OptionalDatePicker), typeof(Kawaw.iOS.OptionalDatePickerRenderer))]
namespace Kawaw.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    public class OptionalDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            this.Control.Placeholder = "Date of Birth";
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

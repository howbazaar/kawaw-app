using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Kawaw.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
#if DEBUG
            Insights.Initialize(Insights.DebugModeKey);
#else
            Insights.Initialize("22fd93ca44698441312e444b5a31160691bc86e5");
#endif

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Kawaw.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Foundation.Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
#if DEBUG
            // Insights.Initialize(Insights.DebugModeKey);
            Insights.Initialize(Constants.InsightsApiKey);
#else
            Insights.Initialize(Constants.InsightsApiKey);
#endif

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Debug.WriteLine("Error registering push notifications");
            var view = new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK",
                null);
            view.Show();
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Get current device token (taken from xamarin website)
            var token = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim('<').Trim('>');
            }

            Debug.WriteLine("Saving token: {0}", token);

            // Save the token in the user defaults. The notification registration getter retrieves it from there.
            NSUserDefaults.StandardUserDefaults.SetString(token, NotificationRegistraion.TokenKey);
        }

        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            application.RegisterForRemoteNotifications();
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            Debug.WriteLine("ReceivedRemoteNotification");
            var tagValue = userInfo["tag"];
            var idValue = userInfo["id"] as NSNumber;
            string tag = "";
            if (tagValue != null)
            {
                tag = tagValue.ToString();
            }
            int id = 0;
            if (idValue != null)
            {
                id = idValue.Int32Value;
            }
            if (application.ApplicationState == UIApplicationState.Active)
            {
                Debug.WriteLine("recieved message while active: {0}, {1}", tag, id);
                var aps = userInfo["aps"] as NSDictionary;
                if (aps == null)
                {
                    Debug.WriteLine("{0} missing 'aps'", userInfo);
                    return;
                }

                var alert = aps["alert"] as NSDictionary;
                if (alert == null)
                {
                    Debug.WriteLine("{0} missing 'alert'", aps);
                    return;
                }

                var body = alert["body"];
                if (body == null)
                {
                    Debug.WriteLine("{0} missing 'body'", alert);
                    return;
                }

                var view = new UIAlertView("New Notification", body.ToString(), null, "OK", null);
                view.Show();
            }
            else
            {
                App.OnNotification(this, tag, id);
            }
        }
    }
}

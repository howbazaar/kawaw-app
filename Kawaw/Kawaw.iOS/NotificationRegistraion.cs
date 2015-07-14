using System.Diagnostics;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Kawaw.iOS.NotificationRegistraion))]
namespace Kawaw.iOS
{
    public class NotificationRegistraion : INotificationRegisration
    {
        public const string TokenKey = "token";

        public string Token { get { return NSUserDefaults.StandardUserDefaults.StringForKey(TokenKey); }
}
        public void Register()
        {
            Debug.Write("Register");
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                Debug.Write("if block");
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                Debug.Write("else block");
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(
                    UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);
            } 
        }

        public void Unregister()
        {
            Debug.Write("Unregister");
            UIApplication.SharedApplication.UnregisterForRemoteNotifications();
        }
    }
}
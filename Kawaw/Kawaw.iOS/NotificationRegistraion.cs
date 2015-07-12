using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Kawaw.iOS.NotificationRegistraion))]
namespace Kawaw.iOS
{
    public class NotificationRegistraion : INotificationRegisration
    {
        private const string TokenKey = "token";

        public string Token { get { return NSUserDefaults.StandardUserDefaults.StringForKey(TokenKey); }
}
        public void Register()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                UIUserNotificationType userNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(userNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            } 
            
            throw new System.NotImplementedException();
        }

        public void Unregister()
        {
            throw new System.NotImplementedException();
        }
    }
}
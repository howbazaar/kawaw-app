using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;

[assembly: Xamarin.Forms.Dependency(typeof(Kawaw.Droid.NotificationRegistraion))]
namespace Kawaw.Droid
{
    public class NotificationRegistraion : INotificationRegisration
    {
        private const string GcmPreferencesKey = "GCMPreferences";
        private const string TokenKey = "token";
        private const string AppVersionKey = "version";

        private const string SenderId = "217815642803";
        public string Token { get { return GetRegistrationId(); } }

        public async void Register()
        {
            var context = Application.Context;

            // If Token is already set, we have registered.
            if (Token != "") return;

            try
            {
                var gcm = GoogleCloudMessaging.GetInstance(context);

                // Run this async so it starts after the app has fully started.
                var regId = await Task.Run(() => gcm.Register(SenderId));

                // Persist the regID - no need to register again.
                StoreRegistrationId(context, regId);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Problem registering: {0}", ex);
            }
        }

        public void Unregister()
        {
            var gcm = GoogleCloudMessaging.GetInstance(Application.Context);
            gcm.Unregister();
        }

        private string GetRegistrationId()
        {
            var context = Application.Context;
            var prefs = GetGCMPreferences(context);
            var registrationId = prefs.GetString(TokenKey, "");

            if (string.IsNullOrEmpty(registrationId))
            {
                return "";
            }

            // Check if app was updated; if so, it must clear the registration ID
            // since the existing registration ID is not guaranteed to work with
            // the new app version.
            var registeredVersion = prefs.GetInt(AppVersionKey, -1);
            var currentVersion = GetAppVersion(context);
            
            if (registeredVersion == currentVersion) return registrationId;

            Debug.WriteLine("App version changed.");
            return "";
        }

        private ISharedPreferences GetGCMPreferences(Context context)
        {
            // This sample app persists the registration ID in shared preferences, but
            // how you store the registration ID in your app is up to you.

            return context.GetSharedPreferences(GcmPreferencesKey, FileCreationMode.Private);
        }

        private static int GetAppVersion(Context context)
        {
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            return packageInfo.VersionCode;
        }

        private void StoreRegistrationId(Context context, string regId)
        {
            var prefs = GetGCMPreferences(context);
            var appVersion = GetAppVersion(context);

            Debug.WriteLine("Saving regId on app version {0}", appVersion);

            var editor = prefs.Edit();
            editor.PutString(TokenKey, regId);
            editor.PutInt(AppVersionKey, appVersion);
            editor.Commit();
        }
    }
}
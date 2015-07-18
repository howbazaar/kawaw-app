using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Xamarin;

namespace Kawaw.Droid
{
    [Service]
    public class PushNotificationImplementation : IntentService
    {
        private const string IntentFromGcmMessage = "com.google.android.c2dm.intent.RECEIVE";
        private const string MessageKey = "message";
        private const string TitleKey = "title";
        private const string TagKey = "tag";
        private const string IdKey = "id";

        public override void OnCreate()
        {
#if DEBUG
            //Insights.Initialize(Insights.DebugModeKey, this);
            Insights.Initialize("22fd93ca44698441312e444b5a31160691bc86e5", this);
#else
            Insights.Initialize("22fd93ca44698441312e444b5a31160691bc86e5", this);
#endif
            base.OnCreate();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            if (intent.Extras != null && !intent.Extras.IsEmpty)
            {
                System.Diagnostics.Debug.WriteLine(intent.Action);

                if (intent.Action.Equals(IntentFromGcmMessage))
                {
                    var parameters = new Dictionary<string, object>();
                    foreach (var key in intent.Extras.KeySet())
                    {
                        parameters.Add(key, intent.Extras.Get(key));
                    }

                    var context = Application.Context;

                    try
                    {
                        int notifyId = 0;
                        string title = "";
                        string message = "";
                        string tag = null;

                        if (parameters.ContainsKey(MessageKey))
                        {
                            message = parameters[MessageKey].ToString();
                        }

                        if (parameters.ContainsKey(TitleKey))
                        {
                            title = parameters[TitleKey].ToString();
                            if (string.IsNullOrEmpty(message))
                            {
                                message = title;
                                title = "";
                            }
                        }
                        if (title == "")
                        {
                            title = context.ApplicationInfo.LoadLabel(context.PackageManager);
                        }

                        if (parameters.ContainsKey(IdKey))
                        {
                            var str = parameters[IdKey].ToString();
                            try
                            {
                                notifyId = Convert.ToInt32(str);
                            }
                            catch (Exception)
                            {
                                // Keep the default value of zero for the notify_id, but log the conversion problem.
                                System.Diagnostics.Debug.WriteLine("Failed to convert {0} to an interger", str);
                            }
                        }
                        if (parameters.ContainsKey(TagKey))
                        {
                            tag = parameters[TagKey].ToString();
                        }

                        // TODO: support silent?
                        CreateNotification(title, message, notifyId, tag);
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                    catch (System.Exception ex1)
                    {
                        System.Diagnostics.Debug.WriteLine(ex1.ToString());
                    }
                }
            }
            // Release the wake lock provided by the WakefulBroadcastReceiver.
            WakefulBroadcastReceiver.CompleteWakefulIntent(intent);
        }

        public static void CreateNotification(string title, string message, int notifyId, string tag)
        {
            Log.Info("kawaw", "CreateNotification");

            var soundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var context = Application.Context;
            var iconResource = context.ApplicationInfo.Icon;

            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.PutExtra("id", notifyId);
            if (!string.IsNullOrEmpty(tag))
            {
                resultIntent.PutExtra("tag", tag);
            }
            resultIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            System.Diagnostics.Debug.WriteLine("extra data: {0}, {1}", tag, notifyId);

            const int pendingIntentId = 0;
            var resultPendingIntent = PendingIntent.GetActivity(context, pendingIntentId, resultIntent, PendingIntentFlags.UpdateCurrent);

            // Build the notification
            var builder = new NotificationCompat.Builder(context)
                      .SetAutoCancel(true) // dismiss the notification from the notification area when the user clicks on it
                      .SetContentIntent(resultPendingIntent) // start up this activity when the user clicks the intent.
                      .SetContentTitle(title) // Set the title
                      .SetSound(soundUri)
                      .SetSmallIcon(iconResource) // This is the icon to display
                      .SetContentText(message); // the message to display.

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(tag, notifyId, builder.Build());
        }

    }
}
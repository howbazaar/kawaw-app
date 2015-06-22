using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    public class CrossPushNotificationListener : IPushNotificationListener
    {
        public void OnMessage(IDictionary<string, object> parameters, DeviceType deviceType)
        {
            Debug.WriteLine("Message Arrived");
            foreach (var entry in parameters)
            {
                Debug.WriteLine("{0}: {1}", entry.Key, entry.Value);
            }
        }
        public void OnRegistered(string token, DeviceType deviceType)
        {
            Debug.WriteLine("Push Notification - Device Registered - Token : {0}", token);
            var registration = new DeviceRegistration
            {
                DeviceType = deviceType,
                Token = token,
            };
            MessagingCenter.Send((object)this, "register-device", registration);
        }
        public void OnUnregistered(DeviceType deviceType)
        {
            Debug.WriteLine("Push Notification - Device Unnregistered");
            MessagingCenter.Send((object)this, "unregister-device", deviceType);
        }
        public void OnError(string message, DeviceType deviceType)
        {
            Debug.WriteLine("Push notification error - {0}", message);
        }
    }
}

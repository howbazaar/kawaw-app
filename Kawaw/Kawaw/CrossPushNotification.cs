using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawaw
{
    public class CrossPushNotificationListener : IPushNotificationListener
    {
        public void OnMessage(IDictionary<string,object> Parameters, DeviceType deviceType)
        {
            Debug.WriteLine("Message Arrived");
        }
        public async void OnRegistered(string Token, DeviceType deviceType)
        {
            // Console log token
            // Use aws to send the token in the console to get message on device 
            Debug.WriteLine(string.Format("Push Notification - Device Registered - Token : {0}", Token));

        }
        public async void OnUnregistered(DeviceType deviceType)
        {
            Debug.WriteLine("Push Notification - Device Unnregistered");
        }
        public void OnError(string message, DeviceType deviceType)
        {
            Debug.WriteLine(string.Format("Push notification error - {0}", message));
        }
    }
}

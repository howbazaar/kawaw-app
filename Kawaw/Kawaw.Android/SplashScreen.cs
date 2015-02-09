using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kawaw.Droid
{
    using System.Threading;
    using Android.App;
    using Android.OS;
    using Android.Content.PM;

    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true) ]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Moving to next activity
            StartActivity(typeof(MainActivity));
        }
    }
}

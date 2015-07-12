using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;

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
        protected override void OnNewIntent(Intent intent)
        {
            var tag = intent.GetStringExtra("tag");
            var id = intent.GetIntExtra("id", 0);
            System.Diagnostics.Debug.WriteLine("Splash... OnNewIntent: {0}, {1}", tag, id);
            base.OnNewIntent(intent);
        }
    }
}

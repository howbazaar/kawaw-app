namespace Kawaw.Droid
{
    using Android.App;
    using Android.OS;

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

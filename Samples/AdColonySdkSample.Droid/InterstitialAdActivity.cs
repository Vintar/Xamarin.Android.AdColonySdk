using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Adcolony.Sdk;

namespace AdColonySdkSample.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class InterstitialAdActivity : AppCompatActivity
    {
        private const string APP_ID = "app185a7e71e1714831a49ec7";
        private const string ZONE_ID = "vz06e8c32a037749699e7050";
        private const string TAG = "AdColonyDemo";

        public Button showButton;
        public ProgressBar progress;
        public AdColonyInterstitial ad;
        public AdColonyAdOptions adOptions;
        public AdColonyInterstitialListener listener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_interstitial_ad);

            progress = FindViewById<ProgressBar>(Resource.Id.progress);

            // Construct optional app options object to be sent with configure
            AdColonyAppOptions appOptions = new AdColonyAppOptions()
                .SetUserID("unique_user_id")
                .SetKeepScreenOn(true);

            // Configure AdColony in your launching Activity's onCreate() method so that cached ads can
            // be available as soon as possible.
            AdColony.Configure(this, appOptions, APP_ID, ZONE_ID);

            // Optional user metadata sent with the ad options in each request
            AdColonyUserMetadata metadata = new AdColonyUserMetadata()
                .SetUserAge(26)
                .SetUserEducation(AdColonyUserMetadata.UserEducationBachelorsDegree)
                .SetUserGender(AdColonyUserMetadata.UserMale);

            // Ad specific options to be sent with request
            adOptions = new AdColonyAdOptions().SetUserMetadata(metadata);

            // Set up listener for interstitial ad callbacks. You only need to implement the callbacks
            // that you care about. The only required callback is onRequestFilled, as this is the only
            // way to get an ad object.
            listener = new MyAdColonyInterstitialListener(this);

            // Set up button to show an ad when clicked
            showButton = FindViewById<Button>(Resource.Id.showbutton);
            showButton.Click += ShowButton_Click;

            // GDPR
            AdColony.AppOptions.SetGDPRConsentString("0");
            AdColony.AppOptions.SetGDPRRequired(false);
        }

        private void ShowButton_Click(object sender, System.EventArgs e)
        {
            ad.Show();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // It's somewhat arbitrary when your ad request should be made. Here we are simply making
            // a request if there is no valid ad available onResume, but really this can be done at any
            // reasonable time before you plan on showing an ad.
            if (ad == null || ad.IsExpired)
            {
                // Optionally update location info in the ad options for each request:
                // LocationManager locationManager =
                //     (LocationManager) getSystemService(Context.LOCATION_SERVICE);
                // Location location =
                //     locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
                // adOptions.setUserMetadata(adOptions.getUserMetadata().setUserLocation(location));
                progress.Visibility = ViewStates.Visible;
                AdColony.RequestInterstitial(ZONE_ID, listener, adOptions);
            }
        }

        private class MyAdColonyInterstitialListener : AdColonyInterstitialListener
        {
            private readonly InterstitialAdActivity _activity;

            public MyAdColonyInterstitialListener(InterstitialAdActivity activity)
            {
                _activity = activity;
            }

            public override void OnRequestFilled(AdColonyInterstitial ad)
            {
                // Ad passed back in request filled callback, ad can now be shown
                _activity.ad = ad;
                _activity.showButton.Enabled = true;
                _activity.progress.Visibility = ViewStates.Invisible;
                Log.Debug(TAG, "onRequestFilled");
            }

            public override void OnRequestNotFilled(AdColonyZone zone)
            {
                // Ad request was not filled
                _activity.progress.Visibility = ViewStates.Invisible;
                Log.Debug(TAG, "onRequestNotFilled");
            }

            public override void OnOpened(AdColonyInterstitial ad)
            {
                // Ad opened, reset UI to reflect state change
                _activity.showButton.Enabled = false;
                _activity.progress.Visibility = ViewStates.Visible;
                Log.Debug(TAG, "onOpened");
            }

            public override void OnExpiring(AdColonyInterstitial ad)
            {
                // Request a new ad if ad is expiring
                _activity.showButton.Enabled = false;
                _activity.progress.Visibility = ViewStates.Visible;
                AdColony.RequestInterstitial(ZONE_ID, this, _activity.adOptions);
                Log.Debug(TAG, "onExpiring");
            }
        }
    }
}
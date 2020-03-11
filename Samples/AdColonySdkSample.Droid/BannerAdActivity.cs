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
    public class BannerAdActivity : AppCompatActivity
    {
        private const string APP_ID = "app185a7e71e1714831a49ec7";
        private const string ZONE_ID = "vz785bc8d42d9c43fdaf";
        private const string TAG = "AdColonyBannerDemo";

        public AdColonyAdViewListener listener;
        public AdColonyAdOptions adOptions;
        public RelativeLayout adContainer;
        public ProgressBar progressBar;
        public Button buttonLoad;
        public AdColonyAdView adView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_banner_ad);

            adContainer = FindViewById<RelativeLayout>(Resource.Id.ad_container);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progress);
            buttonLoad = FindViewById<Button>(Resource.Id.load_button);
            buttonLoad.Click += ButtonLoad_Click;

            // Construct optional app options object to be sent with configure
            AdColonyAppOptions appOptions = new AdColonyAppOptions();

            // Configure AdColony in your launching Activity's onCreate() method so that cached ads can
            // be available as soon as possible.
            AdColony.Configure(this, appOptions, APP_ID, ZONE_ID);

            listener = new MyAdColonyAdViewListener(this);

            // GDPR
            AdColony.AppOptions.SetGDPRConsentString("0");
            AdColony.AppOptions.SetGDPRRequired(false);
        }

        private void ButtonLoad_Click(object sender, System.EventArgs e)
        {
            //Remove previous ad view if present.
            if (adContainer.ChildCount > 0)
            {
                adContainer.RemoveView(adView);
            }

            progressBar.Visibility = ViewStates.Visible;
            buttonLoad.Enabled = false;
            buttonLoad.Alpha = 0.5f;

            RequestBannerAd();
        }

        private void RequestBannerAd()
        {
            // Optional Ad specific options to be sent with request
            adOptions = new AdColonyAdOptions();

            //Request Ad
            AdColony.RequestAdView(ZONE_ID, listener, AdColonyAdSize.Banner, adOptions);
        }

        private void ResetUI()
        {
            progressBar.Visibility = ViewStates.Gone;
            buttonLoad.Enabled = true;
            buttonLoad.Alpha = 1.0f;
        }

        private class MyAdColonyAdViewListener : AdColonyAdViewListener
        {
            private readonly BannerAdActivity _activity;

            public MyAdColonyAdViewListener(BannerAdActivity activity)
            {
                _activity = activity;
            }

            public override void OnRequestFilled(AdColonyAdView adColonyAdView)
            {
                Log.Debug(TAG, "onRequestFilled");
                _activity.ResetUI();
                _activity.adContainer.AddView(adColonyAdView);
                _activity.adView = adColonyAdView;
            }

            public override void OnRequestNotFilled(AdColonyZone zone)
            {
                base.OnRequestNotFilled(zone);
                Log.Debug(TAG, "onRequestNotFilled");
                _activity.ResetUI();
            }

            public override void OnOpened(AdColonyAdView ad)
            {
                base.OnOpened(ad);
                Log.Debug(TAG, "onOpened");
            }

            public override void OnClosed(AdColonyAdView ad)
            {
                base.OnClosed(ad);
                Log.Debug(TAG, "onClosed");
            }

            public override void OnClicked(AdColonyAdView ad)
            {
                base.OnClicked(ad);
                Log.Debug(TAG, "onClicked");
            }

            public override void OnLeftApplication(AdColonyAdView ad)
            {
                base.OnLeftApplication(ad);
                Log.Debug(TAG, "onLeftApplication");
            }
        }
    }
}
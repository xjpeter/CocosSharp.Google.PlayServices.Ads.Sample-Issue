using System;

using CocosSharp;

using System.Threading.Tasks;

using System.Threading;

using Android.Gms.Ads;
using Android.App;
using Android.Content;
using System.Diagnostics;

namespace CocosSharp.Google.PlayServices.Ads.Sample
{
    public class GoogleAdvertisingService
    {
        private const string admobId = "<Get your ID at google.com/ads/admob>";

        private readonly Context _context;

        public GoogleAdvertisingService(Context context)
        {
            _context = context;
        }

        public event EventHandler Closed;

        public void ShowInterstitial()
        {
            var adInterstitial = new InterstitialAd(_context)
                {
                    AdUnitId = admobId
                };

            var request = new AdRequest.Builder()
                .AddTestDevice(AdRequest.DeviceIdEmulator)       // Simulator.
                .Build();

            var intlistener = new adlistener();

            intlistener.AdLoaded += () =>
            {
                Application.SynchronizationContext.Post(_ =>
                    {
                        if (adInterstitial.IsLoaded)
                        {
                            adInterstitial.Show();
                        }
                    }, 
                    null);
            };

            intlistener.AdClosed += () => 
                {
                    Debug.WriteLine("GoogleAdvertisingService - Closed");

                    var closed = Closed;
                    if(closed != null)
                    {
                        closed(this, EventArgs.Empty);
                    }
                };

            intlistener.AdFailedToLoad += () =>
                {
                    Debug.WriteLine("GoogleAdvertisingService - FailedToLoad");
                };

            adInterstitial.AdListener = intlistener;

            adInterstitial.LoadAd(request);
        }

        class adlistener : AdListener
        {
            // Declare the delegate (if using non-generic pattern). 
            public delegate void AdLoadedEvent();
            public delegate void AdClosedEvent();
            public delegate void AdOpenedEvent();

            // Declare the event. 
            public event AdLoadedEvent AdLoaded;
            public event AdClosedEvent AdClosed;
            public event AdOpenedEvent AdOpened;
            public event AdOpenedEvent AdFailedToLoad;

            public override void OnAdLoaded()
            {
                if (AdLoaded != null)
                {
                    this.AdLoaded();
                }

                base.OnAdLoaded();
            }

            public override void OnAdClosed()
            {
                if (AdClosed != null)
                {
                    this.AdClosed();
                }

                base.OnAdClosed();
            }

            public override void OnAdOpened()
            {
                if (AdOpened != null)
                {
                    this.AdOpened();
                }

                base.OnAdOpened();
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                if (AdFailedToLoad != null)
                {
                    this.AdFailedToLoad();
                }

                base.OnAdFailedToLoad(errorCode);
            }
        }
    }
}
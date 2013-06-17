using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Windows.ApplicationModel.Activation;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;

        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            this.Loaded += ExtendedSplashScreen_Loaded;
        }

        async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            bool uploaded = false;
            string persistData = await Instrumentation.Current.LoadPersistData();

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(persistData, Encoding.UTF8, "text/json");

                try
                {
                    // $TODO: replace this with real URL
                    using (var resp = await client.PostAsync("http://paperprototype.cloudapp.net/Instrument", content))
                    {
                        if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // $NOTE: if uploaded successfully, we can simply leave Intrumentation empty.
                            uploaded = true;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (!uploaded && !string.IsNullOrWhiteSpace(persistData))
            {
                Instrumentation.Current.RestoreSettings(persistData);
            }

            this.Frame.Navigate(typeof(DrawingPage));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Retrieve splash screen object
            splash = (SplashScreen)(e.Parameter);
            if (splash != null)
            {
                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }

        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
        }
    }
}

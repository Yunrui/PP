using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace PP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            PerpetuumSoft.WinRT.Framework.Licensing.LicenseManager.SetKey(@"{CIAADGAADDAAC2AADJAAC1AADIAABZAACIAAC3AADGAADEAAC3AADIAADJAADJAA
DBAAA6AACNAACBAAA6AAB5AADDAADCAADIAADGAADDAADAAADHAAA6AAC4AADDAA
DGAAA6AACPAAC7AADCAAC2AADDAADLAADHAAA6AABUAAANAAAKAACGAACZAADBAA
C3AABZAADBAADNAADAAADEAAC7AADHAAB2AAC6AADDAADIAADBAACZAAC7AADAAA
BKAAC1AADDAADBAAANAAAKAAB7AADMAADEAAC7AADGAACZAADIAAC7AADDAADCAA
B6AACZAADIAAC3AABZAABMAABTAABLAABMAABVAABLAABOAABMAABNAABPAAA6AA
BMAABMAABWAABMAABMAABWAABMAABMAAANAAAKAACEAAC7AAC1AAC3AADCAADHAA
C3AACMAADNAADEAAC3AABZAACKAAC3AAC5AAC7AADHAADIAAC3AADGAAC3AAC2AA
ANAAAKAACEAAC7AAC1AAC3AADCAADHAAC3AADHAACBAADCAAB9AADGAADDAADJAA
DEAABZAABNAAANAAAKAACPAAC7AADCAABUAACNAACBAAB5AADDAADCAADIAADGAA
DDAADAAADHAABZAABNAAANAAAKAACPAAC7AADCAABUAACEAAC7AAC1AAC3AADCAA
DHAAC7AADCAAC5AABZAABNAAANAAAKAACKAADJAADCAACMAAC7AADBAAC3AACLAA
C7AAC5AADCAACZAADIAADJAADGAAC3AABZAAB4AACDAAB3AACAAAB6AACEAAB7AA
CBAAB3AAB5AAB3AACFAAB8AACQAAB8AAB4AAB3AABQAAB3AACCAAB6AAB9AAB9AA
COAAB7AACLAAB6AAB7AAB4AACDAAB3AABVAAB6AACFAAB6AABNAAB5AAB7AAB7AA
CRAAB7AACJAAB5AABVAAB7AABOAAB7AACEAAB7AABRAAB5AAB9AAB9AAB6AAB6AA
BOAAB5AAB3AAB8AACBAAB7AACQAAB6AAB6AAB4AABRAAB7AACEAAB5AABTAAB5AA
CCAAB9AABSAAB6AACKAAB3AACDAAB6AABNAAB5AACFAAB7AACCAAB3AACBAACAAA
B6AAB4AACSAAB5AAB7AAB9AAB7AAB4AAB5AAB4AAB6AAB3AABUAAB4AACGAAB9AA
B6AAB3AAB8AAB9AACIAAB3AABMAAB7AAB5AAB9AACKAAB5AACIAAB4AABUAAB4AA
CCAAB9AABPAACAAAB3AAB8AABMAAB4AACRAAB6AACSAAB7AAB4AAB3AABQAAB3AA
BQAAB8AACOAAB4AACKAAB6AACMAAB9AACFAAB8AACDAAB4AACQAAB5AACPAAB8AA
CFAAB9AACJAAB5AABVAAB5AABTAAB4AACDAAB5AABMAAB8AABQAAB6AACRAAB9AA
CQAAB6AABUAAB3AAB6AAB4AACGAAB8AAB3AAB3AACMAAB9AAB6AAB5AACQAAB4AA
COAAB8AACLAAB9AACIAAB8AACRAAB6AACMAAB7AABQAAB5AACGAAB9AACCAAB6AA
CDAAB8AACFAAB8AABOAAB7AABSAAB4AAB4AAB8AACOAAB6AAB5AAB6AACQAAB5AA
CBAAB8AABTAAB5AABVAAB3AABVAAB5AABVAAB3AABVAAB6AABOAAB8AABMAAB5AA
CBAAB4AABQAAB6AAB9AAB5AACGAAB3AABQAAB5AABPAAB9AACHAAB8AACSAAB6AA
CMAAB5AAB8AAB7AACRAAB5AACFAAB9AACQAAANAAAKAAB6AAC3AADHAAC7AAC5AA
DCAACMAAC7AADBAAC3AACLAAC7AAC5AADCAACZAADIAADJAADGAAC3AABZAAB4AA
BRAAB9AABQAAB3AAB7AAB6AAB6AAB6AAB7AAB8AACLAAB8AACHAAB6AACSAAB5AA
BPAAB8AABNAAB9AACRAAB6AAB9AAB8AABVAAB3AACOAAB8AACGAAB3AAB4AAB8AA
BRAAB5AACKAAB7AACDAAB7AACAAAB4AACHAAB4AACKAAB9AACCAAB7AACNAAB7AA
CBAAB9AACKAAB7AACLAAB8AAB6AAB8AACOAAB4AACAAAB3AAB9AAB3AACNAAB6AA
CEAAB4AABOAAB9AABMAAB7AACSAAB6AABVAAB3AACRAAB3AABMAACAAAB5AAB9AA
CHAAB3AABMAAB9AACJAAB6AACMAAB5AABQAAB9AAB9AAB7AABSAAB5AAB7AAB6AA
B7AAB6AAB6AAB6AACCAAB8AACSAAB6AACMAACAAAB4AAB5AABQAAB3AAB7AAB5AA
BTAAB7AACAAAB7AAB8AAB8AABSAAB7AACLAAB8AACCAAB5AACCAAB6AACRAAB5AA
CSAAB8AABMAAB5AACNAAB4AABPAAB3AACMAAB8AABVAAB3AABNAAB3AAB8AAB9AA
BUAAB3AACJAAB4AACSAAB9AACJAAB5AACNAAB4AAB3AAB4AACNAAB6AAB9AAB4AA
B3AAB9AAB3AAB5AAB6AAB4AAB6AACAAAB3AAB3AABPAAB9AACKAAB6AACBAAB7AA
CQAAB8AACQAAB4AACSAAB7AABNAAB4AAB9AAB9AABUAAB5AABVAAB3AABRAAB8AA
B8AAB8AABTAAB4AACBAAB9AACEAAB7AACQAAB5AACHAAB7AACLAAB3AABSAACAAA
B6AACAAAB4AAB8AACJAAB5AABQAAB4AACRAAB5AAB7AAB3AACSAAB9AACNAAB9AA
CCAAB4AACLAAB3AACAAAB6AACRAAB7AACKAAB5AACNAAB8AACKAAB8AACPAAB4AA
CBAAB6AAB5AAB6AACDAAB5AACMAAB9AABPAAB4AACAAAB3AACOAAB6AAB7AAANAA
AKAA}");
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;

            // set sync context for ui thread so async void exceptions can be handled, keeps process alive
            this.UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Instrumentation.Current.Log(e.Exception, e.Exception.StackTrace);
            e.SetObserved();
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // $TODO: should await?? There is no compiler error
            Instrumentation.Current.Log(e.Exception, e.Message);
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(ExtendedSplashScreen), args.SplashScreen))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();

            Instrumentation.Current.SessionStart();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Windows notifies your app when it suspends it, but doesn't provide additional notification when it terminates the app. 
            // That means your app must handle the suspended event and use it to save its state and release its exclusive resources and file handles immediately.

            await Instrumentation.Current.PersistSettings();

            deferral.Complete();
        }

        void App_Resuming(object sender, object e)
        {
            Instrumentation.Current.SessionStart();
        }
    }

}

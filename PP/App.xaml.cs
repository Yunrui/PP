using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
    public class AsyncSynchronizationContext : SynchronizationContext
    {
        public static AsyncSynchronizationContext Register()
        {
            var syncContext = Current;
            if (syncContext == null)
            {
                syncContext = new SynchronizationContext();
                SetSynchronizationContext(syncContext);
            }

            var customSynchronizationContext = syncContext as AsyncSynchronizationContext;

            if (customSynchronizationContext == null)
            {
                customSynchronizationContext = new AsyncSynchronizationContext(syncContext);
                SetSynchronizationContext(customSynchronizationContext);
            }

            return customSynchronizationContext;
        }

        private readonly SynchronizationContext _syncContext;

        public AsyncSynchronizationContext(SynchronizationContext syncContext)
        {
            _syncContext = syncContext;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new AsyncSynchronizationContext(_syncContext.CreateCopy());
        }

        public override void OperationCompleted()
        {
            _syncContext.OperationCompleted();
        }

        public override void OperationStarted()
        {
            _syncContext.OperationStarted();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _syncContext.Post(WrapCallback(d), state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            _syncContext.Send(d, state);
        }

        private static SendOrPostCallback WrapCallback(SendOrPostCallback sendOrPostCallback)
        {
            return state =>
            {
                Exception exception = null;

                try
                {
                    sendOrPostCallback(state);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    Instrumentation.Current.Log(exception, exception.StackTrace);
                }
            };
        }
    }

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
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;

            // set sync context for ui thread so async void exceptions can be handled, keeps process alive
            AsyncSynchronizationContext.Register();
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
                if (!rootFrame.Navigate(typeof(DrawingPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();

            // $TODO: no place to catch this exception??
            // $TODO: should be an async operation
            Instrumentation.Current.RestoreSettings();
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

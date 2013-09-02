using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
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
    public sealed partial class MainPage : Page
    {
        private string[] _ids =
        {
            "1067", "2328", "4357", "4357", "4357", "4357", "4357", "4357", "4357", "4357",
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();


            var comics = new ObservableCollection<CoverFlowDataSource>();

            foreach (string id in _ids)
            {
                CoverFlowDataSource ds = new CoverFlowDataSource();
                ds.Description = "Starting with a blank web page";
                ds.Image = "Assets/WebPage.png";
                comics.Add(ds);
            }

            CoverFlowControl.ItemsSource = comics;
        }

        private void CoverFlowControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DrawingPage));
        }
    }

    public class CoverFlowDataSource
    {
        public string Description
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }
    }
}

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
            "1067", "2328", "2660", "1632", "2486", "2602", "2329", "2634",
            "2297", "1506", "2732", "1453", "1493", "1454", "1516", "1517",
            "1314", "1421", "1444", "1267", "1445", "1135", "1266", "1575",
            "1071", "1263", "1136", "1137", "1138", "1139", "1140", "1141",
            "2498", "1142", "1072", "2427", "1163", "1164", "1165", "1166",
            "1167", "1168", "2604", "1169", "1233", "1234", "1235", "1240",
            "1241", "1242", "1243", "1178", "1244", "1126", "1127", "1128",
            "1129", "1130", "1131", "1264", "1132", "1133", "1265", "1134",
            "1523", "1524", "2168", "1811", "1697", "1698", "1699", "1729",
            "1759", "1760", "2198", "2232"
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();


            var comics = new ObservableCollection<string>();

            foreach (string id in _ids)
                comics.Add("http://199.19.135.170/services/mycomix.svc/cover240/" + id);

            CoverFlowControl.ItemsSource = comics;
        }
    }
}

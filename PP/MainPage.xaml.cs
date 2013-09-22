using PP.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Storage;
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
            var comics = new ObservableCollection<CoverFlowDataSource>();

            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> files = await localFolder.GetFilesAsync();

            foreach (StorageFile file in files)
            {
                if (file.Name.EndsWith(Constants.SuffixOfTemplateFile))
                {
                    CoverFlowDataSource ds = new CoverFlowDataSource();
                    ds.Description = "Starting with a blank web page";
                    ds.TemplateName = file.Name;
                    ds.Image = string.Format("{0}\\{1}.jpg", localFolder.Path, file.Name);
                    comics.Add(ds);
                }
            }

            comics.Add(new CoverFlowDataSource() { TemplateName = string.Empty, Image = "Assets/WebPage.png" });

            if (null == e.Parameter)
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
            }
            else
            {
                string tmp = e.Parameter as string;

                CoverFlowDataSource ds = new CoverFlowDataSource();
                ds.Description = "Your Current Prototype";
                ds.Image = tmp;

                comics.Insert(0, ds);             
            }

            CoverFlowControl.ItemsSource = comics;
        }

        private async void CoverFlowControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            CoverFlowDataSource ds = this.CoverFlowControl.SelectedItem as CoverFlowDataSource;
            
            bool doNavigation = true;

            if (!string.IsNullOrWhiteSpace(ds.TemplateName))
            {
                var md = new MessageDialog("Loading a new template will lose all your previous editing, are you sure you want to load it?");
                md.Commands.Add(new UICommand("Yes"));
                md.Commands.Add(new UICommand("No", (UICommandInvokedHandler) =>
                {
                    doNavigation = false;
                }));
                await md.ShowAsync();
            }

            if (doNavigation)
            {
                this.Frame.Navigate(typeof(DrawingPage), ds.TemplateName);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Clean this datasource to release file handle to the current image file
            this.CoverFlowControl.ItemsSource = null;
            base.OnNavigatingFrom(e);
        }
    }

    public class CoverFlowDataSource
    {
        public string TemplateName
        {
            get;
            set;
        }

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

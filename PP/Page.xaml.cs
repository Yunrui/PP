namespace PP
{
    using PP.Components;
    using PP.Draw;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Core;
    using Windows.Foundation;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;
    using WindowsRuntimeComponent1;
    using Windows.Storage.Pickers;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using PP.Components.Interface;

    /// <summary>
    /// Page for design a prototype
    /// </summary>
    public sealed partial class DrawingPage : Page
    {
        private const int thumbSize = 15;

        private Grid selectedElement = null;

        private D2DWraper d2dManager = new D2DWraper();

        public DrawingPage()
        {
            this.InitializeComponent(); 
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.panelcanvas.Tapped += canvas_Tapped;
            this.appBar.Opened += appBar_Opened;

            d2dManager.Initialize(CoreApplication.MainView.CoreWindow);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var templateName = e.Parameter as string;

            // If another template is selected, we do remove all content in current page and reload template
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                // Seems that just clear Children of Canvas not enough
                TextCollection.Instance.Collection.Clear();

                this.panelcanvas.Children.Clear();
            }
        }

        private void appBar_Opened(object sender, object e)
        {
            this.contextPanel.Visibility = this.selectedElement == null ? Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
        }

        private async void panelcanvas_Drop(object sender, DragEventArgs e)
        {
            // $IMPORTANT: we must catch all exceptions in "async void" method, 
            // otherwise application crashes without a chance to call into UnhandledException Handler
            Exception exception = null;
            try
            {
                // $NOTE: get Component based on selected template
                Uri uri = (Uri)e.Data.Properties["SelectedComponent"];
                Component component = ComponentRetriever.Retrieve(uri.LocalPath.ToString());

                this.AddComponentToUI(component, e.GetPosition(this.panelcanvas));

                await Instrumentation.Current.Log(component.GetType().Name);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private void AddComponentToUI(Component component, Point point)
        {
            Grid grid = new Grid();
            Canvas.SetTop(grid, point.Y);
            Canvas.SetLeft(grid, point.X);
            grid.Width = component.Width + 2 * thumbSize;
            grid.Height = component.Height;

            Thumb thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = thumbSize, Width = thumbSize, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, };
            thumb.DragDelta += ThumbBottomRight_DragDelta;
            grid.Children.Add(thumb);

            thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = thumbSize, Width = thumbSize, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
            thumb.DragDelta += ThumbBottomLeft_DragDelta;
            grid.Children.Add(thumb);

            thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = thumbSize, Width = thumbSize, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top };
            thumb.DragDelta += ThumbTopRight_DragDelta;
            grid.Children.Add(thumb);

            thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = thumbSize, Width = thumbSize, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            thumb.DragDelta += ThumbTopLeft_DragDelta;
            grid.Children.Add(thumb);

            grid.Children.Add(component);

            grid.RenderTransform = new TranslateTransform();

            // Set Manipulation to Component instead of Grid
            // Otherwise, Resizing will also move components
            component.ManipulationMode = ManipulationModes.All;
            component.ManipulationDelta += grid_ManipulationDelta;


            panelcanvas.Children.Add(grid);

            grid.Tapped += new TappedEventHandler(component_Tapped);
        }

        void grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Grid grid = (sender as Component).Parent as Grid;
            TranslateTransform transform = grid.RenderTransform as TranslateTransform;

            double left = - Canvas.GetLeft(grid);
            // $TODO: consider margin later
            transform.X = Math.Min(Math.Max(transform.X + e.Delta.Translation.X, left), this.panelcanvas.ActualWidth + left - grid.Width);

            double top = -Canvas.GetTop(grid);
            transform.Y = Math.Max(transform.Y + e.Delta.Translation.Y, top);
        }

        private void toolbox_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {        
            
            IList<IStorageItem> selectedFiles = new List<IStorageItem>();

            if (e.Items != null && e.Items.Count > 0)
            {
                e.Data.Properties.Add("SelectedComponent", ((BitmapImage)((Image)e.Items[0]).Source).UriSource);
            }
        }

        private void canvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Tap blank area to unselect everything
            Unselect();
        }

        private void component_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Unselect();
            this.Select(sender as Grid);
            e.Handled = true;
        }

        private void ThumbTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Grid grid;
            Component component;
            GetGridAndComponent(sender, out grid, out component);

            // Resize has to respect minimum width/height
            var width = grid.Width;
            if (component.AnchorMode != ResizeAnchorMode.HeightOnly)
            {
                width = Math.Max(grid.Width - e.HorizontalChange, component.ComponentMinWidth + 2 * thumbSize);
            }

            // $NOTE: at least now, grid.Height == component.Height
            var height = grid.Height;
            if (component.AnchorMode != ResizeAnchorMode.WidthOnly)
            {
                height = Math.Max(grid.Height - e.VerticalChange, component.ComponentMinHeight);
            }

            // Set TopLeft position
            Canvas.SetLeft(grid, Canvas.GetLeft(grid) + grid.Width - width);
            Canvas.SetTop(grid, Canvas.GetTop(grid) + grid.Height - height);

            UpdateSize(grid, component, width, height);
        }

        private void ThumbTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Grid grid;
            Component component;
            GetGridAndComponent(sender, out grid, out component);

            // Resize has to respect minimum width/height
            var width = grid.Width;
            if (component.AnchorMode != ResizeAnchorMode.HeightOnly)
            {
                width = Math.Max(grid.Width + e.HorizontalChange, component.ComponentMinWidth + 2 * thumbSize);
            }

            // $NOTE: at least now, grid.Height == component.Height
            var height = grid.Height;
            if (component.AnchorMode != ResizeAnchorMode.WidthOnly)
            {
                height = Math.Max(grid.Height - e.VerticalChange, component.ComponentMinHeight);
            }

            // Set TopLeft position
            Canvas.SetTop(grid, Canvas.GetTop(grid) + grid.Height - height);

            UpdateSize(grid, component, width, height);
        }

        private void ThumbBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Grid grid;
            Component component;
            GetGridAndComponent(sender, out grid, out component);

            // Resize has to respect minimum width/height
            var width = grid.Width;
            if (component.AnchorMode != ResizeAnchorMode.HeightOnly)
            {
                width = Math.Max(grid.Width - e.HorizontalChange, component.ComponentMinWidth + 2 * thumbSize);
            }

            // $NOTE: at least now, grid.Height == component.Height
            var height = grid.Height;
            if (component.AnchorMode != ResizeAnchorMode.WidthOnly)
            {
                height = Math.Max(grid.Height + e.VerticalChange, component.ComponentMinHeight);
            }

            // Set TopLeft position
            Canvas.SetLeft(grid, Canvas.GetLeft(grid) + grid.Width - width);

            UpdateSize(grid, component, width, height);
        }

        private void ThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Grid grid;
            Component component;
            GetGridAndComponent(sender, out grid, out component);

            // Resize has to respect minimum width/height
            var width = grid.Width;
            if (component.AnchorMode != ResizeAnchorMode.HeightOnly)
            {
                width = Math.Max(grid.Width + e.HorizontalChange, component.ComponentMinWidth + 2 * thumbSize);
            }

            // $NOTE: at least now, grid.Height == component.Height
            var height = grid.Height;
            if (component.AnchorMode != ResizeAnchorMode.WidthOnly)
            {
                height = Math.Max(grid.Height + e.VerticalChange, component.ComponentMinHeight);
            }

            UpdateSize(grid, component, width, height);
        }

        private async void Instrument_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                var records = Instrumentation.Current.GetRecords(10);
                MessageDialog md = new MessageDialog(records);
                await md.ShowAsync();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedElement != null)
            {
                this.panelcanvas.Children.Remove(this.selectedElement);
            }

            // After remove selected element, "selected" should disappear then we have to close AppBar
            this.appBar.IsOpen = false;
        }

        private async void Top_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                var maxZIndex = this.panelcanvas.Children.Select(c => Canvas.GetZIndex(c)).Max();
                Canvas.SetZIndex(this.selectedElement, maxZIndex + 1);
                await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Foreground" });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private async void Empty_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                this.panelcanvas.Children.Clear();

                // Seems that just clear Children of Canvas not enough
                TextCollection.Instance.Collection.Clear();

                await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Empty" });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        /// <summary>
        /// Unselect any items in canvas
        /// </summary>
        private void Unselect()
        {
            //Colors.BlanchedAlmond
            // disable previous "selection"
            foreach (UIElement element in this.panelcanvas.Children.Where(c => c is Grid).SelectMany(c => (c as Grid).Children))
            {
                Thumb thumb = element as Thumb;
                if (thumb != null)
                {
                    thumb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }

            // Close AppBar since "Selected" is false now;
            if (this.selectedElement != null)
            {
                selectedElement.Background = new SolidColorBrush(Colors.Transparent);
            }
            this.selectedElement = null;
            this.appBar.IsOpen = false;
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <param name="selectedElement"></param>
        private void Select(Grid selectedElement)
        {
            foreach (UIElement element in selectedElement.Children)
            {
                Thumb thumb = element as Thumb;
                if (thumb != null)
                {
                    thumb.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }

            // $TODO: IsSticky = true better UX?
            this.selectedElement = selectedElement;
            this.appBar.IsOpen = true;
            this.appBar.IsSticky = true;
            selectedElement.Background = new SolidColorBrush(Colors.LightGray);
        }

        private static void GetGridAndComponent(object sender, out Grid grid, out Component component)
        {
            Thumb thumb = sender as Thumb;
            grid = thumb.Parent as Grid;
            component = grid.Children.Where(c => c is Component).First() as Component;
        }

        private static void UpdateSize(Grid grid, Component component, double width, double height)
        {
            // Set Grid Size
            grid.Width = width;
            grid.Height = height;

            // Update Size for component
            component.Resize((width - 2 * thumbSize) / component.Width, height / component.Height);
        }

        private async void Feedback_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                var options = new Windows.System.LauncherOptions();
                options.PreferredApplicationPackageFamilyName = "mail";
                options.PreferredApplicationDisplayName = "mail";

                var mailto = new Uri("mailto:?to=mylpis@hotmail.com&subject=Feedback for Paper Prototype&body=Please enter your feedback below, really appreciate your help!");
                var result = await Windows.System.Launcher.LaunchUriAsync(mailto, options);

                if (!result)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=32005YunRuiSiMa.PP_1ryedwe3pk4t4"));
                }

                await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Feedback" });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            this.appBar.IsOpen = true;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                PPUtils.SavePicture(this.d2dManager, this.panelcanvas);

                await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "SavePicture" });         
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                MessageDialog message = new MessageDialog("Failed to save. Please retry later.");

                await message.ShowAsync();
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;

            try
            {
                Guid guid = Guid.NewGuid();
                string filename = guid + ".jpg";

                PPUtils.SavePicture(this.d2dManager, this.panelcanvas, filename);

                this.Frame.Navigate(typeof(MainPage), "ms-appdata:///local/" + filename);

                await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Back" });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                MessageDialog message = new MessageDialog("Failed to back to landing page. Please retry later.");

                await message.ShowAsync();
                await Instrumentation.Current.Log(exception, exception.StackTrace);
            }
        }

        private async void LoadTemplate(string templateName)
        {
            var json = await PPUtils.ReadFile(templateName);
            MemoryStream stream = new MemoryStream((UTF8Encoding.UTF8.GetBytes(json)));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IList<SerializerComponent>));

            IList<SerializerComponent> components = serializer.ReadObject(stream) as IList<SerializerComponent>;

            this.panelcanvas.Children.Clear();

            foreach (SerializerComponent serializerComponent in components)
            {
                Component compoent = Component.CreateComponent(serializerComponent);

                this.AddComponentToUI(compoent, new Point(compoent.Left, compoent.Top));
            }
        }

        private void GenerateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateButtonPopUp genearteButtonPopUp = new GenerateButtonPopUp();

            this.TemplateNamePopupcontrol.Child = genearteButtonPopUp;

            this.TemplateNamePopupcontrol.IsOpen = true;
        }
    }
}

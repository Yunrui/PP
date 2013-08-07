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

    /// <summary>
    /// Page for design a prototype
    /// </summary>
    public sealed partial class DrawingPage : Page
    {
        private Grid selectedElement = null;
        private const int thumbSize = 15;
        private const string BackgroundImageUri = "ms-appx:///Assets/WebPage.png";

        public DrawingPage()
        {
            this.InitializeComponent();

            this.panelcanvas.Tapped += canvas_Tapped;
            this.appBar.Opened += appBar_Opened;
        }

        public async Task<Stream> GenerateCanvasStream()
        {
            WriteableBitmap bitmap = await this.GenearteWriteableBitmap();

            return new MemoryStream(bitmap.ToByteArray());
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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

                Point point = e.GetPosition(this.panelcanvas);

                Grid grid = new Grid();
                Canvas.SetTop(grid, point.Y);
                Canvas.SetLeft(grid, point.X);
                grid.Width = component.InitialWidth + 2 * thumbSize;
                grid.Height = component.InitialHeight;

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
            var maxZIndex = this.panelcanvas.Children.Select(c => Canvas.GetZIndex(c)).Max();
            Canvas.SetZIndex(this.selectedElement, maxZIndex + 1);
            await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Foreground" });
        }

        private async void Empty_Click(object sender, RoutedEventArgs e)
        {
            this.panelcanvas.Children.Clear();
            await Instrumentation.Current.Log(new Record() { Event = EventId.Action, CustomA = "Empty" });
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

        /// <summary>
        /// Getnerate the writeable bitmap, and register the text.
        /// </summary>
        /// <returns></returns>
        private async Task<WriteableBitmap> GenearteWriteableBitmap()
        {
            Uri backgroundImageUri = new Uri(BackgroundImageUri);

            WriteableBitmap bitmap = await new WriteableBitmap(1, 1).FromContent(backgroundImageUri);

            foreach (UIElement element in this.panelcanvas.Children)
            {
                Grid grid = element as Grid;
                Component component = grid.Children.Where(c => c is Component).First() as Component;

                int left = (int)Canvas.GetLeft(grid);
                int top = (int)Canvas.GetTop(grid);

                TranslateTransform translateTransform = grid.RenderTransform as TranslateTransform;

                if (translateTransform != null)
                {
                    left += (int) translateTransform.X;
                    top += (int) translateTransform.Y;
                }

                component.Draw(bitmap, left, top);
            }

            return bitmap;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap bitmap = await this.GenearteWriteableBitmap();

            await PPUtils.SaveImage(bitmap, true);

            //// draw text
            D2DWraper d2dManager = new D2DWraper();

            d2dManager.Initialize(CoreApplication.MainView.CoreWindow);

            IRandomAccessStream randStream = null;
            
            foreach (TextItem item in TextCollection.Instance.Collection)
            {
                randStream = d2dManager.DrawTextToImage(item.Context, string.Format("{0}\\{1}", ApplicationData.Current.LocalFolder.Path, "tmpImage.jpg"), item.Left, item.Top, item.IsHyperLink).CloneStream();

                if (randStream != null)
                {
                    bitmap.SetSource(randStream);
                }

                await PPUtils.SaveImage(bitmap, true);
            }

            await PPUtils.SaveImage(bitmap);
        }      
    }
}

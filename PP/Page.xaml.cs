namespace PP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Windows.Devices.Input;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.Storage;
    using Windows.UI;
    using Windows.UI.Input;
    using Windows.UI.Input.Inking;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Page for design a prototype
    /// </summary>
    public sealed partial class DrawingPage : Page
    {
        private Grid selectedElement = null;

        public DrawingPage()
        {
            this.InitializeComponent();

            this.panelcanvas.Tapped += canvas_Tapped;
            this.appBar.Opened += appBar_Opened;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
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
                Component component = ComponentRetriever.Retrieve(uri.ToString());

                Point point = e.GetPosition(this.panelcanvas);

                Grid grid = new Grid();
                Canvas.SetTop(grid, point.Y);
                Canvas.SetLeft(grid, point.X);
                grid.Width = component.InitialWidth + 30;
                grid.Height = component.InitialHeight;

                Thumb thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, };
                thumb.DragDelta += ThumbBottomRight_DragDelta;
                grid.Children.Add(thumb);

                thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
                thumb.DragDelta += ThumbBottomLeft_DragDelta;
                grid.Children.Add(thumb);

                thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top };
                thumb.DragDelta += ThumbTopRight_DragDelta;
                grid.Children.Add(thumb);

                thumb = new Thumb() { Background = new SolidColorBrush(Colors.Red), Visibility = Windows.UI.Xaml.Visibility.Collapsed, Height = 15, Width = 15, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                thumb.DragDelta += ThumbTopLeft_DragDelta;
                grid.Children.Add(thumb);

                grid.Children.Add(component);

                // Set Manipulation to Component instead of Grid
                // Otherwise, Resizing will also move components
                component.ManipulationMode = ManipulationModes.All;
                component.ManipulationDelta += grid_ManipulationDelta;
                grid.RenderTransform = new TranslateTransform();

                panelcanvas.Children.Add(grid);

                grid.Tapped += new TappedEventHandler(component_Tapped);

                await Instrumentation.Current.Log(uri.ToString());
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
            transform.X += e.Delta.Translation.X;
            transform.Y += e.Delta.Translation.Y;
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
            Thumb thumb = sender as Thumb;
            Grid grid = thumb.Parent as Grid;

            var component = grid.Children.Where(c => c is Component).First() as Component;

            // $TODO: separate class for this logic and the following
            var width = Math.Max(grid.Width - e.HorizontalChange, component.ComponentMinWidth);
            var height = Math.Max(grid.Height - e.VerticalChange, component.ComponentMinHeight);

            Canvas.SetLeft(grid, Canvas.GetLeft(grid) + (grid.Width - width));
            Canvas.SetTop(grid, Canvas.GetTop(grid) + (grid.Height - height));

            grid.Width = width;
            grid.Height = height;
        }

        private void ThumbTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Grid grid = thumb.Parent as Grid;

            grid.Width += e.HorizontalChange;
            grid.Height -= e.VerticalChange;
            Canvas.SetTop(grid, Canvas.GetTop(grid) + e.VerticalChange);

        }

        private void ThumbBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Grid grid = thumb.Parent as Grid;

            grid.Width -= e.HorizontalChange;
            grid.Height += e.VerticalChange;
            Canvas.SetLeft(grid, Canvas.GetLeft(grid) + e.HorizontalChange);
        }

        private void ThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Grid grid = thumb.Parent as Grid;

            grid.Width += e.HorizontalChange;
            grid.Height += e.VerticalChange;
        }

        private async void Instrument_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = null;
            try
            {
                var records = Instrumentation.Current.GetRecords(5);
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

        /// <summary>
        /// Unselect any items in canvas
        /// </summary>
        private void Unselect()
        {
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
    }
}

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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <remarks>
    /// $TODO: we need
    /// - Move items
    /// - resize items
    /// - layers
    /// - Change content
    /// - Alignme
    /// </remarks>
    public sealed partial class DrawingPage : Page
    {
        InkManager inkManager = new Windows.UI.Input.Inking.InkManager();
        private uint penID;
        private uint touchID;
        private Point previousContactPt;
        private Point currentContactPt;
        private double x1;
        private double y1;
        private double x2;
        private double y2;


        public DrawingPage()
        {
            this.InitializeComponent();

            panelcanvas.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
            panelcanvas.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
            panelcanvas.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
            panelcanvas.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == penID)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(panelcanvas);

                // Pass the pointer information to the InkManager.  
                inkManager.ProcessPointerUp(pt);
            }

            else if (e.Pointer.PointerId == touchID)
            {
                // Process touch input 
            }

            this.touchID = 0;
            this.penID = 0;

            // Call an application-defined function to render the ink strokes. 


            e.Handled = true;
        }


        private void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            if (e.Pointer.PointerId == this.penID)
            {
                PointerPoint pt = e.GetCurrentPoint(panelcanvas);

                // Render a red line on the canvas as the pointer moves.  
                // Distance() is an application-defined function that tests 
                // whether the pointer has moved far enough to justify  
                // drawing a new line. 
                currentContactPt = pt.Position;
                x1 = previousContactPt.X;
                y1 = previousContactPt.Y;
                x2 = currentContactPt.X;
                y2 = currentContactPt.Y;

                if (Distance(x1, y1, x2, y2) > 2.0)
                {
                    Line line = new Line()
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                        StrokeThickness = 4.0,
                        Stroke = new SolidColorBrush(Colors.Green)
                    };

                    previousContactPt = currentContactPt;

                    // Draw the line on the canvas by adding the Line object as 
                    // a child of the Canvas object. 
                    panelcanvas.Children.Add(line);

                    // Pass the pointer information to the InkManager. 
                    inkManager.ProcessPointerUpdate(pt);
                }
            }

            else if (e.Pointer.PointerId == touchID)
            {
                // Process touch input 
            }


        }


        private double Distance(double x1, double y1, double x2, double y2)
        {
            double d = 0;
            d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return d;
        }

        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location. 
            PointerPoint pt = e.GetCurrentPoint(panelcanvas);
            previousContactPt = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed.  
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen || pointerDevType == PointerDeviceType.Touch ||
                    pointerDevType == PointerDeviceType.Mouse &&
                    pt.Properties.IsLeftButtonPressed)
            {
                // Pass the pointer information to the InkManager. 
                inkManager.ProcessPointerDown(pt);
                penID = pt.PointerId;

                e.Handled = true;
            }
        }

        private void panelcanvas_Drop(object sender, DragEventArgs e)
        {
            Uri uri = (Uri) e.Data.Properties["SelectedComponent"];

            Point point = e.GetPosition(this.panelcanvas);

            Rectangle blueRectangle = new Rectangle();
            
            blueRectangle.Height = 200;
            blueRectangle.Width = 200;
            Canvas.SetTop(blueRectangle, point.Y);
            Canvas.SetLeft(blueRectangle, point.X);
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            blueRectangle.Fill = imgBrush;
            panelcanvas.Children.Add(blueRectangle);

            blueRectangle.Tapped += new TappedEventHandler(panelcanvas_Tapped);
        }

        private void toolbox_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {        
            IList<IStorageItem> selectedFiles = new List<IStorageItem>();

            if (e.Items != null && e.Items.Count > 0)
            {
                e.Data.Properties.Add("SelectedComponent", ((BitmapImage)((Image)e.Items[0]).Source).UriSource);
            }
        }

        private void panelcanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}

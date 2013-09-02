namespace PP{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public static class PPUtils
    {
        /// <summary>
        /// Read a file in the Application.Current.LocalFolder into string
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> ReadFile(string filename)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await localFolder.GetFileAsync(filename);

            using (var fs = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                var inStream = fs.GetInputStreamAt(0);
                Windows.Storage.Streams.DataReader reader = new Windows.Storage.Streams.DataReader(inStream);
                await reader.LoadAsync((uint)fs.Size);
                string data = reader.ReadString((uint)fs.Size);
                reader.DetachStream();
                return data;
            }
        }

        /// <summary>
        /// Get the left top point of a component. As a component will be surrounded by a grid, so use the grid to calc.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Point GetActualLeftTop(Grid grid)
        {
            Point point = new Point(Canvas.GetLeft(grid), Canvas.GetTop(grid));

            TranslateTransform translateTransform = grid.RenderTransform as TranslateTransform;

            if (translateTransform != null)
            {
                point.X += (int)translateTransform.X;
                point.Y += (int)translateTransform.Y;
            }

            return point;
        }

        public async static Task SaveImage(WriteableBitmap src, StorageFile savedItem)
        {
            Exception exception = null;
            try
            {
                Guid encoderId;

                if (savedItem == null)
                {
                    return;
                }

                switch (savedItem.FileType.ToLower())
                {
                    case ".jpg":
                        encoderId = BitmapEncoder.JpegEncoderId;
                        break;
                    case ".bmp":
                        encoderId = BitmapEncoder.BmpEncoderId;
                        break;
                    case ".png":
                    default:
                        encoderId = BitmapEncoder.PngEncoderId;
                        break;
                }

                using (IRandomAccessStream fileStream = await savedItem.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, fileStream);

                    encoder.SetPixelData(
                        BitmapPixelFormat.Rgba8,
                        BitmapAlphaMode.Straight,
                        (uint)src.PixelWidth,
                        (uint)src.PixelHeight,
                        96, // Horizontal DPI 
                        96, // Vertical DPI 
                        src.ToByteArray()
                        );
                    await encoder.FlushAsync();
                }                
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
    }
}

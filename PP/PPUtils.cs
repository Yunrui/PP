namespace PP{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    public static class PPUtils
    {
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

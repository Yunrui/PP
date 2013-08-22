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
        public async static Task<bool> SaveImage(WriteableBitmap src, bool saveToLocal = false)
        {
            bool isSuccess = false;

            StorageFile savedItem = null;

            if (saveToLocal)
            {
                savedItem = await ApplicationData.Current.LocalFolder.CreateFileAsync("tmpImage.jpg", CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                FileSavePicker save = new FileSavePicker();
                save.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                save.DefaultFileExtension = ".jpg";
                save.SuggestedFileName = "newimage";
                save.FileTypeChoices.Add(".bmp", new List<string>() { ".bmp" });
                save.FileTypeChoices.Add(".png", new List<string>() { ".png" });
                save.FileTypeChoices.Add(".jpg", new List<string>() { ".jpg", ".jpeg" });
                savedItem = await save.PickSaveFileAsync();
            }

            Exception exception = null;
            try
            {
                Guid encoderId;
                if (savedItem != null)
                {
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

                    isSuccess = true;
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

            return isSuccess;
        }
    }
}

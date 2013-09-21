namespace PP{
    using PP.Components;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using System.Runtime.Serialization.Json;
    using Windows.Graphics.Imaging;
    using PP.Draw;
    using Windows.Storage.Pickers;
    using WindowsRuntimeComponent1;

    /// <summary>
    /// The general utils class
    /// </summary>
    public static class PPUtils
    {
        public static WriteableBitmap IconBitmap;

        private const string IconImageUri = "ms-appx:///Assets/IconForSave.png";
        private const string BackgroundImageUri = "ms-appx:///Assets/WebPage.png";

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

        private static async void SaveTemplate(string templateName, Canvas panelCanvas)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IList<SerializerComponent>));

            using (MemoryStream stream = new MemoryStream())
            {
                IList<SerializerComponent> components = new List<SerializerComponent>();

                foreach (UIElement element in panelCanvas.Children)
                {
                    Grid grid = element as Grid;

                    Component component = grid.Children.Where(c => c is Component).First() as Component;

                    SerializerComponent serializerComponent = new SerializerComponent(component);

                    Point leftTopPoint = PPUtils.GetActualLeftTop(grid);

                    serializerComponent.Left = leftTopPoint.X;
                    serializerComponent.Top = leftTopPoint.Y;

                    components.Add(serializerComponent);
                }

                serializer.WriteObject(stream, components);
                await stream.FlushAsync();

                stream.Seek(0, SeekOrigin.Begin);
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(templateName, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Save a bitmap with the current file name
        /// </summary>
        /// <param name="finalPictureStorageFile">null means save to local</param>
        public static async void SavePicture(D2DWraper d2dManager, Canvas panelCanvas, string localFileName = "")
        {
            StorageFile finalPictureStorageFile = null;

            if (string.IsNullOrEmpty(localFileName))
            {
                finalPictureStorageFile = await PPUtils.GetStorageFile();
                localFileName = "tmpImage.jpg";
            }

            StorageFile localStorageFile = await PPUtils.GetStorageFile(localFileName);

            WriteableBitmap bitmap = await PPUtils.GenearteWriteableBitmap(panelCanvas);

            await PPUtils.SaveImage(bitmap, localStorageFile);

            foreach (TextItem item in TextCollection.Instance.Collection)
            {
                using (IRandomAccessStream randStream = d2dManager.DrawTextToImage(item.Context, string.Format("{0}\\{1}", ApplicationData.Current.LocalFolder.Path, localFileName), item.Left, item.Top, item.IsHyperLink).CloneStream())
                {
                    if (randStream != null)
                    {
                        bitmap.SetSource(randStream);
                    }
                }

                await PPUtils.SaveImage(bitmap, localStorageFile);
            }

            if (finalPictureStorageFile != null)
            {
                await PPUtils.SaveImage(bitmap, finalPictureStorageFile);
            }
        }

        /// <summary>
        /// Getnerate the writeable bitmap, and register the text.
        /// </summary>
        /// <returns></returns>
        public static async Task<WriteableBitmap> GenearteWriteableBitmap(Canvas panelCanvas)
        {
            Uri iconImageUri = new Uri(IconImageUri);
            PPUtils.IconBitmap = await new WriteableBitmap(1, 1).FromContent(iconImageUri);

            Uri backgroundImageUri = new Uri(BackgroundImageUri);

            WriteableBitmap bitmap = await new WriteableBitmap(1, 1).FromContent(backgroundImageUri);

            foreach (UIElement element in panelCanvas.Children)
            {
                Grid grid = element as Grid;
                Component component = grid.Children.Where(c => c is Component).First() as Component;

                Point leftTopPoint = PPUtils.GetActualLeftTop(grid);

                if (component is Icon)
                {
                    (component as Icon).Draw(bitmap, (int)leftTopPoint.X, (int)leftTopPoint.Y, PPUtils.IconBitmap);
                }
                else
                {
                    component.Draw(bitmap, (int)leftTopPoint.X, (int)leftTopPoint.Y);
                }
            }

            return bitmap;
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

        private static async Task<StorageFile> GetStorageFile(string fileName = "")
        {
            StorageFile savedItem = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                savedItem = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
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

            return savedItem;
        }
    }
}

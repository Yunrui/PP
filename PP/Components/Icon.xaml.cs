namespace PP.Components
{
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class Icon : Component
    {
        private const double DefaultIconSize = 50.4259;
        private const double DefaultControlSize = 130;
        private const string IconImageUri = "ms-appx:///Assets/IconForSave.png";

        public Icon()
        {
            this.InitializeComponent();
        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.BorderRectangle.Height *= percentageHeight;
            this.BorderRectangle.Width *= percentageWidth;

            double alpha = Math.Min(this.Height, this.Width) / DefaultControlSize;

            this.PencilIcon.Height = alpha * DefaultIconSize;
            this.PencilIcon.Width = alpha * DefaultIconSize;
        }

        public void Draw(WriteableBitmap bitmap, int left, int top, WriteableBitmap iconBitMap)
        {
            int iconSize = iconBitMap.PixelWidth;
            bitmap.Blit(new Rect() { Height = this.Height, Width = this.Width, X = left, Y = top },
                iconBitMap, new Rect() { Height = iconSize, Width = iconSize, X = 0, Y = 0 },
                WriteableBitmapExtensions.BlendMode.ColorKeying);
        }
    }
}

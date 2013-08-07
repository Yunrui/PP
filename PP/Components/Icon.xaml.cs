namespace PP.Components
{
    using System;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class Icon : Component
    {
        private const double DefaultIconSize = 50.4259;
        private const double DefaultControlSize = 130;

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

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
        }

    }
}

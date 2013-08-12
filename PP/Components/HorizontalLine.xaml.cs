namespace PP.Components
{
    using Windows.UI;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HorizontalLine : Component
    {
        public HorizontalLine()
        {
            this.InitializeComponent();

        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.Line.X2 *= percentageWidth;
        }

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            bitmap.FillRectangle(left, top, (int) ( left + this.Width ), (int) (top + DeltaPixel), Colors.Black);
        }
    }
}

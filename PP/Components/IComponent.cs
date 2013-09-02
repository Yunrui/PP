namespace PP
{
    using Windows.UI.Xaml.Media.Imaging;

    public enum ResizeAnchorMode
    {
        Full = 0,
        WidthOnly = 1,
        HeightOnly = 2,
        None = 3,
    }

    public interface IComponent
    {
        string ComponentName { get; }
        double ComponentMinWidth { get; }
        double ComponentMinHeight { get; }
        double Width { get; }
        double Height { get; }
        ResizeAnchorMode AnchorMode { get; }

        void Resize(double percentageWidth, double percentageHeight);

        void Draw(WriteableBitmap bitmap, int left, int top);
    }
}

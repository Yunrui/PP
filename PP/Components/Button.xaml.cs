namespace PP.Components
{
    using PP.Common;
    using PP.Draw;
    using System;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class Button : Component
    {
        public Button()
        {
            this.InitializeComponent();

            this.SetText(Constants.DefaultButtonContext);
        }

        public override void SetText(string text)
        {
            base.SetText(text);

            this.TextBlock.Text = this.Text;
            this.ConfigureTextBox.Text = this.TextBlock.Text;
        }

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            /*
             * As the api doesn't support the text alignment, so we will add some space to make it looks like align:center
             * we calc the signle text width runtime to make sure we can support the resize of text later
             */
            string context = this.TextBlock.Text;

            double singleWidth = this.TextBlock.ActualWidth / this.TextBlock.Text.Length;

            int spaceLength = (int) Math.Floor( (this.ActualWidth - this.TextBlock.ActualWidth) / (2 * singleWidth) ) + 1;

            for (int i = 0; i <= spaceLength; i++)
            {
                context = " " + context;
            }

            TextCollection.Instance.Collection.Add(
                new TextItem()
                {
                    Context = context,
                    Left = left + 2 * DeltaPixel,
                    Top = top + 2 * DeltaPixel
                }
            );

            bitmap.DrawRectangle(left, top, (int)(left + this.Width), (int)(top + this.Height), Colors.Gray);
            bitmap.DrawRectangle(left + DeltaPixel, top + DeltaPixel, (int)(left + this.Width - DeltaPixel), (int)(top + this.Height - DeltaPixel), Colors.Black);

            base.Draw(bitmap, left, top);
        }

        /// <summary>
        /// 1. Show the popup
        /// 2. Set the keyboard foucs to the configure text box
        /// </summary>
        /// <param name="sender">the sender param</param>
        /// <param name="e">the event param</param>
        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.ConfigurePopup.IsOpen = true;

            this.ConfigureTextBox.Focus(FocusState.Keyboard);
            this.ConfigureTextBox.SelectAll();
        }

        /// <summary>
        /// This method will be triggered by taped in any other place
        /// </summary>
        /// <param name="sender">the sender param</param>
        /// <param name="e">the event param</param>
        private void ConfigurePopup_Closed(object sender, object e)
        {
            /*
             * Replace the new line with " "
             * The conifgure textbox with support text wrap & accepct returns.
             * But the textblock will not by design
             */
            this.TextBlock.Text = this.ConfigureTextBox.Text.Replace(Environment.NewLine, " ");
            this.Text = this.TextBlock.Text;
            this.ConfigureTextBox.SelectAll();
        }
    }
}

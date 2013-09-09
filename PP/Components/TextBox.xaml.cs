namespace PP.Components
{
    using PP.Common;
    using PP.Components.Interface;
    using PP.Draw;
    using System;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class TextBox : Component
    {
        public TextBox()
        {
            this.InitializeComponent();

            this.SetText(Constants.DefaultTextBoxContext);
        }

        public override void SetText(string text)
        {
            base.SetText(text);

            this.TextBlock.Text = this.Text;
            this.ConfigureTextBox.Text = this.TextBlock.Text;
        }

        /// <summary>
        /// Draw the TextBox control to the bitmap
        /// </summary>
        /// <param name="bitmap">the target to draw the control</param>
        /// <param name="left">the x-axis of the left</param>
        /// <param name="top">the y-axis of the top</param>
        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            TextCollection.Instance.Collection.Add(
                new TextItem()
                {
                    Context = this.TextBlock.Text,
                    Left = left + DeltaPixel,
                    Top = top + DeltaPixel
                }
            );

            bitmap.DrawRectangle(left, top, (int)(left + this.Width), (int)(top + this.Height), Colors.Black);
        }

        /// <summary>
        /// 1. Show the popup
        /// 2. Set the keyboard foucs to the configure text box
        /// </summary>
        /// <param name="sender">the sender param</param>
        /// <param name="e">the event param</param>
        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.ConfigurePopup.IsOpen = true;

            this.ConfigureTextBox.Focus(FocusState.Keyboard);
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

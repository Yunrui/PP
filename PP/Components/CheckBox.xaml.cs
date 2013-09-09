namespace PP.Components
{
    using PP.Common;
    using PP.Draw;
    using System;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CheckBox : Component
    {
        private const int CheckBoxSize = 18;

        public CheckBox()
        {
            this.InitializeComponent();

            this.SetText(Constants.DefaultCheckBoxContext);
        }

        public override void SetText(string text)
        {
            base.SetText(text);

            this.TextBlock.Text = this.Text;
            this.ConfigureTextBox.Text = this.TextBlock.Text;
        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.TextBlock.Width = this.Width - 32;
        }

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            TextCollection.Instance.Collection.Add(
                new TextItem()
                {
                    Context = this.TextBlock.Text,
                    Left = left + DeltaPixel * 2 + CheckBoxSize,
                    Top = top + DeltaPixel
                }
            );

            //draw the check box

            bitmap.DrawRectangle(left, top + DeltaPixel * 2, left + CheckBoxSize, top + DeltaPixel * 2 + CheckBoxSize, Colors.Black);

            //draw the correct mark when it's visible
            if (this.CheckIcon.Visibility == Visibility.Visible)
            {
                // the transform from the (0,0)
                int dotX = left;
                int dotY = top + DeltaPixel * 2;

                bitmap.DrawLine(
                    dotX, 
                    dotY + CheckBoxSize / 2,
                    dotX + CheckBoxSize / 2,
                    dotY + CheckBoxSize,
                    Colors.Black
                );

                bitmap.DrawLine(
                    dotX + CheckBoxSize / 2,
                    dotY + CheckBoxSize,
                    dotX + CheckBoxSize,
                    dotY,
                    Colors.Black
                );
            }
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
             * This code is duped with the code in the TextBox.xaml.cs
             * I'll refactor it in the Week2
             */
            this.TextBlock.Text = this.ConfigureTextBox.Text.Replace(Environment.NewLine, " ");
            this.Text = this.TextBlock.Text;
            this.ConfigureTextBox.SelectAll();
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
            this.ConfigureTextBox.SelectAll();
        }

        private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.CheckIcon.Visibility == Visibility.Visible)
            {
                this.CheckIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckIcon.Visibility = Visibility.Visible;
            }
        }
    }
}

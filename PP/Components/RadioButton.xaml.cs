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
    public sealed partial class RadioButton : Component
    {
        private const int CheckBoxSize = 18;
        private const int RadiusOfChosenMark = 8;

        public RadioButton()
        {
            this.InitializeComponent();

            this.TextBlock.Text = Constants.DefaultRadioButtonContext;
            this.ConfigureTextBox.Text = Constants.DefaultRadioButtonContext;
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

            int delta = (CheckBoxSize - RadiusOfChosenMark) / 2;

            //draw the check box
            bitmap.DrawEllipse(left, top + DeltaPixel * 2, left + CheckBoxSize, top + DeltaPixel * 2 + CheckBoxSize, Colors.Black);

            if (this.ChooseEllipse.Visibility == Visibility.Visible)
            {
                bitmap.FillEllipse(left + delta, top + DeltaPixel * 2 + delta, left + CheckBoxSize - delta, top + DeltaPixel * 2 + CheckBoxSize - delta, Colors.Black);
            }
        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.TextBlock.Width = this.Width - 32;
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

        private void Ellipse_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.ChooseEllipse.Visibility == Visibility.Visible)
            {
                this.ChooseEllipse.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ChooseEllipse.Visibility = Visibility.Visible;
            }
        }
    }
}

namespace PP.Components
{
    using PP.Common;
    using PP.Draw;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComboBox : Component
    {
        private int BoxSize = 18;

        public ComboBox()
        {
            this.InitializeComponent();

            this.TextBlock.Text = Constants.DeafultComboBoxContext;
            this.ConfigureTextBox.Text = Constants.DeafultComboBoxContext;
        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.TextBlock.Width = this.Width - 32;
        }

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            this.BoxSize = (int) this.Height;

            TextCollection.Instance.Collection.Add(
                new TextItem()
                {
                    Context = this.TextBlock.Text,
                    Left = left + DeltaPixel,
                    Top = top + DeltaPixel
                }
            );

            // draw textbox of the context
            bitmap.DrawRectangle(left, top, (int) (left + this.Width - BoxSize), (int)( top + this.Height), Colors.Black);

            int dotX = (int) (left + this.Width - BoxSize);
            int dotY = top;

            // draw the box
            bitmap.DrawRectangle(
                dotX, 
                dotY,
                dotX + BoxSize,
                dotY + BoxSize,
                Colors.Black);

            double heightRateTop = 0.27;
            double heightRateBottom = 0.918;

            bitmap.FillTriangle(
                dotX,
                (int)(dotY + heightRateTop * BoxSize),
                dotX + BoxSize,
                (int)(dotY + heightRateTop * BoxSize),
                dotX + BoxSize / 2,
                (int) (dotY + heightRateBottom * BoxSize),
                Colors.Black
            );
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

    }
}

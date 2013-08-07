namespace PP.Components
{
    using PP.Common;
    using PP.Draw;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;
    using Windows.UI.Xaml.Shapes;
    using WindowsRuntimeComponent1;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Label : Component
    {
        public Label()
        {
            this.InitializeComponent();

            this.TextBlock.Text = Constants.DefaultLabelContext;
            this.ConfigureTextBox.Text = Constants.DefaultLabelContext;
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
        }
    }
}

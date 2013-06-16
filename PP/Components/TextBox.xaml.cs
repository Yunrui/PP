using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PP.Components
{
    public sealed partial class TextBox : Component
    {
        public TextBox()
        {
            this.InitializeComponent();
        }

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.ConfigurePopup.IsOpen = true;
        }

        private void ConfigurePopup_Closed(object sender, object e)
        {
            /*
             * Replace the new line with " "
             * The conifgure textbox with support text wrap & accepct returns.
             * But the textblock will not by design
             */
            this.TextBlock.Text = this.ConfigureTextBox.Text.Replace(Environment.NewLine, " ");
        }
    }
}

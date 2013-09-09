namespace PP.Components
{
    using PP.Common;
    using PP.Draw;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HyperLink : Component
    {
        public HyperLink()
        {
            this.InitializeComponent();

            this.SetText(Constants.DefaultHyperLinkContext);
        }

        public override void SetText(string text)
        {
            base.SetText(text);

            this.updateUnderLine(this.Text);
            this.ConfigureTextBox.Text = this.Text;
        }

        public override void Draw(WriteableBitmap bitmap, int left, int top)
        {
            TextCollection.Instance.Collection.Add(
                new TextItem()
                {
                    Context = this.TextBlock.Text,
                    Left = left + DeltaPixel,
                    Top = top + DeltaPixel,
                    IsHyperLink = true
                }
            );            
        }

        private void updateUnderLine(string context)
        {
            var newContext = new Windows.UI.Xaml.Documents.Run();

            newContext.Text = context;

            this.UnderLine.Inlines.Clear();

            this.UnderLine.Inlines.Add(newContext);
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
    }
}

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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PP.Components
{
    public sealed partial class Icon : Component
    {
        public Icon()
        {
            this.InitializeComponent();
        }

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.Ellipse.Height *= percentageHeight;
            this.Path.Height *= percentageHeight;

            this.Ellipse.Width *= percentageWidth;
            this.Path.Width *= percentageWidth;
        }
    }
}

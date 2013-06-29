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

        private const double DefaultIconSize = 50.4259;
        private const double DefaultControlSize = 130;

        public override void Resize(double percentageWidth, double percentageHeight)
        {
            base.Resize(percentageWidth, percentageHeight);

            this.BorderRectangle.Height *= percentageHeight;
            this.BorderRectangle.Width *= percentageWidth;

            double alpha = Math.Min(this.Height, this.Width) / DefaultControlSize;

            this.PencilIcon.Height = alpha * DefaultIconSize;
            this.PencilIcon.Width = alpha * DefaultIconSize;
        }

    }
}

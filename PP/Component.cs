using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace PP
{
    public abstract class Component : UserControl, IComponent
    {
        public string ComponentName
        {
            get { return this.GetType().FullName; }
        }

        public int ComponentMinWidth { get { return 100; } }

        public int ComponentMinHeight { get { return 32; } }

        public double InitialWidth { get { return this.Width; } }

        public double InitialHeight { get { return this.Height; } }

        public ResizeAnchorMode AnchorMode { get { return ResizeAnchorMode.Full; } }
    }
}

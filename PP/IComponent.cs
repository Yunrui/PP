using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP
{
    public enum ResizeAnchorMode
    {
        Full = 0,
        WidthOnly = 1
    }

    public interface IComponent
    {
        string ComponentName { get; }
        double ComponentMinWidth { get; }
        double ComponentMinHeight { get; }
        double InitialWidth { get; }
        double InitialHeight { get; }
        ResizeAnchorMode AnchorMode { get; }

        void Resize(double widthPercentage, double heightPercentage);
    }
}

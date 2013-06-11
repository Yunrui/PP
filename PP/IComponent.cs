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
    }

    public interface IComponent
    {
        string ComponentName { get; }
        int ComponentMinWidth { get; }
        int ComponentMinHeight { get; }
        double InitialWidth { get; }
        double InitialHeight { get; }
        ResizeAnchorMode AnchorMode { get; }
    }
}

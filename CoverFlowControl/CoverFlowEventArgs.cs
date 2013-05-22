using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverFlowControl
{
    public class CoverFlowEventArgs
    {
        public int Index { get; set; }
        public object Item { get; set; }
        public bool MouseClick { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Sail
    {
        public float sailSpeed { get; set; }
        public int windWindowAdd { get; set; } // used for shipWindWindow bounds
        public int windWindowSub { get; set; } // ...
        public int sailIsLeftColumn { get; set; }
        public int sailIsRightColumn { get; set; }
    }
}

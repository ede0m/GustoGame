using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounding
{
    public class Target
    {
        public Point mapCordPoint { get; set; }
        public Vector2 targetLoc { get; set; }
        public Guid interiorId { get; set; }
    }
}

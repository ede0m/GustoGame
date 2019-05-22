using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public class PhysicsUtility
    {
        public static float sin45deg = (float)(1 / Math.Sqrt(2));
        public static float baseShipMovementSpeed = 0.2f;
        public static float basePlayerMovementSpeed = 0.4f;

        public static float VectorMagnitude(float x2, float x1, float y2, float y1)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}

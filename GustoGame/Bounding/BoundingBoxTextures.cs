using Gusto.Bounds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Gusto
{
    public class BoundingBoxTextures
    {
        public static Dictionary<string, Dictionary<string, Rectangle>> DynamicBoundingBoxTextures = new Dictionary<string, Dictionary<string, Rectangle>>();
        public static Dictionary<string, Dictionary<string, Polygon>> DynamicBoundingPolygons = new Dictionary<string, Dictionary<string, Polygon>>();
    }
}

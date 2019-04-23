using Comora;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounding
{
    public class SpatialBounding
    {
        private Camera cam;
        private int verticalMid;
        private int horizontalMid;

        private Rectangle overLay;

        private static Rectangle q0;
        private static Rectangle q1;
        private static Rectangle q2;
        private static Rectangle q3;
        public static Dictionary<string, List<Sprite>> spatialBoundingMap;
        private static Dictionary<Rectangle, List<string>> bbQuadMap { get; set; }
        private Dictionary<string, Rectangle> quadNameMap;


        public SpatialBounding(Rectangle bounds, Camera camera)
        {
            cam = camera;
            verticalMid = bounds.Width / 2;
            horizontalMid = bounds.Height / 2;

            overLay = new Rectangle(0, 0, bounds.Width, bounds.Height);

            q0 = new Rectangle((int)overLay.X, (int)overLay.Y, verticalMid, horizontalMid);
            q1 = new Rectangle((int)overLay.X + verticalMid, (int)overLay.Y, verticalMid, horizontalMid);
            q2 = new Rectangle((int)overLay.X, (int)overLay.Y + horizontalMid, verticalMid, horizontalMid);
            q3 = new Rectangle((int)overLay.X + verticalMid, (int)overLay.Y + horizontalMid, verticalMid, horizontalMid);

            bbQuadMap = new Dictionary<Rectangle, List<string>>();
            spatialBoundingMap = new Dictionary<string, List<Sprite>>();
            quadNameMap = new Dictionary<string, Rectangle>();

            spatialBoundingMap.Add("q0", new List<Sprite>());
            spatialBoundingMap.Add("q1", new List<Sprite>());
            spatialBoundingMap.Add("q2", new List<Sprite>());
            spatialBoundingMap.Add("q3", new List<Sprite>());
        }

        public static void SetQuad(Sprite sp)
        {
            var bb = sp.GetBoundingBox();
            if (!bbQuadMap.ContainsKey(bb))
                bbQuadMap[bb] = new List<string>();

            if (bb.Intersects(q0))
            {
                spatialBoundingMap["q0"].Add(sp);
                bbQuadMap[bb].Add("q0");
            }
            if (bb.Intersects(q1))
            {
                spatialBoundingMap["q1"].Add(sp);
                bbQuadMap[bb].Add("q1");
            }
            if (bb.Intersects(q2))
            {
                spatialBoundingMap["q2"].Add(sp);
                bbQuadMap[bb].Add("q2");
            }
            if (bb.Intersects(q3))
            {
                spatialBoundingMap["q3"].Add(sp);
                bbQuadMap[bb].Add("q3");
            }
        }

        public void Update(Vector2 pos)
        {
            overLay.X = (int)pos.X - verticalMid; // sets overlay to top left.. hopefully
            overLay.Y = (int)pos.Y - horizontalMid;

            q0 = new Rectangle((int)overLay.X, (int)overLay.Y, verticalMid, horizontalMid);
            q1 = new Rectangle((int)overLay.X + verticalMid, (int)overLay.Y, verticalMid, horizontalMid);
            q2 = new Rectangle((int)overLay.X, (int)overLay.Y + horizontalMid, verticalMid, horizontalMid);
            q3 = new Rectangle((int)overLay.X + verticalMid, (int)overLay.Y + horizontalMid, verticalMid, horizontalMid);
        }

        public void Clear()
        {
            spatialBoundingMap.Clear();
            bbQuadMap.Clear();
            spatialBoundingMap.Add("q0", new List<Sprite>());
            spatialBoundingMap.Add("q1", new List<Sprite>());
            spatialBoundingMap.Add("q2", new List<Sprite>());
            spatialBoundingMap.Add("q3", new List<Sprite>());
        }

        public List<string> GetQuadKey(Rectangle spriteBB)
        {
            return bbQuadMap[spriteBB];
        }

        public Dictionary<string, List<Sprite>> GetSpatialBoundingMap()
        {
            return spatialBoundingMap;
        }

        public Dictionary<Rectangle, List<string>> GetBBQuadMap()
        {
            return bbQuadMap;
        }
    }
}

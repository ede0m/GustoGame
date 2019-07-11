using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounds
{
    public class Polygon
    {
        public List<Line> Verts;
        public Vector2 UpperLeftPoint;


        public bool IntersectsRect(Rectangle rect)
        {
            List<Point> rectPoints = new List<Point>();
            rectPoints.Add(new Point(rect.Left, rect.Top));
            rectPoints.Add(new Point(rect.Left, rect.Bottom));
            rectPoints.Add(new Point(rect.Right, rect.Top));
            rectPoints.Add(new Point(rect.Right, rect.Bottom));

            List<Line> vertsInWorld = VertsInWorld(Verts, UpperLeftPoint);

            foreach (var point in rectPoints)
            {
                if (PointInPoly(point, vertsInWorld))
                    return true;
            }
            return false;
        }

        public bool IntersectsPoly(Polygon poly)
        {
            List<Point> polyPoints = new List<Point>();
            List<Line> polyVertsInWorld = poly.VertsInWorld(poly.Verts, poly.UpperLeftPoint);
            foreach (var line in polyVertsInWorld)
                polyPoints.Add(new Point((int)line.Start.X, (int)line.Start.Y)); // don't need to test start and end because the next starting point will be the last ending point

            List<Line> vertsInWorld = VertsInWorld(Verts, UpperLeftPoint);
            foreach (var point in polyPoints)
            {
                if (PointInPoly(point, vertsInWorld))
                    return true;
            }
            return false;
        }

        private bool PointInPoly(Point point, List<Line> vertsInWorld)
        {
            bool inside = false;
            foreach (var side in vertsInWorld)
            {
                if (point.Y > Math.Min(side.Start.Y, side.End.Y))
                    if (point.Y <= Math.Max(side.Start.Y, side.End.Y))
                        if (point.X <= Math.Max(side.Start.X, side.End.X))
                        {
                            if (side.Start.Y != side.End.Y)
                            {
                                float xIntersection = (point.Y - side.Start.Y) * (side.End.X - side.Start.X) / (side.End.Y - side.Start.Y) + side.Start.X;
                                if (side.Start.X == side.End.X || point.X <= xIntersection)
                                    inside = !inside;
                            }
                        }
            }
            return inside;
        }

        public List<Line> VertsInWorld(List<Line> verts, Vector2 upperLeftLoc)
        {
            List<Line> vertsInWorld = new List<Line>();
            foreach(var vert in verts)
            {
                Line line = new Line();
                line.Start = new Vector2(upperLeftLoc.X + vert.Start.X, upperLeftLoc.Y + vert.Start.Y);
                line.End = new Vector2(upperLeftLoc.X + vert.End.X, upperLeftLoc.Y + vert.End.Y);
                vertsInWorld.Add(line);
            }
            return vertsInWorld;
        }

    }
}

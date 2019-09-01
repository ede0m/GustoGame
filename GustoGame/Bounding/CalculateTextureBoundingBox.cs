using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gusto.Bounds
{
    public class CalculateTextureBoundingBox
    {


        //Get smallest rectangle from Texture, cased on color
        public static Rectangle GetSmallestRectangleFromTexture(Texture2D Texture, float scale, float scaleBB)
        {
            //Create our index of sprite frames
            Color[,] Colors = TextureUtility.TextureTo2DArray(Texture);

            //determine the min/max bounds
            int x1 = 9999999, y1 = 9999999;
            int x2 = -999999, y2 = -999999;

            for (int a = 0; a < Texture.Width; a++)
            {
                for (int b = 0; b < Texture.Height; b++)
                {
                    //If we find a non transparent pixel, update bounds if required
                    if (Colors[a, b].A != 0)
                    {
                        if (x1 > a) x1 = a;
                        if (x2 < a) x2 = a;

                        if (y1 > b) y1 = b;
                        if (y2 < b) y2 = b;
                    }
                }
            }

            //We now have our smallest possible rectangle for this texture
            return new Rectangle(x1, y1, (int)((x2 - x1 + 1) * scale * scaleBB), (int)((y2 - y1 + 1) * scale * scaleBB)); // check for rounding errors here with scale

            //return new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
        }

        public static List<Line> CropTextureToPolygon(Texture2D Texture, float scale)
        {

            List<Vector2> verticies = new List<Vector2>();
            //Create our index of sprite frames
            Color[,] Colors = TextureUtility.TextureTo2DArray(Texture);

            for (int a = 0; a < Texture.Width; a++)
            {
                for (int b = 0; b < Texture.Height; b++)
                {
                    //If we find a non transparent pixel, update bounds if required
                    if (Colors[a, b].A != 0)
                    {
                        // if pixel before or after this pixel is transparent, we are on an edge pixel
                        if (Colors[a, b - 1].A == 0 || Colors[a, b + 1].A == 0 || Colors[a - 1, b].A == 0 || Colors[a + 1, b].A == 0)
                            verticies.Add(new Vector2(a * scale, b * scale));
                    }
                }
            }

            //We now have our smallest possible rectangle for this texture
            return ReducePolygon(ArrageVerticies(verticies));
            //return ArrageVerticies(verticies);
        }

        private static List<Line> ArrageVerticies(List<Vector2> verts)
        {
            List<Line> polygon = new List<Line>();
            Vector2 startingVertex = verts[0];
            Vector2 currentVertex = startingVertex;
            List<Vector2> pointsNotAttached = new List<Vector2>(verts);

            // we attach the current vertex to the nearest vertex, then remove them from pointsNotAttached
            while (pointsNotAttached.Count != 1)
            {

                // find min distance 
                double minDistance = 999999;
                Vector2 nearestVert = Vector2.Zero;
                foreach (var vert in pointsNotAttached)
                {
                    if (vert == currentVertex)
                        continue;

                    double distance = Math.Sqrt(Math.Pow((currentVertex.X - vert.X), 2) + Math.Pow((currentVertex.Y - vert.Y), 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestVert = vert;
                    }
                }

                // attach
                Line line = new Line();
                line.Start = currentVertex;
                line.End = nearestVert;
                polygon.Add(line);

                // remove and advance
                pointsNotAttached.Remove(currentVertex);
                currentVertex = nearestVert;
            }

            // attach last point to starting vertex
            Line finishingLine = new Line();
            finishingLine.Start = currentVertex;
            finishingLine.End = startingVertex;
            polygon.Add(finishingLine);

            return polygon;
        }

        private static List<Line> ReducePolygon(List<Line> lines)
        {
            List<Line> reducedPoly = new List<Line>();
            int maxPoints = 7;
            int window = lines.Count / maxPoints;

            for (int i = 0; i < maxPoints; i++)
            {
                List<Line> reduceLines = lines.GetRange(i * window, window);
                Line reducedLine = new Line();
                reducedLine.Start = reduceLines[0].Start;
                reducedLine.End = reduceLines[reduceLines.Count - 1].End;
                reducedPoly.Add(reducedLine);
            }

            // always start and end at the same spot to close the polygon
            reducedPoly[reducedPoly.Count - 1].End = reducedPoly[0].Start;
            return reducedPoly;
        }

    }
}

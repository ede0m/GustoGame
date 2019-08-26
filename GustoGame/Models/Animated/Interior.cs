using Comora;
using Gusto.AnimatedSprite.GameMap;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Interior
    {

        public string interiorKey;
        public HashSet<Sprite> interiorObjects; // anything placed or drop in this interior (Similar to ItemUtility.ItemsToUpdate except that is for world view)

        private JObject _interiorMapData;
        private List<TilePiece> interiorMap;
        public bool playerInInterior;

        private int width;
        private int height;
        private int cols;
        private int rows;

        Sprite interiorForObj;

        public Interior(string ik, Sprite interiorFor, ContentManager content, GraphicsDevice graphics)
        {
            interiorKey = ik;
            interiorMap = new List<TilePiece>();
            interiorForObj = interiorFor;
            // load the interiorMap tileset
            _interiorMapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\" + interiorKey + "Interior.json"));

            // set the interior map
            int multX = (int)_interiorMapData["multX"];
            int multY = (int)_interiorMapData["multY"];

            width = multX * GameOptions.PrefferedBackBufferWidth;
            height = multY * GameOptions.PrefferedBackBufferHeight;
            cols = width / (GameOptions.tileWidth * 2);
            rows = height / (GameOptions.tileHeight * 2);

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TilePiece tile = null;
                    JObject tileDetails = _interiorMapData["data"][index.ToString()].Value<JObject>();

                    // set interiorPiece piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "sd1":
                            tile = new ShipDeckTile(index, null, Vector2.Zero, "GameGusto", content, graphics, "shipDeckTile");
                            break;
                        case "sd1w":
                            tile = new ShipDeckTileWall(index, null, Vector2.Zero, "GameGusto", content, graphics, "shipDeckTileWall");
                            break;
                        case "si1":
                            tile = new ShipInteriorTile(index, null, Vector2.Zero, "GameGusto", content, graphics, "shipInteriorTile");
                            break;
                        case "si1w":
                            tile = new ShipInteriorTileWall(index, null, Vector2.Zero, "GameGusto", content, graphics, "shipInteriorTileWall");
                            break;
                    }

                    if (tile != null)
                        tile.SetTileDesignRow(RandomEvents.rand.Next(0, tile.nRows));

                    interiorMap.Add(tile);
                    index++;
                }
            }

            // Go through again to set all columns to correct frame for walls
            int index2 = 0;
            foreach (var tile in interiorMap)
            {
                if (tile == null)
                {
                    index2++;
                    continue;
                }

                if (tile.wallPiece)
                {
                    // left
                    if (interiorMap[index2 + 1] != null && interiorMap[index2 - 1] == null && !interiorMap[index2 + 1].wallPiece)
                        tile.currColumnFrame = 3; // left wall
                    // right
                    if (interiorMap[index2 - 1] != null && interiorMap[index2 + 1] == null && !interiorMap[index2 - 1].wallPiece)
                        tile.currColumnFrame = 1; // right wall
                    // bottom wall
                    if (interiorMap[index2 - cols] != null && interiorMap[index2 + cols] == null && !interiorMap[index2 - cols].wallPiece) // check above
                        tile.currColumnFrame = 2; // bottom wall
                    // top wall (looks the best)
                    if (interiorMap[index2 + cols] != null && interiorMap[index2 - cols] == null && !interiorMap[index2 + cols].wallPiece) // check below
                        tile.currColumnFrame = 0;
                }

                index2++;
            }
        }

        public void Draw(SpriteBatch sb, Camera cam)
        {

            // Draw the tileset
            Vector2 minCorner = new Vector2(cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            Vector2 startDrawPoint = new Vector2(interiorForObj.location.X - (width / 2), interiorForObj.location.Y - (height / 2));
            Vector2 drawPoint = startDrawPoint;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TilePiece tile = interiorMap[((cols) * i) + j];
                    if (tile == null)
                    {
                        drawPoint.X += GameOptions.tileWidth * 2;
                        continue;
                    }
                    tile.location = drawPoint;
                    var loc = tile.location;
                    if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                    {
                        tile.Draw(sb, cam);
                    }
                    drawPoint.X += GameOptions.tileWidth * 2;
                }
                drawPoint.Y += GameOptions.tileHeight * 2;
                drawPoint.X = startDrawPoint.X;
            }
        }

    }
}

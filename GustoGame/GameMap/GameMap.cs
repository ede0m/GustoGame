using Comora;
using Gusto.Models;
using Gusto.AnimatedSprite.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Gusto.Utility;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using GustoGame.GameMap;

namespace Gusto.GameMap
{

    public static class GameMapTiles
    {
        public static List<TilePiece> map = new List<TilePiece>();
        public static int rows;
        public static int cols;
    }

    public class TileGameMap
    {
        ContentManager _content;
        GraphicsDevice _graphics;

        private Camera _cam;
        private int _width;
        private int _height;
        private int _cols;
        private int _rows;

        private Dictionary<string, List<Sprite>> _regionMap = new Dictionary<string, List<Sprite>>();

        int tileHeight = GameOptions.tileHeight;
        int tileWidth = GameOptions.tileWidth;

        OceanWater oceanWater;
        RenderTarget2D waterScene;

        Vector2 startMapPoint;

        public TileGameMap(Camera camera)
        {
            _width = GameOptions.PrefferedBackBufferWidth * GameOptions.GameMapWidthMult;
            _height = GameOptions.PrefferedBackBufferHeight * GameOptions.GameMapHeightMult;
            _cols = _width / tileWidth;
            _rows = _height / tileHeight;
            GameMapTiles.rows = _rows;
            GameMapTiles.cols = _cols;
            AIUtility.LandPathWeights = new byte[_rows, _cols];
            AIUtility.OceanPathWeights = new byte[_rows, _cols];
            AIUtility.AllPathWeights = new byte[_rows, _cols];
            startMapPoint = new Vector2(0 - (_width/2), 0 - (_height/2));

            _cam = camera;
        }

        public void SetGameMap(ContentManager content, GraphicsDevice graphics, JObject mapData)
        {
            _content = content;
            _graphics = graphics;

            oceanWater = new OceanWater(_content, _graphics);
            waterScene = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height);

            var worldLoc = startMapPoint;
            int index = 0;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    TilePiece tile = null;
                    List<Sprite> groundObjects = null;
                    JObject tileDetails = mapData[index.ToString()].Value<JObject>();

                    // region
                    string regionName = (string)tileDetails["regionName"];
                    if (!BoundingBoxLocations.RegionMap.ContainsKey(regionName))
                        BoundingBoxLocations.RegionMap[regionName] = new Region(regionName);

                    // ground object
                    if ((string)tileDetails["sittingObject"] != "null")
                    {
                        groundObjects = GetGroundObjects((string)tileDetails["sittingObject"], regionName, worldLoc, content, graphics);

                        foreach (var groundObject in groundObjects)
                        {
                            groundObject.SetTileDesignRow(RandomEvents.rand.Next(0, groundObject.nRows));
                            groundObject.location.X += RandomEvents.rand.Next(-tileWidth, tileWidth);
                            groundObject.location.Y += RandomEvents.rand.Next(-tileHeight, tileHeight);
                        }
                    }

                    // set terrain piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "o1":
                            tile = new OceanTile(index, new Point(i, j), groundObjects, worldLoc, regionName, content, graphics, "o1");
                            tile.transparency = 0.6f;
                            tile.SetTileDesignRow(RandomEvents.rand.Next(0, tile.nRows));
                            AIUtility.OceanPathWeights[i, j] = 1;
                            AIUtility.LandPathWeights[i, j] = 0;
                            AIUtility.AllPathWeights[i, j] = 1;
                            BoundingBoxLocations.RegionMap[regionName].RegionOceanTiles.Add(tile);
                            break;
                        case "o2":
                            tile = new OceanTile(index, new Point(i, j), groundObjects, worldLoc, regionName, content, graphics, "o2");
                            tile.transparency = 0.7f;
                            AIUtility.OceanPathWeights[i, j] = 0;
                            AIUtility.LandPathWeights[i, j] = 1;
                            AIUtility.AllPathWeights[i, j] = 1;
                            //BoundingBoxLocations.RegionMap[regionName].RegionOceanTiles.Add(tile); omit these since they cause no path found in A*
                            break;
                        case "l1":
                            tile = new LandTile(index, new Point(i, j), groundObjects, worldLoc, regionName, content, graphics, "l1");
                            AIUtility.OceanPathWeights[i, j] = 0;
                            AIUtility.LandPathWeights[i, j] = 1;
                            AIUtility.AllPathWeights[i, j] = 1;
                            BoundingBoxLocations.RegionMap[regionName].RegionLandTiles.Add(tile);
                            break;
                        case "l2":
                            tile = new LandTile(index, new Point(i, j), groundObjects, worldLoc, regionName, content, graphics, "l2");
                            AIUtility.OceanPathWeights[i, j] = 0;
                            AIUtility.LandPathWeights[i, j] = 1;
                            AIUtility.AllPathWeights[i, j] = 1;
                            BoundingBoxLocations.RegionMap[regionName].RegionLandTiles.Add(tile);
                            break;
                    }

                    worldLoc.X += tileWidth;
                    GameMapTiles.map.Add(tile);
                    index++;
                }
                worldLoc.Y += tileHeight;
                worldLoc.X = startMapPoint.X;
            }

            // Go through again to set all columns to correct frame for land tiles
            int index2 = 0;
            foreach (var tile in GameMapTiles.map)
            {
                if (tile is ILand)
                {
                    // inner land
                    if (GameMapTiles.map[index2 + 1] is ILand && GameMapTiles.map[index2 - 1] is ILand && GameMapTiles.map[index2 + _cols] is ILand && GameMapTiles.map[index2 - _cols] is ILand)
                    {
                        tile.shorePiece = false;
                        tile.currRowFrame = 0;
                    }
                    else
                    {
                        // land tile rounding
                        tile.shorePiece = true;

                        if (GameMapTiles.map[index2 + 1] is ILand && !(GameMapTiles.map[index2 - 1] is ILand))
                            tile.currRowFrame = 1; // left shore
                        if (GameMapTiles.map[index2 - _cols] is ILand && !(GameMapTiles.map[index2 + _cols] is ILand)) // check above
                            tile.currRowFrame = 4; // bottom shore
                        if (GameMapTiles.map[index2 + _cols] is ILand && !(GameMapTiles.map[index2 - _cols] is ILand)) // check below
                            tile.currRowFrame = 2; // top shore
                        if (GameMapTiles.map[index2 - 1] is ILand && !(GameMapTiles.map[index2 + 1] is ILand))
                            tile.currRowFrame = 3; // right shore
                        if (GameMapTiles.map[index2 + 1] is ILand && !(GameMapTiles.map[index2 - 1] is ILand) && !(GameMapTiles.map[index2 - _cols] is ILand))
                            tile.currRowFrame = 5; // left top corner shore
                        if (GameMapTiles.map[index2 + 1] is ILand && !(GameMapTiles.map[index2 - 1] is ILand) && !(GameMapTiles.map[index2 + _cols] is ILand))
                            tile.currRowFrame = 8; // left bottom corner shore
                        if (GameMapTiles.map[index2 - 1] is ILand && !(GameMapTiles.map[index2 + 1] is ILand) && !(GameMapTiles.map[index2 - _cols] is ILand))
                            tile.currRowFrame = 6; // right top corner shore
                        if (GameMapTiles.map[index2 - 1] is ILand && !(GameMapTiles.map[index2 + 1] is ILand) && !(GameMapTiles.map[index2 + _cols] is ILand))
                            tile.currRowFrame = 7; // right bottom corner shore


                        tile.SetTileDesignColumn(RandomEvents.rand.Next(0, tile.nColumns));
                    }
                }

                index2++;
            }
        }

        private List<Sprite> GetGroundObjects(string key, string region, Vector2 loc, ContentManager content, GraphicsDevice graphics)
        {
            List<Sprite> groundObjs = new List<Sprite>();
            switch (key)
            {
                case "t1":
                    groundObjs.Add( new Tree1(TeamType.GroundObject, region, loc, content, graphics));
                    break;
                case "t2":
                    groundObjs.Add(new Tree2(TeamType.GroundObject, region, loc, content, graphics));
                    break;
                case "t3":
                    groundObjs.Add(new Tree3(TeamType.GroundObject, region, loc, content, graphics));
                    break;
                case "gr1":
                    groundObjs.Add(new Grass1(TeamType.GroundObject, region, loc, content, graphics));
                    groundObjs.Add(new Grass1(TeamType.GroundObject, region, loc, content, graphics)); 
                    //groundObjs.Add(new Grass1(TeamType.GroundObject, region, loc, content, graphics)); // three grasses encoded per tile
                    break;
                case "rk1":
                    groundObjs.Add(new Rock1(TeamType.GroundObject, region, loc, content, graphics));
                    break;
            }
            return groundObjs;
        }

        public void DrawMap(SpriteBatch sbWorld, SpriteBatch sbStatic, RenderTarget2D worldScene, GameTime gameTime)
        {

            // Set Water RenderTarget
            _graphics.SetRenderTarget(waterScene);
            _graphics.Clear(Color.CornflowerBlue);
            sbWorld.Begin(_cam, SpriteSortMode.Texture);
            foreach (var t in BoundingBoxLocations.OceanTileLocationList)
            {
                TilePiece tile = (TilePiece)t;
                tile.DrawTile(sbWorld);
            }
            sbWorld.End();

            // set up gamescene draw
            _graphics.SetRenderTarget(worldScene);
            _graphics.Clear(Color.PeachPuff);

            // water
            oceanWater.Draw(sbWorld, waterScene);

            // land
            sbWorld.Begin(_cam, SpriteSortMode.Texture);
            foreach (var t in BoundingBoxLocations.LandTileLocationList)
            {
                TilePiece tile = (TilePiece)t;
                tile.DrawTile(sbWorld);
            }
            sbWorld.End();

            /*_graphics.SetRenderTarget(worldScene);
            _graphics.Clear(Color.PeachPuff);
            sbWorld.Begin(_cam);
            foreach (var tile in BoundingBoxLocations.TilesInView)
            {
                tile.DrawTile(sbWorld);
            }
            sbWorld.End();*/

        }

    }
}

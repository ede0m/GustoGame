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

namespace Gusto.GameMap
{
    public class TileGameMap
    {
        ContentManager _content;
        GraphicsDevice _graphics;

        private Camera _cam;
        private int _width;
        private int _height;
        private int _cols;
        private int _rows;

        private List<TilePiece> map;
        private Dictionary<string, List<Sprite>> _regionMap = new Dictionary<string, List<Sprite>>();
        private JObject _mapData;

        int tileHeight = GameOptions.tileHeight;
        int tileWidth = GameOptions.tileWidth;
        Vector2 startMapPoint;

        public TileGameMap(Camera camera)
        {
            _width = GameOptions.PrefferedBackBufferWidth * GameOptions.GameMapWidthMult;
            _height = GameOptions.PrefferedBackBufferHeight * GameOptions.GameMapHeightMult;
            _cols = _width / tileWidth;
            _rows = _height / tileHeight;
            startMapPoint = new Vector2(0 - (_width/2), 0 - (_height/2));

            _cam = camera;
            map = new List<TilePiece>();
        }

        public void SetGameMap(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            var worldLoc = startMapPoint;
            int index = 0;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    TilePiece tile = null;
                    Sprite groundObject = null;
                    JObject tileDetails = _mapData[index.ToString()].Value<JObject>();

                    // region
                    string regionName = (string)tileDetails["regionName"];
                    if (!BoundingBoxLocations.RegionMap.ContainsKey(regionName))
                        BoundingBoxLocations.RegionMap[regionName] = new Region(regionName);

                    // ground object
                    if ((string)tileDetails["sittingObject"] != "null")
                    {
                        groundObject = GetGroundObject((string)tileDetails["sittingObject"], regionName, worldLoc, content, graphics);
                        groundObject.SetTileDesignRow(RandomEvents.rand.Next(0, groundObject.nRows));
                        groundObject.location.X += RandomEvents.rand.Next(-tileWidth, tileWidth);
                        groundObject.location.Y += RandomEvents.rand.Next(-tileHeight, tileHeight);
                    }

                    // set terrain piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "o1":
                            tile = new OceanTile(index, groundObject, worldLoc, regionName, content, graphics, "o1");
                            tile.transparency = 0.6f;
                            tile.SetTileDesignRow(RandomEvents.rand.Next(0, tile.nRows));
                            BoundingBoxLocations.RegionMap[regionName].RegionOceanTiles.Add(tile);
                            break;
                        case "o2":
                            tile = new OceanTile(index, groundObject, worldLoc, regionName, content, graphics, "o2");
                            tile.transparency = 0.6f;
                            BoundingBoxLocations.RegionMap[regionName].RegionOceanTiles.Add(tile);
                            break;
                        case "l1":
                            tile = new LandTile(index, groundObject, worldLoc, regionName, content, graphics, "l1");
                            BoundingBoxLocations.RegionMap[regionName].RegionLandTiles.Add(tile);
                            break;
                    }

                    worldLoc.X += tileWidth;
                    map.Add(tile);
                    index++;
                }
                worldLoc.Y += tileHeight;
                worldLoc.X = startMapPoint.X;
            }

            // Go through again to set all columns to correct frame for walls
            int index2 = 0;
            foreach (var tile in map)
            {
                if (tile is ILand)
                {
                    // inner land
                    if (map[index2 + 1] is ILand && map[index2 - 1] is ILand && map[index2 + _cols] is ILand && map[index2 - _cols] is ILand)
                    {
                        tile.shorePiece = false;
                        tile.currRowFrame = 0;
                    }
                    else
                    {

                        tile.shorePiece = true;

                        if (map[index2 + 1] is ILand && !(map[index2 - 1] is ILand))
                            tile.currRowFrame = 1; // left shore
                        if (map[index2 - _cols] is ILand && !(map[index2 + _cols] is ILand)) // check above
                            tile.currRowFrame = 4; // bottom shore
                        if (map[index2 + _cols] is ILand && !(map[index2 - _cols] is ILand)) // check below
                            tile.currRowFrame = 2; // top shore
                        if (map[index2 - 1] is ILand && !(map[index2 + 1] is ILand))
                            tile.currRowFrame = 3; // right shore
                        if (map[index2 + 1] is ILand && !(map[index2 - 1] is ILand) && !(map[index2 - _cols] is ILand))
                            tile.currRowFrame = 5; // left top corner shore
                        if (map[index2 + 1] is ILand && !(map[index2 - 1] is ILand) && !(map[index2 + _cols] is ILand))
                            tile.currRowFrame = 8; // left bottom corner shore
                        if (map[index2 - 1] is ILand && !(map[index2 + 1] is ILand) && !(map[index2 - _cols] is ILand))
                            tile.currRowFrame = 6; // right top corner shore
                        if (map[index2 - 1] is ILand && !(map[index2 + 1] is ILand) && !(map[index2 + _cols] is ILand))
                            tile.currRowFrame = 7; // right bottom corner shore


                        tile.SetTileDesignColumn(RandomEvents.rand.Next(0, tile.nColumns));
                    }
                }

                index2++;
            }
        }


        private Sprite GetGroundObject(string key, string region, Vector2 loc, ContentManager content, GraphicsDevice graphics)
        {
            switch (key)
            {
                case "t1":
                    return new Tree1(TeamType.GroundObject, region, loc, content, graphics);
                case "gr1":
                    return new Grass1(TeamType.GroundObject, region, loc, content, graphics);
                case "rk1":
                    return new Rock1(TeamType.GroundObject, region, loc, content, graphics);
            }
            return null;
        }

        public void DrawMap(SpriteBatch sb, GameTime gameTime)
        {
            OceanTile shoreOceanTile = new OceanTile(0, null, Vector2.Zero, "GustoGame", _content, _graphics, "o2");
            shoreOceanTile.transparency = 0.6f;
            Vector2 minCorner = new Vector2(_cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(_cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            sb.Begin(_cam);
            foreach (var tile in BoundingBoxLocations.TilesInView)
            {
                // draw water under shore pieces so transparent backbuffer doesn't show through
                if (tile.shorePiece)
                {
                    shoreOceanTile.location = tile.location;
                    shoreOceanTile.DrawTile(sb);
                }

                tile.DrawTile(sb);
            }
            sb.End();
        }

        public void LoadMapData(JObject data)
        {
            _mapData = data;
        }

        public List<TilePiece> GetMap()
        {
            return map;
        }
    }
}

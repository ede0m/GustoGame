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
                    tile.SetTileDesignRow(RandomEvents.rand.Next(0, tile.nRows));

                    worldLoc.X += tileWidth;
                    map.Add(tile);
                    index++;
                }
                worldLoc.Y += tileHeight;
                worldLoc.X = startMapPoint.X;
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

            Vector2 minCorner = new Vector2(_cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(_cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));
            foreach (var tile in BoundingBoxLocations.TilesInView)
            {
                tile.Draw(sb, _cam);
            }
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

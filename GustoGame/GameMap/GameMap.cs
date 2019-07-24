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

namespace Gusto.GameMap
{
    public class TileGameMap
    {
        private Camera _cam;
        private int _width;
        private int _height;
        private int _cols;
        private int _rows;

        private List<Sprite> map;
        private List<Sprite> collidablePieces;
        private Dictionary<string, List<Sprite>> _regionMap = new Dictionary<string, List<Sprite>>();
        private JObject _mapData;

        const int tileHeight = 32;
        const int tileWidth = 32;
        Vector2 startMapPoint;

        Random rand;

        public TileGameMap(Camera camera)
        {

            _width = GameOptions.PrefferedBackBufferWidth * GameOptions.GameMapWidthMult;
            _height = GameOptions.PrefferedBackBufferHeight * GameOptions.GameMapHeightMult;
            _cols = _width / tileWidth;
            _rows = _height / tileHeight;
            startMapPoint = new Vector2(0 - (_width/2), 0 - (_height/2));

            _cam = camera;
            map = new List<Sprite>();
            collidablePieces = new List<Sprite>();
            rand = new Random();
        }

        public void SetGameMap(ContentManager content, GraphicsDevice graphics)
        {
            var worldLoc = startMapPoint;
            int index = 0;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    Sprite tile = null;
                    Sprite groundObject = null;
                    JObject tileDetails = _mapData[index.ToString()].Value<JObject>();

                    // region
                    string regionName = (string)tileDetails["regionName"];
                    if (!BoundingBoxLocations.RegionMap.ContainsKey(regionName))
                        BoundingBoxLocations.RegionMap[regionName] = new List<Sprite>();

                    // ground object
                    if ((string)tileDetails["sittingObject"] != "null")
                    {
                        groundObject = GetGroundObject((string)tileDetails["sittingObject"], regionName, worldLoc, content, graphics);
                        groundObject.SetTileDesignRow(RandomEvents.RandomSelection(groundObject.nRows, rand));
                        groundObject.location.X += RandomEvents.RandomSelectionRange(tileWidth, rand);
                        groundObject.location.Y += RandomEvents.RandomSelectionRange(tileHeight, rand);
                    }

                    // set terrain piece
                    switch (tileDetails["terrainPiece"].ToString())
                    {
                        case "o1":
                            tile = new OceanTile(groundObject, worldLoc, regionName, content, graphics, "o1");
                            BoundingBoxLocations.RegionMap[regionName].Add(tile);
                            break;
                        case "o2":
                            tile = new OceanTile(groundObject, worldLoc, regionName, content, graphics, "o2");
                            BoundingBoxLocations.RegionMap[regionName].Add(tile);
                            break;
                        case "l1":
                            tile = new LandTile(groundObject, worldLoc, regionName, content, graphics, "l1");
                            BoundingBoxLocations.RegionMap[regionName].Add(tile);
                            break;
                    }
                    tile.SetTileDesignColumn(RandomEvents.RandomSelection(tile.nColumns, rand));

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
            }
            return null;
        }

        public void DrawMap(SpriteBatch sb)
        {

            Vector2 minCorner = new Vector2(_cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(_cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));
            BoundingBoxLocations.LandTileLocationList.Clear();
            BoundingBoxLocations.GroundObjectLocationList.Clear();
            //collidablePieces.Clear();
            foreach (var tile in map)
            {
                var loc = tile.location;
                if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                {
                    if (tile.bbKey.Equals("landTile"))
                    {
                        BoundingBoxLocations.LandTileLocationList.Add(tile);
                    }
                        //collidablePieces.Add(tile);
                    tile.Draw(sb, _cam);

                    TilePiece tileP = (TilePiece)tile;
                    if (tileP.groundObject != null)
                        BoundingBoxLocations.GroundObjectLocationList.Add(tileP.groundObject);

                }
            }
        }

        public void LoadMapData(JObject data)
        {
            _mapData = data;
        }
    }
}

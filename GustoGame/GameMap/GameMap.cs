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
                    JObject tileDetails = _mapData[index.ToString()].Value<JObject>();
                    string regionName = (string)tileDetails["regionName"];
                    if (!_regionMap.ContainsKey(regionName))
                        _regionMap[regionName] = new List<Sprite>();

                    switch(tileDetails["terrainPiece"].ToString())
                    {
                        case "o1":
                            tile = new OceanTile(worldLoc, content, graphics, "o1");
                            _regionMap[regionName].Add(tile);
                            break;
                        case "o2":
                            tile = new OceanTile(worldLoc, content, graphics, "o2");
                            _regionMap[regionName].Add(tile);
                            break;
                        case "l1":
                            tile = new LandTile(worldLoc, content, graphics, "l1");
                            _regionMap[regionName].Add(tile);
                            break;
                    }

                    tile.SetTileDesignColumn(RandomEvents.RandomTilePiece(tile.nColumns, rand));
                    worldLoc.X += tileWidth;
                    map.Add(tile);
                    index++;
                }
                worldLoc.Y += tileHeight;
                worldLoc.X = startMapPoint.X;
            }
        }

        public void DrawMap(SpriteBatch sb)
        {

            Vector2 minCorner = new Vector2(_cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(_cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), _cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));
            BoundingBoxLocations.LandTileLocationList.Clear();
            //collidablePieces.Clear();
            foreach (var tile in map)
            {
                var loc = tile.location;
                if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                {
                    if (tile.bbKey.Equals("landTile"))
                        BoundingBoxLocations.LandTileLocationList.Add(tile);
                        //collidablePieces.Add(tile);
                    tile.Draw(sb, _cam);
                }
            }
        }

        public void LoadMapData(JObject data)
        {
            _mapData = data;
        }

        public List<Sprite> GetCollidableTiles()
        {
            return collidablePieces;
        }

        public Dictionary<string, List<Sprite>> GetRegionMap()
        {
            return _regionMap;
        }
    }
}

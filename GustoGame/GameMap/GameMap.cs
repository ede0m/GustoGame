﻿using Comora;
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
        private JObject _mapData;

        const int tileHeight = 64;
        const int tileWidth = 64;
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
                    switch(_mapData[index.ToString()].ToString())
                    {
                        case "o1":
                            tile = new OceanTile(worldLoc, content, graphics);
                            break;
                        case "l1":
                            tile = new LandTile(worldLoc, content, graphics);
                            break;
                    }

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
            foreach (var tile in map)
            {
                var loc = tile.location;
                if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                    tile.Draw(sb, _cam);
            }
        }

        public void LoadMapData(JObject data)
        {
            _mapData = data;
        }
    }
}

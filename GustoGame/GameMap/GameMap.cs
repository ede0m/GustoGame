using Comora;
using Gusto.Models;
using GustoGame.AnimatedSprite.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public class GameMapProcedural
    {
        private Camera _cam;
        private int _width;
        private int _height;
        private List<TilePiece> map;
        const int tileHeight = 64;
        const int tileWidth = 64;
        Vector2 startMapPoint;

        public GameMapProcedural(Camera camera)
        {
            _width = GameOptions.PrefferedBackBufferWidth * GameOptions.GameMapWidthMult;
            _height = GameOptions.PrefferedBackBufferHeight * GameOptions.GameMapHeightMult;
            startMapPoint = new Vector2(0 - _width, 0 - _height);
            _cam = camera;
            map = new List<TilePiece>();
        }

        public void SetGameMap(ContentManager content, GraphicsDevice graphics)
        {
            var worldLoc = startMapPoint;
            var cols = _width / tileWidth;
            var rows = _height / tileHeight;
            for (int i = 0; i <= (cols * rows) * 4; i++) // 4 quads
            {
                var oceanTile = new OceanTile(worldLoc, content, graphics);
                worldLoc.X += (oceanTile.GetWidth());
                if (worldLoc.X >= _width)
                {
                    worldLoc.Y += (oceanTile.GetHeight());
                    worldLoc.X = startMapPoint.X;
                }
                map.Add(oceanTile);
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
    }
}

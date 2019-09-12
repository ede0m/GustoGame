
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Bounds;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;

namespace Gusto.AnimatedSprite.GameMap
{
    public class CanvasTileWall : TilePiece, IInteriorTile
    {
        public CanvasTileWall(int index, Point? p, List<Sprite> groundObjs, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base(index, p, groundObjs, content, graphics)
        {
            Texture2D textureTile = content.Load<Texture2D>("CanvasInteriorWall");
            Asset tileAsset = new Asset(textureTile, null, 4, 1, 1.0f, "interiorTileWall", region);
            SetSpriteAsset(tileAsset, location);
            wallPiece = true;
        }

    }
}


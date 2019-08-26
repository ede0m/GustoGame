
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

namespace Gusto.AnimatedSprite.GameMap
{
    public class ShipInteriorTile : TilePiece
    {
        public ShipInteriorTile(int index, Sprite groundObj, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base(index, groundObj, content, graphics)
        {
            Texture2D textureTile = content.Load<Texture2D>("ShipInterior");
            Asset tileAsset = new Asset(textureTile, null, 1, 1, 1.0f, "shipInteriorTile", region);
            SetSpriteAsset(tileAsset, location);
        }

    }
}


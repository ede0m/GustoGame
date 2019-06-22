
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
    public class LandTile : TilePiece
    {
        public LandTile(Sprite groundObj, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base (groundObj)
        {
            Texture2D textureLandTile = content.Load<Texture2D>("Land1");
            Asset landTileAsset = new Asset(textureLandTile, null, 4, 1, 1.0f, "landTile", region);
            SetSpriteAsset(landTileAsset, location);
        }

    }
}


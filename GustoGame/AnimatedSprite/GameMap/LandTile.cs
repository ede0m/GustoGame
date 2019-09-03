
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
    public class LandTile : TilePiece, ILand
    {
        public LandTile(int index, Sprite groundObj, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base (index, groundObj, content, graphics)
        {
            Texture2D textureLandTile = null;

            if (key.Equals("l1"))
                textureLandTile = content.Load<Texture2D>("Land1HolesShore");
            else if (key.Equals("l2"))
                textureLandTile = content.Load<Texture2D>("Land2HolesShore");

            Asset landTileAsset = new Asset(textureLandTile, null, 4, 9, 1.0f, "landTile", region);
            SetSpriteAsset(landTileAsset, location);
        }

    }
}



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
        public LandTile(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {

            //MapModelMovementVectorValues();
            Texture2D textureOceanTile = content.Load<Texture2D>("Land1");
            Asset oceanTileAsset = new Asset(textureOceanTile, null, 1, 1, 1.0f, "landTile");
            SetSpriteAsset(oceanTileAsset, location);
        }

    }
}


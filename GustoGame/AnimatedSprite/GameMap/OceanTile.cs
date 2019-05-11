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

namespace GustoGame.AnimatedSprite.GameMap
{
    public class OceanTile : TilePiece
    {
        public OceanTile(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {

            //MapModelMovementVectorValues();
            Texture2D textureOceanTile = content.Load<Texture2D>("Ocean1");
            Asset oceanTileAsset = new Asset(textureOceanTile, null, 1, 1, 1.0f, "oceanTile");
            SetSpriteAsset(oceanTileAsset, location);
        }

    }
}


﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Gusto.Models;

namespace Gusto.AnimatedSprite.GameMap
{
    public class OceanTile : TilePiece
    {
        public OceanTile(int index, Point? p, List<Sprite> groundObjs, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base (index, p, groundObjs, content, graphics)
        {
            Texture2D textureOceanTile = null;

            if (key.Equals("o1"))
                textureOceanTile = content.Load<Texture2D>("Ocean1v3");
            else if (key.Equals("oD"))
                textureOceanTile = content.Load<Texture2D>("OceanD");
            else if (key.Equals("o2"))
            {
                textureOceanTile = content.Load<Texture2D>("Ocean2v3");
                shallowWaterPiece = true;
            }

            Asset oceanTileAsset = new Asset(textureOceanTile, null, 1, 4, 1.0f, "oceanTile", region);
            SetSpriteAsset(oceanTileAsset, location);
        }

    }
}


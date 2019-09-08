
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
    public class DirtTile : TilePiece, IInteriorTile
    {
        public DirtTile(int index, List<Sprite> groundObjs, Vector2 location, string region, ContentManager content, GraphicsDevice graphics, string key) : base(index, groundObjs, content, graphics)
        {
            Texture2D textureTile = content.Load<Texture2D>("DirtInterior");
            Asset tileAsset = new Asset(textureTile, null, 1, 1, 1.0f, "interiorTile", region);
            SetSpriteAsset(tileAsset, location);
        }

    }
}


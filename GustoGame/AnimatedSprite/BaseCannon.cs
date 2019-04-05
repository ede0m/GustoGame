using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite
{
    public class BaseCannon : Cannon
    {
        public BaseCannon(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            timeSinceLastFrame = 0;

            Texture2D textureBaseCannon = content.Load<Texture2D>("BaseCannon");
            Texture2D textureBaseCannonBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseCannonBB = new Texture2D(graphics, textureBaseCannon.Width, textureBaseCannon.Height);
            Asset baseCannonAsset = new Asset(textureBaseCannon, textureBaseCannonBB, 1, 8, 1.0f, "baseCannon");
            SetSpriteAsset(baseCannonAsset, location);
        }
    }
}

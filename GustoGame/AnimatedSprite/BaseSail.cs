using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite
{
    public class BaseSail : Sail
    {

        public BaseSail(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 300; // turn speed
            sailIsLeftColumn = 2;
            sailIsRightColumn = 0;
            sailSpeed = 1.5f;
            windWindowAdd = 1;
            windWindowSub = 1;

            Texture2D textureBaseSail = content.Load<Texture2D>("DecomposedBaseSail");
            Texture2D textureBaseSailBB = null;
            Asset baseSailAsset = new Asset(textureBaseSail, textureBaseSailBB, 3, 8, 0.6f, "baseSail");
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseSailBB = new Texture2D(graphics, textureBaseSail.Width, textureBaseSail.Height);

            SetSpriteAsset(baseSailAsset, location);
        }

    }
}

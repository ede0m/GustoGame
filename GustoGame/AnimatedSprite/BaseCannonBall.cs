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
    public class BaseCannonBall : CannonBall
    {
        public BaseCannonBall(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 2.0f;

            Texture2D textureBaseCannonBall = content.Load<Texture2D>("CannonBall");
            Texture2D textureBaseCannonBallBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseCannonBallBB = new Texture2D(graphics, textureBaseCannonBall.Width, textureBaseCannonBall.Height);
            Asset baseCannonBallAsset = new Asset(textureBaseCannonBall, textureBaseCannonBallBB, 2, 1, 1.0f, "baseCannonBall");
            SetSpriteAsset(baseCannonBallAsset, location);
        }
    }
}

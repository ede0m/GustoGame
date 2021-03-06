﻿using Gusto.Models;
using Gusto.Models.Animated;
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
    public class BaseCannonBall : Ammo
    {
        public BaseCannonBall(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base (location, team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 2.0f;
            structureDamage = 5.0f;
            groundDamage = 10.0f;

            Texture2D textureBaseCannonBall = content.Load<Texture2D>("CannonBall");
            Texture2D textureBaseCannonBallBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseCannonBallBB = new Texture2D(graphics, textureBaseCannonBall.Width, textureBaseCannonBall.Height);
            Asset baseCannonBallAsset = new Asset(textureBaseCannonBall, textureBaseCannonBallBB, 2, 1, 1.0f, "baseCannonBall", region);
            SetSpriteAsset(baseCannonBallAsset, location);
        }
    }
}

﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Bounds;


namespace Gusto.AnimatedSprite
{
    public class BaseTribal : EnemyGround
    {
        public BaseTribal(TeamType team, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastTurnFrame = 0;
            timeSinceLastWalkFrame = 0;
            millisecondsPerTurnFrame = 500; // turn speed
            millisecondsPerWalkFrame = 100; // turn speed
            millisecondsCombatSwing = 75;

            Texture2D textureBaseTribal = content.Load<Texture2D>("Tribal1");
            Texture2D textureBaseTribalBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseTribalBB = new Texture2D(graphics, textureBaseTribal.Width, textureBaseTribal.Height);
            Asset baseTribalAsset = new Asset(textureBaseTribal, textureBaseTribalBB, 12, 4, 0.8f, "baseTribal");
            SetSpriteAsset(baseTribalAsset, location);
        }
    }

}

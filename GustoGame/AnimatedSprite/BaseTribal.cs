﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Bounds;
using Gusto.Utility;

namespace Gusto.AnimatedSprite
{
    public class BaseTribal : GroundEnemy
    {
        public BaseTribal(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastTurnFrame = 0;
            timeSinceLastWalkFrame = 0;
            millisecondToDie = 10000;
            millisecondsPerTurnFrame = 500; // turn speed
            millisecondsPerWalkFrame = 100; // turn speed
            millisecondsCombatSwing = 75;

            fullHealth = 25;
            health = fullHealth;
            damage = 0.05f;

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPCDrops(team, rand, 3);
            inventory = ItemUtility.CreateNPCInventory(itemDrops, team, region, location, content, graphics);

            Texture2D textureBaseTribal = content.Load<Texture2D>("Tribal1");
            Texture2D textureBaseTribalBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseTribalBB = new Texture2D(graphics, textureBaseTribal.Width, textureBaseTribal.Height);
            Asset baseTribalAsset = new Asset(textureBaseTribal, textureBaseTribalBB, 12, 4, 1.0f, "baseTribal", region);
            SetSpriteAsset(baseTribalAsset, location);
        }
    }

}

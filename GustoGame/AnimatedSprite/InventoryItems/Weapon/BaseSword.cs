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

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class BaseSword : HandHeld
    {
        public BaseSword(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.2f;
            itemKey = "baseSword";
            msCraftTime = 5000;

            Texture2D textureBaseSword = content.Load<Texture2D>("BaseSword");
            Texture2D textureBaseSwordBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseSwordBB = new Texture2D(graphics, textureBaseSword.Width, textureBaseSword.Height);
            Asset baseSwordAsset = new Asset(textureBaseSword, textureBaseSwordBB, 3, 4, 1.0f, "baseSword", region);
            SetSpriteAsset(baseSwordAsset, location);

            stackable = false;
        }
    }
}

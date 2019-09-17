﻿using Gusto.Models;
using Gusto.Models.Animated;
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
    public class ShortSail : Sail
    {

        public ShortSail(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, graphics)
        {
            timeSinceLastFrame = 0;
            sailIsLeftColumn = 2;
            sailIsRightColumn = 0;
            sailSpeed = 1.3f;
            windWindowAdd = 1;
            windWindowSub = 1;

            string spriteSheetName = null;
            if (team == TeamType.A)
                spriteSheetName = "ShortSail";
            else if (team == TeamType.Player)
                spriteSheetName = "ShortSail";

            Texture2D textureBaseSail = content.Load<Texture2D>(spriteSheetName);
            Texture2D textureBaseSailBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseSailBB = new Texture2D(graphics, textureBaseSail.Width, textureBaseSail.Height);
            Asset baseSailAsset = new Asset(textureBaseSail, textureBaseSailBB, 3, 8, 1.1f, "shortSail", region);
            SetSpriteAsset(baseSailAsset, location);
        }

    }
}
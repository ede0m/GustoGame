using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Bounds;
using Gusto.Utility;
using Gusto.Models.Animated;

namespace Gusto.AnimatedSprite
{
    public class Snake : Npc
    {
        public Snake(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            millisecondToDie = 10000;
            millisecondsPerTurnFrame = 500; // turn speed
            millisecondsPerWalkFrame = 300; // turn speed
            millisecondsCombatMove = 75;
            msIdleWaitTime = 6000;

            combatFrameIndex = 6;

            fullHealth = 25;
            health = fullHealth;
            damage = 0.05f;
            string objKey = "snake";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 3);
            inventory = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D texture = content.Load<Texture2D>("Snake1");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 9, 5, 0.7f, objKey, region);
            SetSpriteAsset(asset, location);
        }
    }

}

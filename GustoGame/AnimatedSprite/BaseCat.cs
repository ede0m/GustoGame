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
    public class BaseCat : Npc
    {
        public BaseCat(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastTurnFrame = 0;
            timeSinceLastWalkFrame = 0;
            millisecondToDie = 10000;
            millisecondsPerTurnFrame = 400; // turn speed
            millisecondsPerWalkFrame = 100; // turn speed
            millisecondsCombatMove = 75;

            fullHealth = 50;
            health = fullHealth;
            damage = 0.08f;
            string objKey = "baseCat";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 3);
            inventory = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D texture = content.Load<Texture2D>("Cat1");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 12, 4, 0.7f, objKey, region);
            SetSpriteAsset(asset, location);
        }
    }

}

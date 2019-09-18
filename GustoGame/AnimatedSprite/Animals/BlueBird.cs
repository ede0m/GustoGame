using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Gusto.Models;
using Gusto.Utility;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Models.Types;

namespace Gusto.AnimatedSprite
{
    public class BlueBird : Npc, IFlying
    {
        public BlueBird(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            millisecondToDie = 10000;
            millisecondsPerTurnFrame = 400; // turn speed
            millisecondsPerWalkFrame = 200; // turn speed
            millisecondsCombatMove = 75;
            msIdleWaitTime = 6000;

            nIdleRowFrames = 1;

            fullHealth = 25;
            health = fullHealth;
            damage = 0.05f;
            actionState = ActionState.IdleFlee;
            string objKey = "blueBird";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 3);
            inventory = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D texture = content.Load<Texture2D>("BlueBird");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 4, 5, 0.4f, objKey, region);
            SetSpriteAsset(asset, location);
        }
    }

}

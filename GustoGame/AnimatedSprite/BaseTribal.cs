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
using Gusto.Models.Types;

namespace Gusto.AnimatedSprite
{
    public class BaseTribal : Npc
    {
        public BaseTribal(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            millisecondToDie = 10000;
            millisecondsPerTurnFrame = 500; // turn speed
            millisecondsPerWalkFrame = 100; // turn speed
            millisecondsCombatMove = 75;

            combatFrameIndex = 7;

            fullHealth = 25;
            health = fullHealth;
            damage = 0.05f;
            actionState = ActionState.DefenseRoam;
            string objKey = "baseTribal";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 3);
            inventory = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D textureBaseTribal = content.Load<Texture2D>("Tribal1");
            Texture2D textureBaseTribalBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseTribalBB = new Texture2D(graphics, textureBaseTribal.Width, textureBaseTribal.Height);
            Asset baseTribalAsset = new Asset(textureBaseTribal, textureBaseTribalBB, 12, 4, 1.0f, objKey, region);
            SetSpriteAsset(baseTribalAsset, location);
        }
    }

}

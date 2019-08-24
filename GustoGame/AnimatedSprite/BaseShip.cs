using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Models.Menus;
using Gusto.Models.Animated;
using System.Linq;
using Gusto.Utility;

namespace Gusto.AnimatedSprite
{
    public class BaseShip : Ship
    {
        public BaseShip(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base (team, content, graphics)
        {
            timeSinceLastTurn = 0;
            millisecondsPerTurn = 500; // turn speed
            timeSinceStartAnchor = 0;
            millisecondsToAnchor = 1000;
            msToRepair = 5000;
            timeSinceLastExpClean = 0;
            millisecondsExplosionLasts = 400;
            timeSinceLastShot = 0;
            millisecondsNewShot = 2000;
            movementSpeed = 0.2f;
            timeSinceStartSinking = 0;
            millisecondToSink = 10000;
            nSails = 1;
            fullHealth = 40;
            health = fullHealth;
            shotRange = 600f;
            stopRange = 260f;
            attackRange = 400f;
            maxInventorySlots = 15;

            string objKey = "baseShip";

            //MapModelMovementVectorValues();
            Texture2D textureBaseShip = content.Load<Texture2D>("BaseShip");
            Texture2D textureBaseShipBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseShipBB = new Texture2D(graphics, textureBaseShip.Width, textureBaseShip.Height);
            Asset baseShipAsset = new Asset(textureBaseShip, textureBaseShipBB, 1, 8, 0.6f, objKey, region);

            // inventory
            if (team != TeamType.Player)
            {
                List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 3);
                inventory = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);
            }
            else
                inventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots).ToList();

            // TEMPORARY -- hardcode basesail to baseship (later on we want base ship to start without a sail)
            shipSail = new BaseSail(team, region, location, content, graphics);
            shipSail.millisecondsPerFrame = 500; // match turn speed for sail
            SetSpriteAsset(baseShipAsset, location);
        }
    }
}

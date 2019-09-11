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
using Gusto.Models.Interfaces;
using Gusto.AnimatedSprite.InventoryItems;

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
            millisecondsNewShot = 2000;
            movementSpeed = 0.2f;
            timeSinceStartSinking = 0;
            millisecondToSink = 10000;
            nSails = 1;
            fullHealth = 40;
            health = fullHealth;
            stopRange = 260f;
            maxInventorySlots = 5;

            string objKey = "baseShip";

            //MapModelMovementVectorValues();
            Texture2D textureBaseShip = content.Load<Texture2D>("BaseShip");
            Texture2D textureBaseShipBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseShipBB = new Texture2D(graphics, textureBaseShip.Width, textureBaseShip.Height);
            Asset baseShipAsset = new Asset(textureBaseShip, textureBaseShipBB, 1, 8, 0.6f, objKey, region);

            // inventory
            List<Sprite> interiorObjs = null;
            if (team != TeamType.Player)
            {
                List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 5);
                interiorObjs = ItemUtility.CreateInteriorItems(itemDrops, team, region, location, content, graphics);
                mountedOnShip = new BaseCannon(teamType, regionKey, GetBoundingBox().Center.ToVector2(), content, graphics);
            }

            actionInventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots).ToList();

            // TEMPORARY -- hardcode basesail to baseship (later on we want base ship to start without a sail)
            shipSail = new BaseSail(team, region, location, content, graphics);
            shipSail.millisecondsPerFrame = 500; // match turn speed for sail

            shipInterior = new Interior("baseShip", this, content, graphics);

            // set the random drops as interior objects
            if (interiorObjs != null)
            {
                foreach (var obj in interiorObjs)
                {
                    shipInterior.interiorObjects.Add(obj);
                    
                    // need to do this for containers so they drop items within ship
                    if (obj is IContainer)
                    {
                        Container c = (Container)obj;
                        c.inInteriorId = shipInterior.interiorId;
                    }
                }
            }

            SetSpriteAsset(baseShipAsset, location);
        }
    }
}

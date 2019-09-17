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
    public class ShortShip : Ship
    {
        public ShortShip(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastTurn = 0;
            millisecondsPerTurn = 300; // turn speed
            timeSinceStartAnchor = 0;
            millisecondsToAnchor = 800;
            msToRepair = 4000;
            millisecondsNewShot = 4000;
            movementSpeed = 0.3f;
            timeSinceStartSinking = 0;
            millisecondToSink = 10000;
            nSails = 1;
            fullHealth = 30;
            health = fullHealth;
            stopRange = 200f;
            maxInventorySlots = 2;

            string objKey = "shortShip";

            //MapModelMovementVectorValues();
            Texture2D texture = content.Load<Texture2D>("ShortShipHullRedMark");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 1, 8, 1.1f, objKey, region);

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
            shipSail = new ShortSail(team, region, location, content, graphics);
            shipSail.millisecondsPerFrame = millisecondsPerTurn; // match turn speed for sail

            shipInterior = new Interior("shortShip", this, content, graphics);

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

            SetSpriteAsset(asset, location);
        }
    }
}

using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gusto.Models.Animated
{
    public class Furnace : Sprite, ICanUpdate, IPlaceable, ICraftingObject, ILight
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public string craftSet;

        public bool drawCraftingMenu;
        bool smelting;

        float msPerFrame;
        float msThisFrame;
        float msCrafting;


        public Light emittingLight;
        int nTimesHit;
        private int hitsToPickUp;
        private bool canPickUp;
        float msPickupTimer;
        float msSinceStartPickupTimer;


        Queue<InventoryItem> craftingQueue;

        PiratePlayer playerNearItem;
        public TeamType teamType;

        public Furnace(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            msPickupTimer = 5000;
            msPerFrame = 200;
            hitsToPickUp = 10;

            craftingQueue = new Queue<InventoryItem>();
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                playerNearItem = (PiratePlayer)collidedWith;
            }

            if (collidedWith.bbKey.Equals("pickaxe"))
            {
                if (collidedWith.GetBoundingBox().Top > (GetBoundingBox().Center.ToVector2().Y + GetBoundingBox().Height / 3)) // MOVE UP
                    location.Y -= 10;
                else if (collidedWith.GetBoundingBox().Left > (GetBoundingBox().Center.ToVector2().X + GetBoundingBox().Width / 3)) // move left
                    location.X -= 10;
                else if (collidedWith.GetBoundingBox().Right < (GetBoundingBox().Center.ToVector2().X - GetBoundingBox().Width / 3))
                    location.X += 10;
                else if (collidedWith.GetBoundingBox().Bottom < (GetBoundingBox().Center.ToVector2().Y - GetBoundingBox().Height / 3))
                    location.Y += 10;

                nTimesHit += 1;
                if (nTimesHit >= hitsToPickUp)
                    canPickUp = true;
            }
        }        
        

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            if (playerNearItem != null && kstate.IsKeyDown(Keys.C)) // TODO: and keypress time
            {
                int nWood = 0;
                int nGrass = 0;

                // start fire and remove kindling items
                if (!smelting)
                {
                    // check for kindling
                    foreach (var item in playerNearItem.inventory)
                    {
                        if (item is IWood)
                            nWood += item.amountStacked;
                        if (item is IGrass)
                            nGrass += item.amountStacked;
                    }

                    if (nWood >= 2 && nGrass >= 2)
                    {
                        smelting = true;
                        RemoveKindlingConsumables();
                    }
                }  
            }

            if (craftingQueue.Count > 0)
                msCrafting += gameTime.ElapsedGameTime.Milliseconds;

            // create and drop item when crafting
            if (craftingQueue.Count > 0 && msCrafting > craftingQueue.Peek().msCraftTime)
            {

                InventoryItem item = craftingQueue.Dequeue();
                Vector2 dropLoc = new Vector2(GetBoundingBox().Center.ToVector2().X, GetBoundingBox().Center.ToVector2().Y + 40);

                item.location = dropLoc;
                item.onGround = true;
                item.amountStacked = 1;

                if (inInteriorId != Guid.Empty) // add drops to interior
                    BoundingBoxLocations.interiorMap[inInteriorId].interiorObjectsToAdd.Add(item);
                else // add drops to world
                    ItemUtility.ItemsToUpdate.Add(item);

                msCrafting = 0;
                // reset smelting
                if (craftingQueue.Count <= 0)
                {
                    smelting = false;
                    currColumnFrame = 0;
                    emittingLight.lit = false;
                }
            }

            if (playerNearItem != null && kstate.IsKeyDown(Keys.C) && !drawCraftingMenu && smelting)
            {
                drawCraftingMenu = true;
            }
            if (drawCraftingMenu)
            {
                if (kstate.IsKeyDown(Keys.Escape) || playerNearItem == null)
                {
                    drawCraftingMenu = false;
                }
            }

            if (smelting)
            {
                emittingLight.lit = true;
                msThisFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msThisFrame > msPerFrame)
                {
                    currColumnFrame++;
                    msThisFrame = 0;
                    if (currColumnFrame == nColumns)
                        currColumnFrame = 1;
                }
            }

            // lighting the furnace when running
            if (emittingLight.lit)
                emittingLight.Update(kstate, gameTime, GetBoundingBox().Center.ToVector2());

            if (canPickUp)
            {

                // pick up the item
                if (playerNearItem != null && kstate.IsKeyDown(Keys.P))
                {
                    InventoryItem cfi = new ClayFurnaceItem(playerNearItem.teamType, regionKey, location, _content, _graphics);
                    if (playerNearItem.AddInventoryItem(cfi))
                    {
                        cfi.placeableVersion = this;
                        cfi.inInventory = true;
                        cfi.onGround = false;
                        cfi.stackable = false;
                        cfi.amountStacked = 1;
                        remove = true;
                    }
                }


                // there is a timer for how long you have to pick up item once you have the required number of hits
                msSinceStartPickupTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (msSinceStartPickupTimer > msPickupTimer)
                {
                    msSinceStartPickupTimer = 0;
                    canPickUp = false;
                    nTimesHit = 0;
                }
            }

            playerNearItem = null;
        }

        public void DrawCanCraft(SpriteBatch sb, Camera camera)
        {
            if (playerNearItem != null)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(camera);
                sb.DrawString(font, "c", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
                sb.End();
            }
        }

        public void DrawCanPickUp(SpriteBatch sb, Camera camera)
        {
            if (playerNearItem != null && canPickUp)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(camera);
                sb.DrawString(font, "p", new Vector2(GetBoundingBox().X + 20, GetBoundingBox().Y - 50), Color.Black);
                sb.End();
            }
        }


        private void RemoveKindlingConsumables()
        {
            int ngrassRemoved = 0;
            int nwoodRemoved = 0;
            int index = 0;
            while (ngrassRemoved < 2)
            {
                if (index >= playerNearItem.inventory.Count)
                    index = 0;

                if (playerNearItem.inventory[index] == null)
                {
                    index++;
                    continue;
                }
                InventoryItem item = playerNearItem.inventory[index];
                if (item is IGrass)
                {
                    item.amountStacked -= 1;
                    ngrassRemoved += 1;
                }
                index++;
            }

            while (nwoodRemoved < 2)
            {
                if (index >= playerNearItem.inventory.Count)
                    index = 0;

                if (playerNearItem.inventory[index] == null)
                {
                    index++;
                    continue;
                }
                InventoryItem item = playerNearItem.inventory[index];
                if (item is IWood)
                {
                    item.amountStacked -= 1;
                    nwoodRemoved += 1;
                }
                index++;

            }
        }

        public Light GetEmittingLight()
        {
            return emittingLight;
        }

        public string GetCraftSet()
        {
            return craftSet;
        }

        public bool GetShowMenu()
        {
            return drawCraftingMenu;
        }

        public Queue<InventoryItem> GetCraftingQueue()
        {
            return craftingQueue;
        }
    }
}

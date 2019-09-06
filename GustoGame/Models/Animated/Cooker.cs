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
    public class Cooker : Sprite, ICanUpdate, IPlaceable, ICraftingObject, ILight
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        bool nearObj;
        public bool drawCraftingMenu;

        public string craftSet;

        string recipe;
        public bool canCraft;
        public bool cooking;
        float msPerFrame;
        float msThisFrame;
        float msToCook;
        float msCooking;

        public Light emittingLight;
        int nTimesHit;
        private int hitsToPickUp;
        private bool canPickUp;
        float msPickupTimer;
        float msSinceStartPickupTimer;

        PiratePlayer playerNearItem;
        public TeamType teamType;

        public Cooker(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            msPickupTimer = 5000;
            msToCook = 10000;
            msPerFrame = 200;
            hitsToPickUp = 10;

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("playerPirate") && !canCraft)
            {
                playerNearItem = (PiratePlayer)collidedWith;
                nearObj = true;

                // check inventory to see if we have required materials
                int nWood = 0;
                int nGrass = 0;
                foreach (var item in playerNearItem.inventory)
                {
                    if (item is IWood)
                        nWood = item.amountStacked;
                    if (item is IGrass)
                        nGrass = item.amountStacked;
                    
                    // TODO: CHECK FOR INGREDIENTS TODO

                }

                canCraft = true; // TEMP!
                if (nWood > 1 && nGrass > 1) // 2 wood, 2 grass, (and a recipie)
                    canCraft = true;
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
            // start the fire
            if (canCraft && kstate.IsKeyDown(Keys.C)) // TODO: and keypress time
            {

                bool hasGrass = false;
                bool hasWood = false;

                // Remove items from inv TODO: for now this just takes the first ore, grass, wood etc in inventory
                if (!cooking)
                {
                    foreach (var item in playerNearItem.inventory)
                    {
                        if (item is IWood && !hasWood)
                        {
                            item.amountStacked -= 2;
                        }
                        if (item is IGrass && !hasGrass)
                        {
                            item.amountStacked -= 2;
                        }
                        
                        // TODO: checkForRecipe() - checks inventory for recipie

                    }
                    cooking = true;
                }
            }

            if (cooking)
            {
                emittingLight.lit = true;
                // cooking so animate and being timer
                msCooking += gameTime.ElapsedGameTime.Milliseconds;
                msThisFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msThisFrame > msPerFrame)
                {
                    currColumnFrame++;
                    msThisFrame = 0;
                    if (currColumnFrame == nColumns)
                        currColumnFrame = 1;
                }
            }


            if (nearObj && kstate.IsKeyDown(Keys.C) && !drawCraftingMenu && cooking)
            {
                // TODO: bring up crafting menu
                drawCraftingMenu = true;
            }

            if (drawCraftingMenu)
            {
                if (kstate.IsKeyDown(Keys.Escape) || !nearObj)
                {
                    drawCraftingMenu = false;
                }
            }

            // create and drop item when done cooking
            if (msCooking > msToCook)
            {
                if (recipe != null)
                {
                    InventoryItem bar = null;
                    Vector2 dropLoc = new Vector2(GetBoundingBox().Center.ToVector2().X, GetBoundingBox().Center.ToVector2().Y + 40);
                    switch (recipe)
                    {
                        //TODO
                    }
                    bar.onGround = true;
                    bar.amountStacked = 1;
                    ItemUtility.ItemsToUpdate.Add(bar);
                }

                // reset
                cooking = false;
                msCooking = 0;
                recipe = null;
                currColumnFrame = 0;
                emittingLight.lit = false;
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


            canCraft = false;
            nearObj = false;
            playerNearItem = null;
        }

        public void DrawCanCraft(SpriteBatch sb, Camera camera)
        {
            if (canCraft)
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


        private string CheckOreType(Type t)
        {
            string ret = null;
            if (t == typeof(Gusto.AnimatedSprite.InventoryItems.IronOre))
                ret = "iron";

            return ret;

        }

        public Light GetEmittingLight()
        {
            return emittingLight;
        }
    }
}

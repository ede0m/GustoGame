using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
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
    public class Crafter : Sprite, ICanUpdate, IPlaceable, ICraftingObject
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public bool drawCraftingMenu;
        public string craftSet;

        int nTimesHit;
        int hitsToPickUp;
        bool canPickUp;

        float msPickupTimer;
        float msSinceStartPickupTimer;
        float msCrafting;

        Queue<InventoryItem> craftingQueue;

        PiratePlayer playerNearItem;
        public TeamType teamType;

        public Crafter(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            msPickupTimer = 5000;
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
                ItemUtility.ItemsToUpdate.Add(item);
                msCrafting = 0;
            }

            if (playerNearItem != null && kstate.IsKeyDown(Keys.C) && !drawCraftingMenu)
            {
                // TODO: bring up crafting menu
                drawCraftingMenu = true;
            }

            if (drawCraftingMenu)
            {
                if (kstate.IsKeyDown(Keys.Escape) || playerNearItem == null)
                {
                    drawCraftingMenu = false;
                }
            }

            if (canPickUp)
            {

                // pick up the item
                if (playerNearItem != null && kstate.IsKeyDown(Keys.P))
                {
                    InventoryItem ai = new AnvilItem(playerNearItem.teamType, regionKey, location, _content, _graphics);
                    if (playerNearItem.AddInventoryItem(ai))
                    {
                        ai.placeableVersion = this;
                        ai.inInventory = true;
                        ai.onGround = false;
                        ai.stackable = false;
                        ai.amountStacked = 1;
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

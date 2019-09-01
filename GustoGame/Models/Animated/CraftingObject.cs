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
    public class CraftingObject : Sprite, ICanUpdate, IPlaceable, ICraftingObject
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        float msPerFrame;
        float msThisFrame;

        bool nearAnvil;
        public bool drawCraftingMenu;

        int nTimesHit;
        private int hitsToPickUp;
        private bool canPickUp;
        float msPickupTimer;
        float msSinceStartPickupTimer;

        PiratePlayer playerNearItem;
        public TeamType teamType;

        public CraftingObject(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            msPickupTimer = 5000;
            hitsToPickUp = 10;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {

            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                nearAnvil = true;
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
            if (nearAnvil && kstate.IsKeyDown(Keys.C) && !drawCraftingMenu)
            {
                // TODO: bring up crafting menu
                drawCraftingMenu = true;
            }

            if (drawCraftingMenu)
            {
                if (kstate.IsKeyDown(Keys.Escape) || !nearAnvil)
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


            nearAnvil = false;
            playerNearItem = null;
        }

        public void DrawCanCraft(SpriteBatch sb, Camera camera)
        {
            if (nearAnvil)
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

    }
}

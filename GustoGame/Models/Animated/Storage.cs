using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Storage : Sprite, ICanUpdate, IPlaceable, IStorage
    {
        public bool inWater;
        public int nInventorySlots;
        public bool canOpenStorage;
        public bool storageOpen;
        public string storageTierType;

        int nTimesHit;
        int hitsToPickUp;
        bool canPickUp;
        float msPickupTimer;
        float msSinceStartPickupTimer;
        float msPerFrame;
        float msThisFrame;

        ContentManager _content;
        GraphicsDevice _graphics;

        PiratePlayer playerNearItem;
        public List<InventoryItem> inventory;

        public Storage(TeamType type, string region, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            msPerFrame = 250;
            hitsToPickUp = 10;
            msPickupTimer = 5000;

            _content = content;
            _graphics = graphics;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("landTile") || collidedWith.bbKey.Equals("interiorTile"))
            {
                inWater = false;
            }

            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                playerNearItem = (PiratePlayer)collidedWith;
                canOpenStorage = true;
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

            if (canOpenStorage && !storageOpen && kstate.IsKeyDown(Keys.O))
            {
                storageOpen = true;
            }

            if (storageOpen)
            {
                if (kstate.IsKeyDown(Keys.Escape) || !canOpenStorage)
                {
                    storageOpen = false;
                }
            }


            if (canPickUp)
            {
                // pick up the item
                if (playerNearItem != null && kstate.IsKeyDown(Keys.P))
                {
                    // TODO: might need to switch different chest types here?
                    InventoryItem bci = new BaseChestItem(playerNearItem.teamType, regionKey, location, _content, _graphics);
                    if (playerNearItem.AddInventoryItem(bci))
                    {
                        bci.placeableVersion = this;
                        bci.inInventory = true;
                        bci.onGround = false;
                        bci.stackable = false;
                        bci.amountStacked = 1;
                        remove = true;
                        canPickUp = false;
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

            if (inWater)
            {
                currRowFrame = 1;
                msThisFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msThisFrame > msPerFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame >= nColumns)
                        currColumnFrame = 0;
                    msThisFrame = 0;
                }
            }
            else
                currRowFrame = 0;


            inWater = true;
            canOpenStorage = false;
            playerNearItem = null;

        }

        public bool AddInventoryItem(InventoryItem itemToAdd)
        {
            for (int i = 0; i < inventory.Count(); i++)
            {
                // auto stack - TODO MAX STACK
                if (inventory[i] != null && inventory[i].bbKey == itemToAdd.bbKey && itemToAdd.stackable)
                {
                    inventory[i].amountStacked += itemToAdd.amountStacked;
                    return true;
                }

                if (inventory[i] == null)
                {
                    inventory[i] = itemToAdd;
                    return true;
                }
            }
            return false;
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

        public void DrawOpenStorage(SpriteBatch sb, Camera camera)
        {
            if (playerNearItem != null && canOpenStorage)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(camera);
                sb.DrawString(font, "o", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
                sb.End();
            }
        }
    }
}

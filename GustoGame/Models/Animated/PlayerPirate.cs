using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using GustoGame.Mappings;
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
    public class PlayerPirate : Sprite, IWalks, IVulnerable, ICanUpdate, IShadowCaster, IPlayer
    {
        public float timeSinceLastTurnFrame;
        public float timeSinceLastWalkFrame;
        public float timeSinceSwordSwing;
        public float timeSinceExitShipStart;
        public float timeSincePressedBury;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        public float millisecondsCombatSwing;
        float msToggleInterior;

        public float health;
        public float fullHealth;
        private bool showHealthBar;
        private int timeShowingHealthBar;

        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool canBury;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public bool showInventory;
        public List<InventoryItem> inventory;
        public int maxInventorySlots;
        public Ship playerOnShip;
        public Structure playerNearStructure;
        public Interior playerInInterior;
        public HandHeld inHand;
        public TeamType teamType;

        TilePiece buryTile;

        ContentManager _content;
        GraphicsDevice _graphics;

        public PlayerPirate(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {

            teamType = type;
            _content = content;
            _graphics = graphics;

            timeShowingHealthBar = 0;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            Rectangle footSpace = new Rectangle(GetBoundingBox().Left, GetBoundingBox().Bottom - (GetBoundingBox().Height / 3), GetBoundingBox().Width, GetBoundingBox().Height / 3);

            if (collidedWith.bbKey.Equals("landTile"))
            {
                colliding = false;
                // narrow the collision to just the feet (appears more realistic)
                if (footSpace.Intersects(collidedWith.GetBoundingBox()))
                    swimming = false;

                // can bury item?
                TilePiece tp = (TilePiece)collidedWith;
                if (tp.canFillHole)
                {
                    foreach (var item in inventory)
                    {
                        if (item == null)
                            continue;
                        if (item.placeableVersion != null && item.placeableVersion is IStorage)
                        {
                            buryTile = tp;
                            canBury = true;
                            break;
                        }
                    }
                }
            }

            else if (collidedWith.bbKey.Equals("interiorTile"))
            {
                colliding = false;
                swimming = false;
            }

            else if (collidedWith.bbKey.Equals("interiorTileWall"))
            {
                if (footSpace.Intersects(collidedWith.GetBoundingBox()))
                    colliding = true;
            }


            else if (collidedWith is IShip)
            {
                colliding = false;
                if (!onShip)
                {
                    nearShip = true;
                    playerOnShip = (Ship)collidedWith;
                    if (playerOnShip.sinking)
                        playerOnShip = null;
                }
            }
            else if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Animated.Npc))
            {
                Npc enemy = (Npc)collidedWith;
                colliding = false;
                if (enemy.inCombat)
                {
                    showHealthBar = true;
                    health -= enemy.damage;
                }
            }
            else if (collidedWith is IWalks || collidedWith is IGroundObject || collidedWith is IPlaceable)
            {
                colliding = false;
            }
            else if (collidedWith is IStructure)
            {
                colliding = false;
                playerNearStructure = (Structure)collidedWith;
            }
            else if (collidedWith is IAmmo)
            {
                Ammo ball = (Ammo)collidedWith;
                showHealthBar = true;
                if (!ball.exploded)
                    health -= ball.groundDamage;
                return;
            }

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurnFrame += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastWalkFrame += gameTime.ElapsedGameTime.Milliseconds;

            // check inventory for treasure maps
            BoundingBoxLocations.treasureLocationsList.Clear();
            List<int> removeSolved = new List<int>();
            foreach(var item in inventory)
            {
                if (item == null)
                    continue;
                if (item.bbKey.Equals("treasureMapItem"))
                {
                    TreasureMap map = (TreasureMap)item;
                    if (map.solved)
                        removeSolved.Add(inventory.IndexOf(item));
                    else
                        BoundingBoxLocations.treasureLocationsList.Add(map);
                }
            }
            foreach (var removeSolvedMap in removeSolved)
                inventory[removeSolvedMap] = null;
            

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > GameOptions.millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

            if (colliding)
                moving = false;

            colliding = false;
            swimming = true;

            if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
            {
                // toggle inventory (use turn frame speed here for convenience)
                if (kstate.IsKeyDown(Keys.Tab))
                {
                    if (showInventory)
                        showInventory = false;
                    else
                        showInventory = true;
                }

                if (!onShip || playerInInterior != null)
                {
                    moving = true;
                    // player direction
                    if (kstate.IsKeyDown(Keys.W))
                    {
                        currRowFrame = 3;
                        directionalFrame = 0;
                        if (kstate.IsKeyDown(Keys.A))
                            directionalFrame = 1;
                        else if (kstate.IsKeyDown(Keys.D))
                            directionalFrame = 7;
                    }
                    else if (kstate.IsKeyDown(Keys.S))
                    {
                        currRowFrame = 0;
                        directionalFrame = 4;
                        if (kstate.IsKeyDown(Keys.A))
                            directionalFrame = 3;
                        else if (kstate.IsKeyDown(Keys.D))
                            directionalFrame = 5;
                    }
                    else if (kstate.IsKeyDown(Keys.A))
                    {
                        currRowFrame = 2;
                        directionalFrame = 2;
                    }
                    else if (kstate.IsKeyDown(Keys.D))
                    {
                        currRowFrame = 1;
                        directionalFrame = 6;
                    }
                    else
                    {
                        moving = false;
                    }
                    inHand.currRowFrame = currRowFrame;
                } else
                {
                    moving = false;
                }

                timeSinceLastTurnFrame -= millisecondsPerTurnFrame;
            }

            // combat 
            if (!onShip || playerInInterior != null)
            {
                inHand.Update(kstate, gameTime, camera);
                if (playerInInterior != null)
                    inHand.inInteriorId = playerInInterior.interiorId;
                else
                    inHand.inInteriorId = Guid.Empty;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && (!onShip || playerInInterior != null) && !showInventory)
            {
                inCombat = true;
                inHand.inCombat = true;
                currColumnFrame = 8;
                if (inHand is IRanged)
                {
                    currColumnFrame = 9; // better frame for "holding" a ranged weapon
                    
                    //load ammo
                    if (inHand.ammoLoaded == null)
                    {
                        foreach (var item in inventory)
                        {
                            if (item != null && item.GetType() == inHand.ammoType)
                            {
                                if (item.amountStacked > 0)
                                    inHand.LoadAmmo(item);
                                else
                                    inventory[inventory.IndexOf(item)] = null;
                                break;
                            }
                        }
                    }
                }
            }
            else if (inCombat)
            {
                if (timeSinceSwordSwing > millisecondsCombatSwing) 
                {
                    currColumnFrame++;
                    inHand.location = location;
                    inHand.nextFrame = true;
                    if (currColumnFrame == nColumns)
                    {
                        inCombat = false;
                        inHand.inCombat = false;
                        currColumnFrame = 0;
                    }
                    timeSinceSwordSwing = 0;
                }
                timeSinceSwordSwing += gameTime.ElapsedGameTime.Milliseconds;
            }

            inHand.location = location;
            inHand.SetBoundingBox();

            // hop on ship
            if (nearShip && kstate.IsKeyDown(Keys.X) && !onShip && playerOnShip != null && timeSinceExitShipStart < 2000)
            {
                location = playerOnShip.GetBoundingBox().Center.ToVector2();
                onShip = true;
                playerOnShip.playerAboard = true;
                playerOnShip.shipSail.playerAboard = true;

                // can't control non player ships (until taken over)
                if (playerOnShip.teamType != TeamType.Player)
                {
                    playerInInterior = playerOnShip.shipInterior;
                }
            }
            // exit ship
            else if (kstate.IsKeyDown(Keys.X) && onShip)
            {
                timeSinceExitShipStart += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceExitShipStart > 2000)
                {
                    onShip = false;
                    if (playerInInterior != null)
                    {
                        playerInInterior.showingInterior = false;
                        playerInInterior.interiorObjects.Remove(this);
                        playerInInterior = null;
                        inInteriorId = Guid.Empty;
                    }
                    playerOnShip.playerAboard = false;
                    playerOnShip.shipSail.playerAboard = false;
                    location.X = playerOnShip.GetBoundingBox().Center.ToVector2().X - playerOnShip.GetBoundingBox().Width/2 - 20;
                    location.Y = playerOnShip.GetBoundingBox().Center.ToVector2().Y;
                    playerOnShip = null;
                }
            }
            else
            {
                timeSinceExitShipStart = 0;
            }
            nearShip = false;

            // player moves with ship when controlling
            if (onShip && playerInInterior == null)
            {
                location.X = playerOnShip.GetBoundingBox().Center.ToVector2().X;
                location.Y = playerOnShip.GetBoundingBox().Center.ToVector2().Y;
            }

            if (playerOnShip != null && playerOnShip.sinking)
            {
                onShip = false;
                playerInInterior = null;
                playerOnShip = null;
                inInteriorId = Guid.Empty;
            }

            else if (moving && !inCombat)
            {
                // walking animation
                if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame == 7) // stop before combat frames
                        currColumnFrame = 0;
                    timeSinceLastWalkFrame = 0;
                }

                // actual "regular" movement
                location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1);
                location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2);
            }
            else
            {
                if (!inCombat)
                {
                    currColumnFrame = 0;
                }
            }

            // burying storage
            int? removeChestIndex = null; 
            if (canBury && kstate.IsKeyDown(Keys.B))
            {
                timeSincePressedBury += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSincePressedBury > 1000)
                {
                    // remove first chest
                    foreach (var item in inventory)
                    {
                        if (item == null)
                            continue;
                        if (item.placeableVersion != null && item.placeableVersion is IStorage)
                        {
                            removeChestIndex = inventory.IndexOf(item);
                            break;
                        }
                    }

                    // create map
                    if (removeChestIndex != null)
                    {
                        Storage toBury = (Storage)inventory[(int)removeChestIndex].placeableVersion;
                        TreasureMapItem mapToAdd = new TreasureMapItem(toBury, teamType, regionKey, location, _content, _graphics);
                        mapToAdd.digTileLoc = buryTile.location;
                        mapToAdd.treasureInRegion = buryTile.regionKey;
                        mapToAdd.inInventory = false;
                        mapToAdd.remove = false;
                        mapToAdd.onGround = true;
                        buryTile.currColumnFrame = 0;
                        ItemUtility.ItemsToUpdate.Add(mapToAdd);
                        inventory[(int)removeChestIndex] = null;
                    }
                    timeSincePressedBury = 0;
                }
            }
            buryTile = null;
            canBury = false;

            // entering/toggling interior
            if (kstate.IsKeyDown(Keys.I)) // TODO and near entrance!
            {
                msToggleInterior += gameTime.ElapsedGameTime.Milliseconds;
                if (msToggleInterior > 1000)
                {
                    if (playerInInterior != null)
                    {
                        playerInInterior.showingInterior = false;
                        playerInInterior.interiorObjects.Remove(this);
                        inInteriorId = Guid.Empty;
                        playerInInterior = null;

                        if (playerOnShip != null)
                        {
                            playerOnShip.playerInInterior = false;
                            playerOnShip.shipSail.playerInInterior = false;
                        }
                    }
                    else
                    {
                        // enter an interior
                        if (onShip)  // the interior of the ship the player is in
                        {
                            playerInInterior = playerOnShip.shipInterior;
                            playerOnShip.playerInInterior = true;
                            playerOnShip.shipSail.playerInInterior = true;
                        }
                        else if (playerNearStructure != null)
                        {
                            playerInInterior = playerNearStructure.structureInterior;
                        }
                        inInteriorId = playerInInterior.interiorId;
                    }
                    msToggleInterior = 0;
                }
            }
            playerNearStructure = null;
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

        public void DrawSwimming(SpriteBatch sb, Camera camera)
        {
            Rectangle tRect = new Rectangle(targetRectangle.X, targetRectangle.Y, targetRectangle.Width, targetRectangle.Height);
            tRect.X = (_texture.Width / nColumns) * currColumnFrame;
            tRect.Y = (_texture.Height / nRows) * currRowFrame;

            // cut the bottom half of the targetRectangle off to hide the "under water" portion of the body
            tRect.Height = (_texture.Height / nRows) / 2;

            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            targetRectangle.Width = (_texture.Width / nColumns);
            targetRectangle.Height = tRect.Height;

            SetBoundingBox();
            sb.Begin(camera);
            sb.Draw(_texture, location, tRect, Color.White , 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }

        public void DrawEnterShip(SpriteBatch sb, Camera camera)
        {
            SpriteFont font = _content.Load<SpriteFont>("helperFont");
            sb.Begin(camera);
            sb.DrawString(font, "x", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
            sb.End();
        }

        public void DrawOnShip(SpriteBatch sb, Camera camera)
        {

            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            targetRectangle.Width = (_texture.Width / nColumns);
            targetRectangle.Height = (_texture.Height / nRows);

            SetBoundingBox();
            sb.Begin(camera);
            if (playerInInterior == null)
            {
                sb.Draw(_texture, location, targetRectangle, Color.White * 0.0f, 0f,
                    new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            }
            else
            {
                sb.Draw(_texture, location, targetRectangle, Color.White, 0f,
                    new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            }
            sb.End();
        }


        public void DrawHealthBar(SpriteBatch sb, Camera camera)
        {
            if (showHealthBar)
            {
                Texture2D meterAlive = new Texture2D(_graphics, 1, 1);
                Texture2D meterDead = new Texture2D(_graphics, 1, 1);
                meterAlive.SetData<Color>(new Color[] { Color.DarkKhaki });
                meterDead.SetData<Color>(new Color[] { Color.IndianRed });
                float healthLeft = (1f - (1f - (health / fullHealth))) * 60f;
                Rectangle dead = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 130, 60, 7);
                Rectangle alive = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 130, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
        }

        public void DrawCanBury(SpriteBatch sb, Camera camera)
        {
            SpriteFont font = _content.Load<SpriteFont>("helperFont");
            sb.Begin(camera);
            sb.DrawString(font, "b", new Vector2(GetBoundingBox().X - 20, GetBoundingBox().Y - 50), Color.Black);
            sb.End();
        }

        public bool GetSwimming()
        {
            return swimming;
        }
    }
}

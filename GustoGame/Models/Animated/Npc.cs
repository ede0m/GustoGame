using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
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
    public class Npc : Sprite, IWalks, IVulnerable, ICanUpdate, IShadowCaster, INPC
    {
        public float timeSinceLastTurnFrame;
        public float timeSinceLastWalkFrame;
        public float timeSinceCombat;
        public float timeSinceExitShipStart;
        public float timeSinceStartDying;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        public float millisecondsCombatMove;
        public float millisecondToDie;

        public float health;
        public float fullHealth;
        public float damage;
        public bool showHealthBar;
        public int timeShowingHealthBar;
        public bool dying;
        public float dyingTransparency;

        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public bool roaming;
        public bool defense;
        public List<InventoryItem> inventory;
        public Interior npcInInterior;
        public Sprite randomRegionRoamTile;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;

        public Npc(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;
            timeShowingHealthBar = 0;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Animated.HandHeld))
            {
                HandHeld handHeld = (HandHeld)collidedWith;
                showHealthBar = true;
                health -= handHeld.damage;
            }
            else if (collidedWith.bbKey.Equals("landTile") || collidedWith.bbKey.Equals("interiorTile") || collidedWith is IGroundObject)
            {
                colliding = false;

                // narrow the collision to just the feet (appears more realistic)
                Rectangle footSpace = new Rectangle(GetBoundingBox().Left, GetBoundingBox().Bottom - (GetBoundingBox().Height / 3), GetBoundingBox().Width, GetBoundingBox().Height / 3);
                if (footSpace.Intersects(collidedWith.GetBoundingBox()))
                    swimming = false;
            }

            else if (collidedWith.bbKey.Equals("interiorTileWall"))
            {
                colliding = false; 
            }

            else if (collidedWith is IWalks || collidedWith is IShip || collidedWith is IPlaceable || collidedWith is IInventoryItem || collidedWith is IStructure)
            {
                colliding = false;
            }
            else if (collidedWith is IAmmo)
            {
                showHealthBar = true;
                Ammo ball = (Ammo)collidedWith;   // TODO: bug NPC gets hit here by its own ships cannonballs
                if (!ball.exploded && ball.teamType != teamType)
                    health -= ball.groundDamage;
                return;
            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurnFrame += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastWalkFrame += gameTime.ElapsedGameTime.Milliseconds;

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > GameOptions.millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

            // dying
            if (health <= 0)
            {
                // drop items
                foreach (var item in inventory)
                {
                    item.inInventory = false;
                    // scatter items
                    item.location.X = location.X + RandomEvents.rand.Next(-10, 10);
                    item.location.Y = location.Y + RandomEvents.rand.Next(-10, 10);
                    item.onGround = true;

                    if (inInteriorId != Guid.Empty) // add drops to interior
                        BoundingBoxLocations.interiorMap[inInteriorId].interiorObjectsToAdd.Add(item);
                    else // add drops to world
                        ItemUtility.ItemsToUpdate.Add(item);
                }
                inventory.Clear();

                dying = true;
                currRowFrame = 2;

                timeSinceStartDying += gameTime.ElapsedGameTime.Milliseconds;
                dyingTransparency = 1 - (timeSinceStartDying / millisecondToDie);
                if (dyingTransparency <= 0)
                    remove = true;
            }

            if (colliding)
                moving = false;

            UpdateNpcMovement(teamType, gameTime);

            colliding = false;
            swimming = true;
        }


        private void UpdateNpcMovement(TeamType team, GameTime gameTime)
        {
            switch (team)
            {
                case TeamType.B:
                case TeamType.A:
                    // Movement
                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        Vector2? targetV = AIUtility.ChooseTarget(teamType, GetBoundingBox().Width * 2, GetBoundingBox(), inInteriorId);
                        if (targetV != null)
                        {
                            // IN COMBAT
                            if (!inCombat)
                                currColumnFrame = 7;
                            inCombat = true;
                            Tuple<int, int> frames = AIUtility.SetAIGroundMovement((Vector2)targetV, location);
                            currRowFrame = frames.Item1;
                            directionalFrame = frames.Item2;
                        }
                        else
                        {
                            inCombat = false;

                            // if we want to move to attack within a range
                            if (npcInInterior != null)
                            {
                                // attack any player within a large range in the ship
                                targetV = AIUtility.ChooseTarget(teamType, GetBoundingBox().Width * 10, GetBoundingBox(), inInteriorId);
                                if (targetV != null)
                                {
                                    Tuple<int, int> frames = AIUtility.SetAIGroundMovement((Vector2)targetV, location);
                                    currRowFrame = frames.Item1;
                                    directionalFrame = frames.Item2;
                                    defense = true;
                                }
                                else
                                    defense = false;
                            }

                            if (roaming && !defense) // region only rn
                            {
                                moving = true;
                                // go towards random tile
                                Tuple<int, int> frames = AIUtility.SetAIGroundMovement(randomRegionRoamTile.location, location);
                                currRowFrame = frames.Item1;
                                directionalFrame = frames.Item2;

                                // FIND a better way to get this value - can't have references
                                if (npcInInterior != null)
                                    randomRegionRoamTile = npcInInterior.interiorTiles.ToList()[npcInInterior.interiorTiles.ToList().IndexOf((TilePiece)randomRegionRoamTile)];

                                if (GetBoundingBox().Intersects(randomRegionRoamTile.GetBoundingBox()))
                                    roaming = false;
                            }
                            else if (!defense)
                            {
                                if (npcInInterior != null)
                                {
                                    randomRegionRoamTile = npcInInterior.RandomInteriorTile(); // interior tile roaming
                                }
                                else
                                    randomRegionRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles.Count)]; // region tile roaming
                                roaming = true;
                            }
                        }
                        timeSinceLastTurnFrame = 0;
                    }
                    if (moving && !inCombat && !dying)
                    {
                        // walking animation
                        if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
                        {
                            currColumnFrame++;
                            if (currColumnFrame >= 7) // stop before combat frames
                                currColumnFrame = 0;
                            timeSinceLastWalkFrame = 0;
                        }

                        // actual "regular" movement
                        location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1 * 0.5f);
                        location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2 * 0.5f);
                    }
                    else
                    {
                        if (inCombat)
                        {
                            if (timeSinceCombat > millisecondsCombatMove && !dying)
                            {
                                currColumnFrame++;
                                if (currColumnFrame >= nColumns)
                                {
                                    inCombat = false;
                                    currColumnFrame = 7;
                                }
                                timeSinceCombat = 0;
                            }
                            timeSinceCombat += gameTime.ElapsedGameTime.Milliseconds;
                        }
                    }
                    break;


                case TeamType.PassiveGround:

                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        if (roaming) // region only rn
                        {
                            // go towards random tile
                            Tuple<int, int> frames = AIUtility.SetAIGroundMovement(randomRegionRoamTile.location, location);
                            currRowFrame = frames.Item1;
                            directionalFrame = frames.Item2;

                            // passive ground can't travel through water
                            if (swimming)
                                randomRegionRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles.Count)];

                            // FIND a better way to get this value - can't have references
                            if (npcInInterior != null)
                                randomRegionRoamTile = npcInInterior.interiorTiles.ToList()[npcInInterior.interiorTiles.ToList().IndexOf((TilePiece)randomRegionRoamTile)];

                            if (GetBoundingBox().Intersects(randomRegionRoamTile.GetBoundingBox()))
                                roaming = false;
                        }
                        else
                        {
                            if (npcInInterior != null)
                            {
                                randomRegionRoamTile = npcInInterior.RandomInteriorTile(); // interior tile roaming
                            }
                            else
                                randomRegionRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles.Count)]; // region tile roaming
                            roaming = true;
                        }
                        timeSinceLastTurnFrame = 0;
                    }

                    // walking animation
                    if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
                    {
                        currColumnFrame++;
                        if (currColumnFrame <= 5) // stop before idle frames
                            moving = false;
                        else
                            moving = true;
                        if (currColumnFrame >= nColumns)
                            currColumnFrame = 0;
                        timeSinceLastWalkFrame = 0;
                    }

                    if (moving && !dying)
                    {
                        // actual "regular" movement
                        location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1 * 0.5f);
                        location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2 * 0.5f);
                    }
                    break;
            }
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
                Rectangle dead = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 80, 60, 7);
                Rectangle alive = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 80, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
        }

        public void DrawDying(SpriteBatch sb, Camera camera)
        {
            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            sb.Begin(camera);
            sb.Draw(_texture, location, targetRectangle, Color.White * dyingTransparency, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
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
            sb.Draw(_texture, location, tRect, Color.White, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }

        public bool GetSwimming()
        {
            return swimming;
        }
    }
}

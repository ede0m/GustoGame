using Comora;
using Gusto.Bounding;
using Gusto.Models.Interfaces;
using Gusto.Models.Types;
using Gusto.Utility;
using GustoGame.Mappings;
using GustoGame.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gusto.Models.Animated
{
    public class Npc : Sprite, IWalks, IVulnerable, ICanUpdate, IShadowCaster, INPC, IWakes
    {
        float timeSinceLastTurnFrame;
        float timeSinceLastWalkFrame;
        float timeSinceCombat;
        float timeSinceStartDying;
        float timeSinceIdleAnimate;
        float timeSinceIdleFrame;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        public float millisecondsCombatMove;
        public float millisecondToDie;
        public float msIdleWaitTime;

        public int combatFrameIndex; // what is the starting combat frameCol
        public int nIdleRowFrames; // how many rows of idle frames does this sprite sheet have?
        public int idleFreezeColFrame; // all column frames before this freeze movement 

        Texture2D meterAlive;
        Texture2D meterDead;
        public float health;
        public float fullHealth;
        public float damage;
        public bool showHealthBar;
        public int timeShowingHealthBar;
        public bool dying;
        public float dyingTransparency;

        public Sprite randomRegionRoamTile;
        List<TilePiece> currentPath;
        public WakeParticleEngine wake;
        public bool roaming;
        public bool swimming;
        public bool flying;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public bool defense;
        public bool idle;
        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values

        public Vector2 currentSpeed;
        public List<InventoryItem> inventory;
        public Interior npcInInterior;
        public ActionState actionState;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;

        public Npc(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;

            meterAlive = new Texture2D(_graphics, 1, 1);
            meterDead = new Texture2D(_graphics, 1, 1);
            meterAlive.SetData<Color>(new Color[] { Color.DarkKhaki });
            meterDead.SetData<Color>(new Color[] { Color.IndianRed });

            timeShowingHealthBar = 0;

            wake = new WakeParticleEngine(content, location);
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Animated.HandHeld))
            {
                HandHeld handHeld = (HandHeld)collidedWith;
                showHealthBar = true;
                health -= handHeld.damage;
            }
            else if (collidedWith.bbKey.Equals("landTile") || collidedWith.bbKey.Equals("interiorTile"))
            {
                TilePiece tp = (TilePiece)collidedWith;

                colliding = false;
                mapCordPoint = tp.mapCordPoint;

                // narrow the collision to just the feet (appears more realistic)
                Rectangle footSpace = new Rectangle(GetBoundingBox().Left, GetBoundingBox().Bottom - (GetBoundingBox().Height / 3), GetBoundingBox().Width, GetBoundingBox().Height / 3);
                if (footSpace.Intersects(collidedWith.GetBoundingBox()))
                    swimming = false;
            }

            else if (collidedWith is IGroundObject)
            {
                colliding = false;
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

            UpdateNpcActionMovement(gameTime);

            // update any water wake
            wake.EmitterLocation = location;
            wake.Update(currentSpeed, (swimming && moving));

            colliding = false;
            if (!(this is IFlying))
                swimming = true;
        }

        private void UpdateNpcActionMovement(GameTime gameTime)
        {
            switch (actionState)
            {
                case ActionState.DefenseRoam:
                    // Movement
                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        if (npcInInterior != null)
                            NpcSetMoveTowardsTarget(10, gameTime); // wider defense range in ship
                        else
                            NpcSetMoveTowardsTarget(5, gameTime);

                        NpcSetAttackInRange(gameTime);

                        if (roaming && !defense)
                            NpcSetRoamMovement(false, gameTime);
                        else if (!defense)
                            ResetRoam(false);

                        timeSinceLastTurnFrame = 0;
                    }
                    if (moving && !inCombat && !dying)
                        NpcMove(gameTime);
                    else if (inCombat)
                        NpcAttack(gameTime);
                    break;

                case ActionState.IdleDefense:
                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        NpcSetMoveTowardsTarget(5, gameTime);
                        NpcSetAttackInRange(gameTime);
                        timeSinceLastTurnFrame = 0;
                    }

                    if (idle)
                        NpcIdle(gameTime);
                    else if (moving && !inCombat && !dying)
                        NpcMove(gameTime);
                    else if (inCombat)
                        NpcAttack(gameTime);

                    break;

                case ActionState.PassiveRoam:
                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        if (roaming) // region only rn
                            NpcSetRoamMovement(true, gameTime);
                        else
                            ResetRoam(true);
                        timeSinceLastTurnFrame = 0;
                    }

                    NpcMove(gameTime);
                    break;

                case ActionState.IdleFlee:
                    if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
                    {
                        NpcSetFlee(false, gameTime);

                        if (roaming)
                            NpcSetRoamMovement(false, gameTime);
                        else
                            idle = true;

                        timeSinceLastTurnFrame = 0;
                    }

                    if (idle)
                        NpcIdle(gameTime);
                    else if (moving && !inCombat && !dying)
                        NpcMove(gameTime);
                    break;
            }
        }

        private void NpcMove(GameTime gt)
        {
            // walking animation
            if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
            {
                currColumnFrame++;

                // idle freeze frames
                if (idleFreezeColFrame > 0)
                {
                    if (currColumnFrame <= idleFreezeColFrame) // stop on in row idle frames
                        moving = false;
                    else
                        moving = true;
                }

                if (combatFrameIndex > 0)
                {
                    if (currColumnFrame >= combatFrameIndex) // stop before combat frames
                        currColumnFrame = 0;
                }
                else
                {
                    if (currColumnFrame >= nColumns) // reset cols
                        currColumnFrame = 0;
                }

                timeSinceLastWalkFrame = 0;
            }

            if (moving && !dying)
            {
                // actual "regular" movement
                currentSpeed = new Vector2(PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1 * 0.8f,
                    PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2 * 0.8f);
                location += currentSpeed;
            } 

        }

        private void NpcAttack(GameTime gt)
        {
            if (timeSinceCombat > millisecondsCombatMove && !dying)
            {
                currColumnFrame++;
                if (currColumnFrame >= nColumns)
                {
                    inCombat = false;
                    currColumnFrame = combatFrameIndex;
                }
                timeSinceCombat = 0;
            }
            timeSinceCombat += gt.ElapsedGameTime.Milliseconds;
        }

        private void NpcIdle(GameTime gt)
        {
            moving = false;
            currRowFrame = 0; // idle row frame -- TODO: what if i want a few idel row frames?
            timeSinceIdleAnimate += gt.ElapsedGameTime.Milliseconds;
            if (timeSinceIdleAnimate > msIdleWaitTime)
            {
                timeSinceIdleFrame += gt.ElapsedGameTime.Milliseconds;
                if (timeSinceIdleFrame > 100)
                {
                    currColumnFrame++;
                    timeSinceIdleFrame = 0;
                    if (currColumnFrame >= nColumns)
                    {
                        currColumnFrame = 0;
                        timeSinceIdleAnimate = 0;
                    }
                }
            }
        }

        private void NpcSetMoveTowardsTarget(int bbRange, GameTime gt)
        {
            // if target within range, move towards it
            Vector2? targetV = AIUtility.ChooseTargetVector(teamType, GetBoundingBox().Width * bbRange, GetBoundingBox(), inInteriorId);
            if (targetV != null)
            {
                idle = false;
                Tuple<int, int> frames = AIUtility.SetAIGroundMovement((Vector2)targetV, location);
                currRowFrame = frames.Item1 + nIdleRowFrames; // plus one to skip the idle frame
                directionalFrame = frames.Item2;
                moving = true;
                defense = true;
            }
            else
            {
                idle = true;
                defense = false;
            }
        }

        private void NpcSetAttackInRange(GameTime gt)
        {
            // attack range
            Vector2? targetV = AIUtility.ChooseTargetVector(teamType, GetBoundingBox().Width * 2, GetBoundingBox(), inInteriorId);
            if (targetV != null)
            {
                idle = false;
                // IN COMBAT
                if (!inCombat)
                    currColumnFrame = combatFrameIndex;
                inCombat = true;
                Tuple<int, int> frames = AIUtility.SetAIGroundMovement((Vector2)targetV, location);
                currRowFrame = frames.Item1 + nIdleRowFrames; // plus one to skip the idle frame
                directionalFrame = frames.Item2;
            }
        }

        private void NpcSetFlee(bool useAStar, GameTime gt)
        {
            // if target within range, move towards it
            Vector2? targetV = AIUtility.ChooseTargetVector(teamType, GetBoundingBox().Width * 3, GetBoundingBox(), inInteriorId);
            if (targetV != null && !roaming)
            {
                idle = false;
                if (this is IFlying)
                    flying = true;

                ResetRoam(useAStar);
            }
        }

        private void NpcSetRoamMovement(bool useAStar, GameTime gt)
        {
            moving = true;
            // go towards random tile
            Vector2 targetLoc;
            if (useAStar)
                targetLoc = currentPath[0].GetBoundingBox().Center.ToVector2();
            else
                targetLoc = randomRegionRoamTile.location;

            Tuple<int, int> frames = AIUtility.SetAIGroundMovement(targetLoc, location);
            currRowFrame = frames.Item1 + nIdleRowFrames;
            directionalFrame = frames.Item2;

            // FIND a better way to get this value - can't have references
            if (npcInInterior != null)
                randomRegionRoamTile = npcInInterior.interiorTiles.ToList()[npcInInterior.interiorTiles.ToList().IndexOf((TilePiece)randomRegionRoamTile)];

            if (useAStar)
            {
                // we have found the next tile in path
                if (currentPath != null && currentPath[0].GetBoundingBox().Intersects(GetBoundingBox()))
                {
                    currentPath.RemoveAt(0);
                    if (currentPath.Count == 0) // found the end of the path
                        roaming = false;
                }
            }
            else
            {
                // found roam tile
                if (GetBoundingBox().Intersects(randomRegionRoamTile.GetBoundingBox()))
                {
                    roaming = false;
                    flying = false; // for anything flying
                }
            }
        }

        private void ResetRoam(bool useAStar)
        {
            if (npcInInterior != null)
                randomRegionRoamTile = npcInInterior.RandomInteriorTile(); // interior tile roaming
            else
                randomRegionRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionLandTiles.Count)]; // region tile roaming

            if (useAStar)
            {
                TilePiece rtp = (TilePiece)randomRegionRoamTile;
                Point? gridPointTo = rtp.mapCordPoint;
                if (mapCordPoint != Point.Zero && mapCordPoint != null)
                {
                    roaming = true;
                    currentPath = AIUtility.Pathfind(mapCordPoint.Value, gridPointTo.Value, PathType.AllOutdoor); // NOTE: This freezes the game when hitting GustoMap region (because it is almost all the tiles at the moment)
                }
            }
            else
            {
                roaming = true;
            }
        }

        public void DrawHealthBar(SpriteBatch sb, Camera camera)
        {
            if (showHealthBar)
            {
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

        public WakeParticleEngine GetWakeEngine()
        {
            return wake;
        }
    }
}

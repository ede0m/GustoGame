
using Comora;
using global::Gusto.AnimatedSprite;
using Gusto.Bounding;
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
    public class HandHeld : InventoryItem, IWeapon, IHandHeld
    {
        public int timeSinceLastFrame;
        public float timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int millisecondsPerFrame; // turning speed
        public float millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public bool nextFrame;

        public float damage;
        public bool inCombat;
        public int shotRange;

        public bool usingItem;
        public InventoryItem ammoLoaded;
        public Type ammoType;
        public List<Ammo> Shots;
        public Light emittingLight; // if this handheld emits any light

        GraphicsDevice _graphics;
        ContentManager _content;

        public HandHeld(TeamType type, ContentManager content, GraphicsDevice graphics) : base(type, content, graphics)
        {
            Shots = new List<Ammo>();
            _graphics = graphics;
            _content = content;
            teamType = type;
            stackable = false;
            amountStacked = 1;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            // clean shots
            foreach (var shot in Shots)
                shot.Update(kstate, gameTime);
            if (timeSinceLastExpClean > millisecondsExplosionLasts)
            {
                // remove exploded shots
                for (int i = 0; i < Shots.Count; i++)
                {
                    if (Shots[i].exploded || Shots[i].outOfRange)
                        Shots.RemoveAt(i);
                }
                timeSinceLastExpClean = 0;
            }

            // lighting items
            if (emittingLight != null)
            {
                emittingLight.Update(kstate, gameTime, GetBoundingBox().Center.ToVector2());
            }

            usingItem = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                currColumnFrame = 0;
                // aiming
                if (this is IRanged)
                {
                    currColumnFrame = 1;
                    timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;

                    Tuple<int, int> shotDirection = null;
                    int shotOffsetX = 0;
                    int shotOffsetY = 0;
                    bool shootHorz = false;

                    if (currRowFrame == 1 || currRowFrame == 2)
                        shootHorz = true;

                    // diagnoal aim
                    Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    Vector2 clickPos = mousePos - new Vector2(GameOptions.PrefferedBackBufferWidth / 2, GameOptions.PrefferedBackBufferHeight / 2) + camera.Position;
                    int shootMiddleHoverRangeHalf = 40;
                    bool shootLeft = false; // these are not player directional, but used for setting the ammo shot dir with respect to the rotation of gun
                    bool shootRight = false;
                    bool shootUp = false;
                    bool shootDown = false;
                    if (shootHorz)
                    {
                        if (clickPos.Y < GetBoundingBox().Center.ToVector2().Y - shootMiddleHoverRangeHalf)
                        {
                            if (currRowFrame == 1) // right
                                rotation = -0.5f;
                            else
                                rotation = 0.5f;
                            shootUp = true;

                        }
                        else if (clickPos.Y > GetBoundingBox().Center.ToVector2().Y + shootMiddleHoverRangeHalf)
                        {
                            if (currRowFrame == 1)
                                rotation = 0.5f;
                            else
                                rotation = -0.5f;
                            shootDown = true;
                        }
                        else
                            rotation = 0;
                    }
                    else
                    {
                        if (clickPos.X < GetBoundingBox().Center.ToVector2().X - shootMiddleHoverRangeHalf)
                        {
                            if (currRowFrame == 0) // down
                                rotation = 0.5f;
                            else
                                rotation = -0.7f;
                            shootLeft = true;
                        }
                        else if (clickPos.X > GetBoundingBox().Center.ToVector2().X + shootMiddleHoverRangeHalf)
                        {
                            if (currRowFrame == 0)
                                rotation = -0.7f;
                            else
                                rotation = 0.7f;
                            shootRight = true;
                        }
                        else
                            rotation = 0;
                    }

                    // shooting
                    if (kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
                    {
                        if (ammoLoaded != null)
                        {
                            Vector2 shotStart = new Vector2(GetBoundingBox().Center.ToVector2().X + shotOffsetX, GetBoundingBox().Center.ToVector2().Y + shotOffsetY);
                            PistolShot pistolShot = new PistolShot(teamType, regionKey, shotStart, _content, _graphics);
                            //int offsetStraight = shootHorz ? (pistolShot.GetBoundingBox().Y - shotDirection.Item2) : (pistolShot.GetBoundingBox().X - shotDirection.Item1);

                            if (shootHorz)
                            {
                                // aiming straight
                                if (!shootUp && !shootDown)
                                {
                                    if (currRowFrame == 1) // right
                                        shotDirection = new Tuple<int, int>((int)pistolShot.GetBoundingBox().Center.ToVector2().X + shotRange, (int)pistolShot.GetBoundingBox().Center.ToVector2().Y);
                                    else
                                        shotDirection = new Tuple<int, int>((int)pistolShot.GetBoundingBox().Center.ToVector2().X - shotRange, (int)pistolShot.GetBoundingBox().Center.ToVector2().Y);
                                }
                                else
                                {
                                    // angled
                                    if (shootUp)
                                    {
                                        if (currRowFrame == 1)
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X + (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y - (shotRange * PhysicsUtility.sin45deg)));
                                        else
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X - (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y - (shotRange * PhysicsUtility.sin45deg)));
                                    }
                                    else
                                    {
                                        if (currRowFrame == 1)
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X + (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y + (shotRange * PhysicsUtility.sin45deg)));
                                        else
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X - (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y + (shotRange * PhysicsUtility.sin45deg)));
                                    }
                                }
                            }
                            else
                            {
                                // aiming straight
                                if (!shootLeft && !shootRight)
                                {
                                    if (currRowFrame == 0) // down
                                        shotDirection = new Tuple<int, int>((int)pistolShot.GetBoundingBox().Center.ToVector2().X, (int)pistolShot.GetBoundingBox().Center.ToVector2().Y + shotRange);
                                    else
                                        shotDirection = new Tuple<int, int>((int)pistolShot.GetBoundingBox().Center.ToVector2().X, (int)pistolShot.GetBoundingBox().Center.ToVector2().Y - shotRange);
                                }
                                else
                                {
                                    // angled
                                    if (shootLeft)
                                    {
                                        if (currRowFrame == 0)
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X - (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y + (shotRange * PhysicsUtility.sin45deg)));
                                        else
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X - (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y - (shotRange * PhysicsUtility.sin45deg)));
                                    }
                                    else
                                    {
                                        if (currRowFrame == 0)
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X + (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y + (shotRange * PhysicsUtility.sin45deg)));
                                        else
                                            shotDirection = new Tuple<int, int>((int)(pistolShot.GetBoundingBox().Center.ToVector2().X + (shotRange * PhysicsUtility.sin45deg)), (int)(pistolShot.GetBoundingBox().Center.ToVector2().Y - (shotRange * PhysicsUtility.sin45deg)));
                                    }
                                }
                            }

                            pistolShot.SetFireAtDirection(shotDirection, RandomEvents.rand.Next(10, 25), 0);
                            pistolShot.moving = true;
                            Shots.Add(pistolShot);
                            ammoLoaded.amountStacked -= 1;
                            if (ammoLoaded.amountStacked == 0)
                                ammoLoaded = null;
                            timeSinceLastShot = 0;
                        }
                    }

                }

                // lighting on any handheld elements
                if (emittingLight != null)
                    emittingLight.scaleSize = emittingLight.baseSize * 1.4f;
  
            }
            else if (inCombat)
            {
                if (emittingLight != null)
                    emittingLight.scaleSize = emittingLight.baseSize;

                usingItem = true;

                if (nextFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame == nColumns)
                        currColumnFrame = 0;
                }

                if (this is IRanged)
                    currColumnFrame = 1;
            }

            nextFrame = false;
            SpatialBounding.SetQuad(GetBase());
        }

        public void LoadAmmo(InventoryItem item)
        {
            ammoLoaded = item;
        }
    }
}


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

                    // shooting
                    if (kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
                    {
                        if (ammoLoaded != null)
                        {
                            Tuple<int, int> shotDirection = null;
                            int shotOffsetX = 0;
                            int shotOffsetY = 0;
                            bool shootHorz = false;
                            switch (currRowFrame)
                            {
                                case (0):
                                    //down
                                    shotOffsetY = 30;
                                    shotDirection = new Tuple<int, int>((int)GetBoundingBox().Center.ToVector2().X, (int)GetBoundingBox().Center.ToVector2().Y + shotRange);
                                    break;
                                case (1):
                                    //right
                                    shootHorz = true;
                                    shotOffsetX = 30;
                                    shotDirection = new Tuple<int, int>((int)GetBoundingBox().Center.ToVector2().X + shotRange, (int)GetBoundingBox().Center.ToVector2().Y);
                                    break;
                                case (2):
                                    //left
                                    shootHorz = true;
                                    shotOffsetX = -13;
                                    shotDirection = new Tuple<int, int>((int)GetBoundingBox().Center.ToVector2().X - shotRange, (int)GetBoundingBox().Center.ToVector2().Y);
                                    break;
                                case (3):
                                    //up
                                    shotOffsetY = -10;
                                    shotDirection = new Tuple<int, int>((int)GetBoundingBox().Center.ToVector2().X, (int)GetBoundingBox().Center.ToVector2().Y - shotRange);
                                    break;
                            }

                            Vector2 shotStart = new Vector2(GetBoundingBox().Center.ToVector2().X + shotOffsetX, GetBoundingBox().Center.ToVector2().Y + shotOffsetY);
                            PistolShot pistolShot = new PistolShot(teamType, regionKey, shotStart, _content, _graphics);
                            int offsetStraight = shootHorz ? (pistolShot.GetBoundingBox().Y - shotDirection.Item2) : (pistolShot.GetBoundingBox().X - shotDirection.Item1);
                            pistolShot.SetFireAtDirection(shotDirection, RandomEvents.rand.Next(10, 25), offsetStraight);
                            pistolShot.moving = true;
                            Shots.Add(pistolShot);
                            ammoLoaded.amountStacked -= 1;
                            if (ammoLoaded.amountStacked == 0)
                                ammoLoaded = null;
                            timeSinceLastShot = 0;
                        }
                    }
                }
            }
            else if (inCombat)
            {
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

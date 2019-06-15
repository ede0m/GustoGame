
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

        public bool aiming;
        public List<Ammo> Shots;

        Random rand;
        GraphicsDevice _graphics;
        ContentManager _content;

        public HandHeld(TeamType type, ContentManager content, GraphicsDevice graphics) : base(type, content, graphics)
        {
            Shots = new List<Ammo>();
            rand = new Random();
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

            aiming = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                currColumnFrame = 0;
                // aiming
                if (this is IRanged)
                {
                    currColumnFrame = 1;
                    aiming = true;
                    timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;

                    // shooting
                    if (kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
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
                        BaseCannonBall cannonShot = new BaseCannonBall(teamType, regionKey, shotStart, _content, _graphics);
                        int offsetStraight = shootHorz ? (cannonShot.GetBoundingBox().Y - shotDirection.Item2) : (cannonShot.GetBoundingBox().X - shotDirection.Item1);
                        cannonShot.SetFireAtDirection(shotDirection, RandomEvents.RandomShotSpeed(rand), offsetStraight);
                        cannonShot.moving = true;
                        Shots.Add(cannonShot);
                        timeSinceLastShot = 0;
                    }
                }
            }
            else if (inCombat)
            {
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

    }
}

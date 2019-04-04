﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Gusto.Models;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Gusto.Bounding;

namespace Gusto.AnimatedSprite
{
    public class Tower : Sprite
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        private Vector2 Location;
        private int timeSinceLastShot;
        private int timeSinceLastExpClean;
        private int millisecondsNewShot;
        private int millisecondsExplosionLasts;
        private int maxShotsMoving;
        Random randomGeneration;

        public List<CannonBall> Shots;

        public Tower(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            randomGeneration = new Random();
            currRowFrame = 0;
            currColumnFrame = 0;
            timeSinceLastShot = 0;
            timeSinceLastExpClean = 0;
            millisecondsNewShot = 2000;
            millisecondsExplosionLasts = 400;
            maxShotsMoving = 5;

            Shots = new List<CannonBall>();

            Texture2D textureTower = content.Load<Texture2D>("tower");
            Texture2D textureTowerBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTowerBB = new Texture2D(graphics, textureTower.Width, textureTower.Height);
            Asset towerAsset = new Asset(textureTower, textureTowerBB, 1, 1, 0.5f, "tower");

            SetSpriteAsset(towerAsset, location);
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

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
                timeSinceLastExpClean -= millisecondsExplosionLasts;
            }

            if (timeSinceLastShot > millisecondsNewShot && Shots.Count < maxShotsMoving)
            {
                BaseCannonBall cannonShot = new BaseCannonBall(location, _content, _graphics);
                Tuple<int, int> shotDirection = BoundingBoxLocations.BoundingBoxLocationMap["baseShip"]; // TODO REMOVE HARDCODE AND SCAN BY TOWER RANGE
                cannonShot.SetFireAtDirection(shotDirection, RandomShotSpeed());
                cannonShot.moving = true;
                Shots.Add(cannonShot);
                timeSinceLastShot -= millisecondsNewShot;
            }
        }

        private int RandomShot()
        {
            int randomDirection = randomGeneration.Next(-100, 200);
            return 0;
        }

        private int RandomShotSpeed()
        {
            return randomGeneration.Next(10, 25);
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // Only stop movement when colliding at the bottom of the tower
            int movePastTowerThresholdBehind =  this.GetBoundingBox().Bottom - 40;
            int movePastTowerThresholdInfront =  this.GetBoundingBox().Bottom + 40;
            if (collidedWith.bbKey.Equals("baseShip"))
            {
                if ((overlap.Bottom > movePastTowerThresholdBehind && collidedWith.GetBoundingBox().Bottom <= movePastTowerThresholdInfront))
                {
                    Trace.WriteLine("Collision at base of tower");
                    collidedWith.colliding = true;
                } else
                {
                    collidedWith.colliding = false;
                }
            }
        }
    }
}

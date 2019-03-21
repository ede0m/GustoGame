using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Gusto.Models;

namespace Gusto.AnimatedSprite
{
    public class Tower : Sprite
    {
        private Vector2 Location;
        private int timeSinceLastFrame;
        private int millisecondsPerFrame;
        Random randomGeneration;

        public Tower(Vector2 location, Asset asset) : base(location, asset)
        {
            randomGeneration = new Random();
            currRowFrame = 0;
            currColumnFrame = 0;
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 10000;
        }

        // logic to find correct frame of sprite from user input
        public void Update(KeyboardState kstate, GameTime gameTime) // keeping kstate in here for possible powerup to change wind directrion
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

        private void RandomShot()
        {
            int randomDirection = randomGeneration.Next(-100, 200);

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // Only stop movement when colliding at the bottom of the tower
            int movePastTowerThresholdBehind =  this.GetBoundingBox().Bottom - 40;
            int movePastTowerThresholdInfront =  this.GetBoundingBox().Bottom + 40;
            if (collidedWith.bbKey.Equals("ship") && (overlap.Bottom > movePastTowerThresholdBehind && collidedWith.GetBoundingBox().Bottom <= movePastTowerThresholdInfront))
            {
                Trace.WriteLine("Collision at base of tower");
                collidedWith.moving = false;
            } else
            {
                Trace.WriteLine("Collision but passing");
            }
        }
    }
}


using global::Gusto.AnimatedSprite;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Gusto.Models
{
    public class CannonBall : Sprite
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public float baseMovementSpeed;
        public bool exploded = false;
        public bool outOfRange = false;
        public float shotDirX;
        public float shotDirY;
        public float distanceTraveledX = 0;
        public float distanceTraveledY = 0;
        public float vectorMagnitude;
        int shotLenX;
        int shotLenY;
        public Vector2 firedFromLoc;

        public CannonBall(Vector2 firedFrom)
        {
            firedFromLoc = firedFrom;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("tower"))
            {

            } else if (collidedWith.bbKey.Equals("baseSail"))
            {
                Trace.WriteLine("COLLIDED WITH SAIL");
            }

            else
            {
                currColumnFrame++; // explosion
                exploded = true;
                BoundFrames();
            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            if (colliding)
                moving = false;
            else
                moving = true;

            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                if (moving && !exploded)
                {
                    location.X += shotDirX;
                    location.Y += shotDirY;
                    distanceTraveledX += Math.Abs(shotDirX);
                    distanceTraveledY += Math.Abs(shotDirY);
                }
                if (distanceTraveledX > (shotLenX * 1.3) || distanceTraveledY > (shotLenY * 1.3))
                    outOfRange = true;
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

        public void SetFireAtDirection(Tuple<int,int> fireAtDirection, int shotSpeed)
        {
            vectorMagnitude = VectorMagnitude(GetBoundingBox().X, fireAtDirection.Item1, GetBoundingBox().Y, fireAtDirection.Item2);
            shotLenX = Math.Max(fireAtDirection.Item1, GetBoundingBox().X) - Math.Min(fireAtDirection.Item1, GetBoundingBox().X);
            shotLenY = Math.Max(fireAtDirection.Item2, GetBoundingBox().Y) - Math.Min(fireAtDirection.Item2, GetBoundingBox().Y);
            shotDirX = (fireAtDirection.Item1 - GetBoundingBox().X) / vectorMagnitude  * shotSpeed;
            shotDirY = (fireAtDirection.Item2 - GetBoundingBox().Y) /vectorMagnitude * shotSpeed;
        }

        private float VectorMagnitude(float x2, float x1, float y2, float y1)
        {
            return (float) Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}

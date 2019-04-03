
using global::Gusto.AnimatedSprite;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gusto.Models
{
    public class CannonBall : Sprite
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public float baseMovementSpeed;
        public bool exploded;
        public int shotDirX;
        public int shotDirY;

        public CannonBall(){}

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("tower"))
            {

            } else
            {
                currColumnFrame++; // explosion
                moving = false;
                exploded = true;
                BoundFrames();
            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                if (moving)
                {
                    location.X += shotDirX;
                    location.Y += shotDirY;
                }
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

        public void SetFireAtDirection(Tuple<int,int> fireAtDirection)
        {
            shotDirX = (fireAtDirection.Item1 - GetBoundingBox().X) / 10;
            shotDirY = (fireAtDirection.Item2 - GetBoundingBox().Y) / 10;
        }

    }
}

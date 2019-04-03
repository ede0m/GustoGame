
using global::Gusto.AnimatedSprite;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models
{
    public class CannonBall : Sprite
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public float baseMovementSpeed;
        public bool exploded;

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
                    location.X += 100;
                    location.Y += 100;
                }
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

    }
}

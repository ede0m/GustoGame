using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Gusto.Models;

namespace Gusto.AnimatedSprite
{
    public class WindArrows : Sprite
    {
        private Vector2 Location;
        private int timeSinceLastFrame;
        private int millisecondsPerFrame;
        Random randomGeneration;

        public WindArrows(Vector2 location, Asset asset) : base(location, asset)
        {
            randomGeneration = new Random();
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 10000;
        }

        // returns the index of row in sprite sheet which corresponds to the wind direction
        public int getWindDirection()
        {
            return currRowFrame;
        }
        // returns the index of column in sprite sheet which corresponds to the wind speed
        public int getWindSpeed()
        {
            return currColumnFrame + 1; // only columns 0 and 1, so add one to return as speed that can be used in calculations
        }

        // logic to find correct frame of sprite from user input
        public void Update(KeyboardState kstate, GameTime gameTime) // keeping kstate in here for possible powerup to change wind directrion
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                RandomWind();
                BoundFrames();
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        private void RandomWind()
        {
            int randomDirection = randomGeneration.Next(-100, 200); 
            if (randomDirection < 0)
                currRowFrame++;
            else if (randomDirection >= 0 && randomDirection < 100) // over 100 direction stays the same
                currRowFrame--;

            int randomSpeed = randomGeneration.Next(-100, 100); // if > 0, the wind stays the same
            if (randomSpeed > 0)
                currColumnFrame++;
        }

    }
}

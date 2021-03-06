﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Gusto.Models;
using Microsoft.Xna.Framework.Content;
using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using Gusto.Utility;

namespace Gusto.Models.Menus
{
    public class WindArrows : Sprite, ICanUpdate, IMenuGUI
    {
        private Vector2 Location;
        private int timeSinceLastFrame;
        private int millisecondsPerFrame;

        public WindArrows(Vector2 location, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 10000;
            Texture2D textureWindArrows = content.Load<Texture2D>("WindArrows");
            Asset windArrowsAsset = new Asset(textureWindArrows, null, 2, 8, 1.0f, null, null);
            SetSpriteAsset(windArrowsAsset, location);
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
        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam) // keeping kstate in here for possible powerup to change wind directrion
        {

            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                RandomWind();
                Tuple<int, int> frames = BoundFrames(currRowFrame, currColumnFrame);
                currColumnFrame = frames.Item2;
                currRowFrame = frames.Item1;
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // TODO: abstract method HandleCollision needs to be a layer down from GUI stuff. 
            throw new NotImplementedException();
        }

        private void RandomWind()
        {
            int randomDirection = RandomEvents.rand.Next(-100, 200); 
            if (randomDirection < 0)
                currRowFrame++;
            else if (randomDirection >= 0 && randomDirection < 100) // over 100 direction stays the same
                currRowFrame--;

            int randomSpeed = RandomEvents.rand.Next(-100, 100); // if > 0, the wind stays the same
            if (randomSpeed > 0)
                currColumnFrame++;
        }
    }
}

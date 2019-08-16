using Comora;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated.Weather
{
    public class RainDrop : Sprite
    {

        float msFalling;
        float msToFall;
        float msSwitchFrame;
        float msToSwitchFrame;

        bool hitGround;

        ContentManager _content;
        GraphicsDevice _graphics;

        // Handles timing and combination of weather events
        public RainDrop(ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            _texture = content.Load<Texture2D>("RainDrop");
            nRows = 1;
            nColumns = 4;

            spriteScale = 0.2f;
            msToFall = RandomEvents.rand.Next(500, 1500);
            msToSwitchFrame = 100;

            location.X = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferWidth);
            location.Y = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferHeight);
            
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            // TODO: movement of one raindrop - when it splashes where it starts
            msFalling += gameTime.ElapsedGameTime.Milliseconds;

            if (msFalling > msToFall)
            {
                msSwitchFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msSwitchFrame > msToSwitchFrame)
                {
                    hitGround = true;
                    currColumnFrame++;
                    msSwitchFrame = 0;
                }

                if (currColumnFrame >= nColumns)
                {
                    msFalling = 0;
                    currColumnFrame = 0;
                    location.X = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferWidth);
                    location.Y = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferHeight);
                    hitGround = false;
                }

            }

            // move straight down.. for now?
            if (!hitGround)
                location.Y += 5f;

        }
    }
}

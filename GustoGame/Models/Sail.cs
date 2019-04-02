using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Sail : Sprite
    {
        private Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues; // maps ship frames to vector movement values (Tuple x,y)

        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed

        public float sailSpeed { get; set; }
        public int windWindowAdd { get; set; } // used for shipWindWindow bounds
        public int windWindowSub { get; set; } // ...
        public int sailIsLeftColumn { get; set; }
        public int sailIsRightColumn { get; set; }

        public Sail()
        {
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("tower"))
            {

            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime, int windDir, int windSp)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                // sail direction
                if (kstate.IsKeyDown(Keys.LeftShift))
                {
                    if (kstate.IsKeyDown(Keys.Left))
                        currColumnFrame++;
                    else if (kstate.IsKeyDown(Keys.Right))
                        currColumnFrame--;
                }
                else 
                {
                    // ship direction
                    if (kstate.IsKeyDown(Keys.Left))
                        currRowFrame++;
                    else if (kstate.IsKeyDown(Keys.Right))
                        currRowFrame--;
                }
                BoundFrames();
                timeSinceLastFrame -= millisecondsPerFrame;
            }
        }

    }
}

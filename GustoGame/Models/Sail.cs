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
            MapModelMovementVectorValues();
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
            if (moving)
            {
                // map frame to vector movement
                Tuple<float, float> movementValues = ShipDirectionVectorValues[currRowFrame];
                location.X += movementValues.Item1;
                location.Y += movementValues.Item2;
                Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
        }


        // map ship direction sprite frames (ROWS) to base movement values
        private void MapModelMovementVectorValues()
        {
            float sin45deg = (float)(1 / Math.Sqrt(2));

            ShipDirectionVectorValues = new Dictionary<int, Tuple<float, float>>();
            // map ship direction sprite frames (ROWS) to base movement values
            ShipDirectionVectorValues[0] = new Tuple<float, float>(0, -sailSpeed);
            ShipDirectionVectorValues[1] = new Tuple<float, float>(-(sailSpeed * sin45deg), -sailSpeed * sin45deg); // NW so -25x and +25y
            ShipDirectionVectorValues[2] = new Tuple<float, float>(-(sailSpeed), 0); // W so -50x and 0y
            ShipDirectionVectorValues[3] = new Tuple<float, float>(-sailSpeed * sin45deg, sailSpeed * sin45deg); // ...
            ShipDirectionVectorValues[4] = new Tuple<float, float>(0, (sailSpeed));
            ShipDirectionVectorValues[5] = new Tuple<float, float>(sailSpeed * sin45deg, sailSpeed * sin45deg);
            ShipDirectionVectorValues[6] = new Tuple<float, float>(sailSpeed, 0);
            ShipDirectionVectorValues[7] = new Tuple<float, float>(sailSpeed * sin45deg, -sailSpeed * sin45deg);
        }

    }
}

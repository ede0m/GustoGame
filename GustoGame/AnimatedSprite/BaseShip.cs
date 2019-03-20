using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;

namespace Gusto.AnimatedSprite
{
    public class BaseShip : Ship
    {
        private Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues; // maps ship frames to vector movement values (Tuple x,y)

        public BaseShip(Texture2D texture, Texture2D bbF, int rows, int columns, Vector2 location, float scale, string bbKey) : base(texture, bbF, rows, columns, location, scale, bbKey)
        {
            shipSail = new BaseSail();
            // TODO: align the saile on the 

            timeSinceLastFrame = 0;
            millisecondsPerFrame = 300; // turn speed
            baseMovementSpeed = 0.3f;
            sailUnits = 1;
            health = 100;
            MapModelMovementVectorValues();
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
                } else
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
                SetSailBonusMovement(ShipDirectionVectorValues, windDir, windSp, 2.0f, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
        }

        // map ship direction sprite frames (ROWS) to base movement values
        private void MapModelMovementVectorValues()
        {
            float sin45deg = (float)(1 / Math.Sqrt(2));

            ShipDirectionVectorValues = new Dictionary<int, Tuple<float, float>>();
            // map ship direction sprite frames (ROWS) to base movement values
            ShipDirectionVectorValues[0] = new Tuple<float, float>(0, -baseMovementSpeed);
            ShipDirectionVectorValues[1] = new Tuple<float, float>(-(baseMovementSpeed * sin45deg), -baseMovementSpeed * sin45deg); // NW so -25x and +25y
            ShipDirectionVectorValues[2] = new Tuple<float, float>(-(baseMovementSpeed), 0); // W so -50x and 0y
            ShipDirectionVectorValues[3] = new Tuple<float, float>(-baseMovementSpeed * sin45deg, baseMovementSpeed * sin45deg); // ...
            ShipDirectionVectorValues[4] = new Tuple<float, float>(0, (baseMovementSpeed));
            ShipDirectionVectorValues[5] = new Tuple<float, float>(baseMovementSpeed * sin45deg, baseMovementSpeed * sin45deg);
            ShipDirectionVectorValues[6] = new Tuple<float, float>(baseMovementSpeed, 0);
            ShipDirectionVectorValues[7] = new Tuple<float, float>(baseMovementSpeed * sin45deg, -baseMovementSpeed * sin45deg);
        }

    }
}

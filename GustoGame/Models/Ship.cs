using Gusto.AnimatedSprite;
using Gusto.Bounds;
using Gusto.Utility;
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
    public class Ship : Sprite, IShip
    {
        private Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues; // maps ship frames to vector movement values (Tuple x,y)

        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed

        public float baseMovementSpeed;
        public int health;
        public int sailUnits;
        int shipWindWindowMax;
        int shipWindWindowMin;
        int sailPositionInRespectToShip;

        public Sail shipSail { get; set; }

        public Ship() 
        {
        }


        // Ship collision handler
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
                if (!kstate.IsKeyDown(Keys.LeftShift))
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
                SetSailBonusMovement(ShipDirectionVectorValues, windDir, windSp, shipSail.sailSpeed, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
        }

        /* Adds movement values to X Y location vector based on sail position with wind. 
         * Logic works because the ship direction sprite frames and wind direction sprite frames are aligned.*/
        public void SetSailBonusMovement(Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues, 
             int windDirection, int windSpeed, float sailSpeedBonus, int sailRColumn, int sailLColumn)
        {
            bool sailDirectlyInWind = false;
            // get ship direction (sign of the X Y cords)
            int xdir = Math.Sign(ShipDirectionVectorValues[currRowFrame].Item1);
            int ydir = Math.Sign(ShipDirectionVectorValues[currRowFrame].Item2);
            // construct ship window
            shipWindWindowMax = windDirection + shipSail.windWindowAdd;
            shipWindWindowMin = windDirection - shipSail.windWindowSub;

            sailPositionInRespectToShip = shipSail.currRowFrame; // TODO: this (row) needs to draw from sail sprite data

            BoundShipWindow();

            int addedWindWindow = windDirection;
            //sailPositionInRespectToShip = currRowFrame;
            // sail in wind direction bonus (expands ShipWindWindow)
            if (currColumnFrame == sailRColumn)  // sail is right
            {
                sailPositionInRespectToShip--;
                addedWindWindow = shipWindWindowMax;
                shipWindWindowMax++;
            }
            else if (currColumnFrame == sailLColumn) // sail is left
            {
                sailPositionInRespectToShip++;
                addedWindWindow = shipWindWindowMin;
                shipWindWindowMin--;
            }
            BoundShipWindow();

            // bonus for sailing directly into wind
            if (sailPositionInRespectToShip == windDirection)
                sailDirectlyInWind = true;

            // is the ship able to catch wind?
            if (currRowFrame == shipWindWindowMin || currRowFrame == shipWindWindowMax || currRowFrame == addedWindWindow || currRowFrame == windDirection)
            {

                Trace.WriteLine("CATCHING WIND\n ship pos: " + currRowFrame.ToString() + "\n Max: " + shipWindWindowMax.ToString() + " windDir: " + windDirection.ToString() + " Min: " + shipWindWindowMin.ToString());
                location.Y += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus * windSpeed;
                location.X += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus * windSpeed;
                if (sailDirectlyInWind)
                {
                    location.Y += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus;
                    location.X += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus;
                }
            }

            // set the sail location here (equal to ship location plus the offset on the texture to hit the mount)
            int sailMountX = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item1;
            int sailMountY = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item2;

            shipSail.location.X = location.X + sailMountX;
            shipSail.location.Y = location.Y + sailMountY;
        }
        
        // handles cycling the shipWindWindow and sail position against ship direction
        private void BoundShipWindow()
        {
            if (shipWindWindowMax == nRows)
                shipWindWindowMax = 0;
            if (shipWindWindowMin == -1)
                shipWindWindowMin = nRows - 1;

            if (sailPositionInRespectToShip == -1)
                sailPositionInRespectToShip = nRows - 1;
            else if (sailPositionInRespectToShip == nRows)
                sailPositionInRespectToShip = 0;
        }

        // map ship direction sprite frames (ROWS) to base movement values
        public void MapModelMovementVectorValues()
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

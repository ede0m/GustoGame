using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public float baseMovementSpeed;
        public int health;
        public int shipWindWindowAdd; // used to see if the ship can catch wind
        public int shipWindWindowSub; // ...
        int shipWindWindowMax;
        int shipWindWindowMin;
        int sailPositionInRespectToShip;

        public Ship(Texture2D texture, Texture2D boundingBoxFrame, int rows, int columns, Vector2 startingLoc, float scale, string bbKey) 
            : base(texture, boundingBoxFrame, rows, columns, startingLoc, scale, bbKey){}

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.boundingBoxKey.Equals("tower"))
            {

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
            shipWindWindowMax = windDirection + shipWindWindowAdd;
            shipWindWindowMin = windDirection - shipWindWindowSub;
            sailPositionInRespectToShip = currRowFrame;
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

    }
}

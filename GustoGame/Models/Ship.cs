﻿using Gusto.AnimatedSprite;
using Gusto.Mappings;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gusto.Models
{
    public class Ship : Sprite, IShip
    {
        private Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues; // maps ship frames to vector movement values (Tuple x,y)

        private ContentManager _content;
        private GraphicsDevice _graphics;

        Rectangle aimLine;
        Vector2 startAimLine;
        Vector2 endAimLine;

        public int timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public int maxShotsMoving;
        public float range;
        public int timeSinceLastTurn;
        public int millisecondsPerTurn; // turning speed

        public float baseMovementSpeed;
        public int health;
        public int nSails;
        public int nCannons;
        public bool aiming;
        int shipWindWindowMax;
        int shipWindWindowMin;
        int sailPositionInRespectToShip;

        Random rand;
        public TeamType teamType;
        public Sail shipSail { get; set; }
        public List<Cannon> Cannons { get; set; }
        public List<CannonBall> Shots;

        public Ship(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            aimLine = new Rectangle();
            Shots = new List<CannonBall>();
            Cannons = new List<Cannon>();
            teamType = type;
            _content = content;
            _graphics = graphics;
            rand = new Random();
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
            timeSinceLastTurn += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            foreach (var shot in Shots)
                shot.Update(kstate, gameTime);

            if (colliding)
                moving = false;
            else
                moving = true;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
                aiming = true;
                startAimLine = Cannons[0].GetBoundingBox().Location.ToVector2(); // TODO - Remove hard code and draw line for all cannons
                endAimLine.X = Mouse.GetState().X;
                endAimLine.Y = Mouse.GetState().Y;
            } else { aiming = false; }

            if (timeSinceLastExpClean > millisecondsExplosionLasts)
            {
                // remove exploded shots
                for (int i = 0; i < Shots.Count; i++)
                {
                    if (Shots[i].exploded || Shots[i].outOfRange)
                        Shots.RemoveAt(i);
                }
                timeSinceLastExpClean = 0;
            }

            if (aiming && kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
            {
                Tuple<int, int> shotDirection = new Tuple<int, int>((int)endAimLine.X, (int)endAimLine.Y);
                BaseCannonBall cannonShot = new BaseCannonBall(startAimLine, _content, _graphics);
                cannonShot.SetFireAtDirection(shotDirection, RandomEvents.RandomShotSpeed(this.rand), RandomEvents.RandomAimOffset(this.rand));
                cannonShot.moving = true;
                Shots.Add(cannonShot);
                timeSinceLastShot = 0;
            }

            if (timeSinceLastTurn > millisecondsPerTurn)
            {
                // sail direction
                if (!kstate.IsKeyDown(Keys.LeftShift))
                {
                    // ship direction
                    if (kstate.IsKeyDown(Keys.A))
                        currRowFrame++;
                    else if (kstate.IsKeyDown(Keys.D))
                        currRowFrame--;
                }
                BoundFrames();
                timeSinceLastTurn -= millisecondsPerTurn;
            }
            if (moving)
            {
                // map frame to vector movement
                Tuple<float, float> movementValues = ShipDirectionVectorValues[currRowFrame];
                Tuple<float, float> bonus = SetSailBonusMovement(ShipDirectionVectorValues, windDir, windSp, shipSail.sailSpeed, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                location.X += movementValues.Item1 + bonus.Item1;
                location.Y += movementValues.Item2 + bonus.Item2;
                //Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");

                foreach (var cannon in Cannons)
                {
                    cannon.Update(kstate, gameTime, movementValues);
                    cannon.location.X = location.X;
                    cannon.location.Y = location.Y; 
;                }
                // set the sail location here (equal to ship location plus the offset on the texture to hit the mount)
                int sailMountX = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item1;
                int sailMountY = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item2;
                shipSail.location.X = location.X + sailMountX;
                shipSail.location.Y = location.Y + sailMountY;
            }
        }

        /* Adds movement values to X Y location vector based on sail position with wind. 
         * Logic works because the ship direction sprite frames and wind direction sprite frames are aligned.*/
        public Tuple<float, float> SetSailBonusMovement(Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues, 
             int windDirection, int windSpeed, float sailSpeedBonus, int sailRColumn, int sailLColumn)
        {
            bool sailDirectlyInWind = false;
            float xBonus = 0f;
            float yBonus = 0f;

            // construct ship window
            shipWindWindowMax = windDirection + shipSail.windWindowAdd;
            shipWindWindowMin = windDirection - shipSail.windWindowSub;

            sailPositionInRespectToShip = shipSail.currRowFrame;
            BoundShipWindow();

            int addedWindWindow = windDirection;
            // sail in wind direction bonus (expands ShipWindWindow)
            if (shipSail.currColumnFrame == sailRColumn)  // sail is right
            {
                sailPositionInRespectToShip--;
                addedWindWindow = shipWindWindowMax;
                shipWindWindowMax++;
            }
            else if (shipSail.currColumnFrame == sailLColumn) // sail is left
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

                Trace.WriteLine("\nCATCHING WIND\n ship pos: " + currRowFrame.ToString() + "\n Max: " + shipWindWindowMax.ToString() + " windDir: " + windDirection.ToString() + " Min: " + shipWindWindowMin.ToString() + "\n");
                yBonus += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus * windSpeed;
                xBonus += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus * windSpeed;
                if (sailDirectlyInWind)
                {
                    yBonus += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus;
                    xBonus += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus;
                }
            }
            return new Tuple<float, float>(xBonus, yBonus);
        }

        public void DrawAimLine(SpriteBatch sb)
        {
            Texture2D aimLineTexture = new Texture2D(_graphics, 1, 1);
            aimLineTexture.SetData<Color>(new Color[] { Color.DarkSeaGreen });
            Vector2 edge = endAimLine - startAimLine;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            var line = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edge.Length(), 2);
            sb.Begin();
            sb.Draw(aimLineTexture, line, null, Color.DarkSeaGreen, angle, new Vector2(0, 0), SpriteEffects.None, 0);
            sb.End();
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

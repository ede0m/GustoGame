using Comora;
using Gusto.AnimatedSprite;
using Gusto.Mappings;
using Gusto.Models.Weapon;
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
        private ContentManager _content;
        private GraphicsDevice _graphics;

        Vector2 edge;
        Vector2 startAimLine;
        Vector2 endAimLine;

        public int timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public int timeSinceLastTurn;
        public int millisecondsPerTurn; // turning speed

        public float shotRange;
        public float attackRange;
        public float stopRange;
        public float movementSpeed;
        public int health;
        public int maxShotsMoving;
        public int nSails;
        public bool aiming;
        int shipWindWindowMax;
        int shipWindWindowMin;

        Random rand;
        public TeamType teamType;
        public Sail shipSail { get; set; }
        public List<CannonBall> Shots;

        public Ship(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            Shots = new List<CannonBall>();
            teamType = type;
            _content = content;
            _graphics = graphics;
            rand = new Random();
        }

        // Ship collision handler
        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IWeapon)
            {
                colliding = false;
            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime, int windDir, int windSp, Camera camera)
        {
            timeSinceLastTurn += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            // AI logic
            if (teamType != TeamType.Player)
                AIUpdate(gameTime);
            // player logic
            else
                PlayerUpdate(kstate, gameTime, camera);
            
            // clean shots
            foreach (var shot in Shots)
                shot.Update(kstate, gameTime);
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

            if (moving)
            {
                // map frame to vector movement
                Tuple<float, float> bonus = SetSailBonusMovement(ShipMovementVectorMapping.ShipDirectionVectorValues, windDir, windSp, shipSail.sailSpeed, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                location.X += ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item1 + bonus.Item1;
                location.Y += ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item2 + bonus.Item2;
                //Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
            // set the sail and cannon offsets here (equal to ship location plus the offset on the texture to hit the mount)
            int sailMountX = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item1;
            int sailMountY = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item2;
            shipSail.location.X = location.X + sailMountX;
            shipSail.location.Y = location.Y + sailMountY;
            shipSail.Update(kstate, gameTime, windDir, windSp);
        }

        private void PlayerUpdate(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
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
                    BoundFrames();
                }

                timeSinceLastTurn -= millisecondsPerTurn;
            }

            // aiming
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
                aiming = true;
                startAimLine = GetBoundingBox().Center.ToVector2();

                Vector2 mousePos = new Vector2(Mouse.GetState().X  , Mouse.GetState().Y );

                var lineDistance = PhysicsUtility.VectorMagnitude(mousePos.X, startAimLine.X, mousePos.Y, startAimLine.Y);
                if (lineDistance > shotRange)
                {
                    float disRatio = shotRange / lineDistance;
                    Vector2 maxPos = new Vector2(((1 - disRatio) * startAimLine.X + (disRatio * mousePos.X)), ((1 - disRatio) * startAimLine.Y + (disRatio * mousePos.Y)));
                    endAimLine.X = maxPos.X;
                    endAimLine.Y = maxPos.Y;
                }
                else
                {
                    endAimLine.X = mousePos.X;
                    endAimLine.Y = mousePos.Y;
                }
            }
            else { aiming = false; }

            // shooting
            if (aiming && kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
            {
                Tuple<int, int> shotDirection = new Tuple<int, int>((int)endAimLine.X, (int)endAimLine.Y);
                BaseCannonBall cannonShot = new BaseCannonBall(teamType, startAimLine, _content, _graphics);
                int cannonBallTextureCenterOffsetX = cannonShot.targetRectangle.Width / 2;
                int cannonBallTextureCenterOffsetY = cannonShot.targetRectangle.Height / 2;
                cannonShot.location.X -= cannonBallTextureCenterOffsetX;
                cannonShot.location.Y -= cannonBallTextureCenterOffsetY;
                cannonShot.SetFireAtDirection(shotDirection, RandomEvents.RandomShotSpeed(this.rand), RandomEvents.RandomAimOffset(this.rand));
                cannonShot.moving = true;
                Shots.Add(cannonShot);
                timeSinceLastShot = 0;
            }

            if (colliding)
            {
                moving = false;
                shipSail.moving = false;
            }
            else
            {
                moving = true;
                shipSail.moving = true;
            }
        }

        private void AIUpdate(GameTime gameTime)
        {
            // AI ship direction and movement
            if (timeSinceLastTurn > millisecondsPerTurn)
            {
                Tuple<int, int> target = AIUtility.ChooseTarget(teamType, shotRange, GetBoundingBox());
                if (target == null)
                {
                    moving = false;
                    shipSail.moving = false;
                    return;
                }
                var distanceToTarget = PhysicsUtility.VectorMagnitude(target.Item1, location.X, target.Item2, location.Y);
                if (distanceToTarget <= stopRange)
                {
                    moving = false;
                    shipSail.moving = false;
                }
                else
                {
                    moving = true;
                    shipSail.moving = true;
                }

                currRowFrame = AIUtility.SetAIShipDirection(target, location);
                shipSail.currRowFrame = currRowFrame;

                timeSinceLastTurn -= millisecondsPerTurn;
            }

            // AI Ship Shooting
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastShot > millisecondsNewShot)
            {
                Tuple<int, int> shotDirection = AIUtility.ChooseTarget(teamType, shotRange, GetBoundingBox());
                if (shotDirection != null)
                {
                    Vector2 shipCenter = GetBoundingBox().Center.ToVector2();
                    BaseCannonBall cannonShot = new BaseCannonBall(teamType, shipCenter, _content, _graphics);
                    int cannonBallTextureCenterOffsetX = cannonShot.targetRectangle.Width / 2;
                    int cannonBallTextureCenterOffsetY = cannonShot.targetRectangle.Height / 2;
                    cannonShot.location.X -= cannonBallTextureCenterOffsetX;
                    cannonShot.location.Y -= cannonBallTextureCenterOffsetY;
                    cannonShot.SetFireAtDirection(shotDirection, RandomEvents.RandomShotSpeed(rand), RandomEvents.RandomAimOffset(rand));
                    cannonShot.moving = true;
                    Shots.Add(cannonShot);
                }
                timeSinceLastShot = 0;
            }
        }

        /* Adds movement values to X Y location vector based on sail position with wind. 
         * Logic works because the ship direction sprite frames and wind direction sprite frames are aligned.*/
        public Tuple<float, float> SetSailBonusMovement(Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues, 
             int windDirection, int windSpeed, float sailSpeedBonus, int sailRColumn, int sailLColumn)
        {
            shipSail.sailDirectlyInWind = false;
            float xBonus = 0f;
            float yBonus = 0f;

            // construct ship window
            shipWindWindowMax = windDirection + shipSail.windWindowAdd;
            shipWindWindowMin = windDirection - shipSail.windWindowSub;

            shipSail.sailPositionInRespectToShip = shipSail.currRowFrame;
            BoundShipWindow();

            int addedWindWindow = windDirection;
            // sail in wind direction bonus (expands ShipWindWindow)
            if (shipSail.currColumnFrame == sailRColumn)  // sail is right
            {
                shipSail.sailPositionInRespectToShip--;
                addedWindWindow = shipWindWindowMax;
                shipWindWindowMax++;
            }
            else if (shipSail.currColumnFrame == sailLColumn) // sail is left
            {
                shipSail.sailPositionInRespectToShip++;
                addedWindWindow = shipWindWindowMin;
                shipWindWindowMin--;
            }
            BoundShipWindow();

            // bonus for sailing directly into wind
            if (shipSail.sailPositionInRespectToShip == windDirection)
                shipSail.sailDirectlyInWind = true;

            shipSail.SetWindWindow(shipWindWindowMin, shipWindWindowMax, addedWindWindow);

            // is the ship able to catch wind?
            if (currRowFrame == shipWindWindowMin || currRowFrame == shipWindWindowMax || currRowFrame == addedWindWindow || currRowFrame == windDirection)
            {

                Trace.WriteLine("\nCATCHING WIND\n ship pos: " + currRowFrame.ToString() + "\n Max: " + shipWindWindowMax.ToString() + " windDir: " + windDirection.ToString() + " Min: " + shipWindWindowMin.ToString() + "\n");
                yBonus += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus * windSpeed;
                xBonus += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus * windSpeed;
                if (shipSail.sailDirectlyInWind)
                {
                    yBonus += ShipDirectionVectorValues[currRowFrame].Item2 * sailSpeedBonus;
                    xBonus += ShipDirectionVectorValues[currRowFrame].Item1 * sailSpeedBonus;
                }
            }
            return new Tuple<float, float>(xBonus, yBonus);
        }

        public void DrawAimLine(SpriteBatch sb, Camera camera)
        {
            Texture2D aimLineTexture = new Texture2D(_graphics, 1, 1);
            aimLineTexture.SetData<Color>(new Color[] { Color.DarkSeaGreen });

            //Vector2 perpendicularLinePlus = new Vector2(startAimLine.X + ShipDirectionVectorValues[currRowFrame].Item1, startAimLine.Y + ShipDirectionVectorValues[currRowFrame].Item2);
            //Vector2 perpendicularLineMinus = new Vector2(startAimLine.X - ShipDirectionVectorValues[currRowFrame].Item1, startAimLine.Y - ShipDirectionVectorValues[currRowFrame].Item2);
            edge = endAimLine - startAimLine;
            float radiansToDegrees = 180f / (float)Math.PI;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float angleInDeg = angle * radiansToDegrees;
            var line = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edge.Length(), 2);
            sb.Begin(camera);
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

            if (shipSail.sailPositionInRespectToShip == -1)
                shipSail.sailPositionInRespectToShip = nRows - 1;
            else if (shipSail.sailPositionInRespectToShip == nRows)
                shipSail.sailPositionInRespectToShip = 0;
        }
    }
}

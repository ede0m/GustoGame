using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.GameMap;
using Gusto.Mappings;
using Gusto.Models.Animated;
using Gusto.Models.Menus;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gusto.AnimatedSprite.InventoryItems;

namespace Gusto.Models.Animated
{
    public class Ship : Sprite, IShip, IVulnerable, ICanUpdate, IShadowCaster
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public float timeSinceLastShot;
        public float timeSinceStartAnchor;
        public float timeSinceStartRepairing;
        public float timeSinceStartSinking;
        public int timeSinceLastExpClean;
        public int timeSinceLastTurn;
        public int timeShowingHealthBar;
        public float millisecondsNewShot;
        public float millisecondsToAnchor;
        public float msToRepair;
        public float millisecondToSink;
        public int millisecondsExplosionLasts;
        public int millisecondsPerTurn; // turning speed

        // aim line stuff
        Vector2 edgeFull;
        Vector2 edgeReload;
        Vector2 startAimLine;
        Vector2 endAimLineFull;
        Vector2 endAimLineReload;

        private Texture2D meterFull;
        private Texture2D meterProg;

        public float currentShipSpeedX;
        public float currentShipSpeedY;
        public float shotRange;
        public float attackRange;
        public float stopRange;
        public float movementSpeed;
        public float percentNotAnchored;
        public float percentNotRepaired;
        public float sinkingTransparency;
        public int maxShotsMoving;
        public int nSails;
        int shipWindWindowMax;
        int shipWindWindowMin;
        public float health;
        public float fullHealth;
        private bool showHealthBar;
        public bool hittingLand;
        public bool sinking;
        public bool aiming;
        public bool anchored;
        public bool playerAboard;
        public bool playerInInterior;
        bool roaming;

        public TeamType teamType;
        public Sprite randomRoamTile;
        public Sail shipSail { get; set; }
        public List<Ammo> Shots;
        public List<InventoryItem> inventory;
        public Interior shipInterior;
        public InventoryItem ammoLoaded;
        public int maxInventorySlots;

        public Ship(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            Shots = new List<Ammo>();
            inventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots).ToList();
            teamType = type;
            _content = content;
            _graphics = graphics;

            // anchor feat init
            percentNotAnchored = 1;
            meterFull = new Texture2D(_graphics, 1, 1);
            meterProg = new Texture2D(_graphics, 1, 1);

            timeShowingHealthBar = 0;
        }

        // Ship collision handler
        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IWeapon) // weapons don't stop ship movement - its own weapons have already been filtered out
                colliding = false;

            if (collidedWith is IAmmo)
            {
                showHealthBar = true;
                Ammo ball = (Ammo)collidedWith;
                if (!ball.exploded)
                    health -= ball.structureDamage;
                return;
            }

            if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Animated.HandHeld))
            {
                HandHeld handHeld = (HandHeld)collidedWith;
                showHealthBar = true;
                health -= handHeld.damage;
            }

            if (collidedWith.bbKey.Equals("landTile"))
            {
                colliding = false;
                if (!anchored)
                {
                    showHealthBar = true;
                    health -= 1;
                }
                anchored = true;
                return;
            }

            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                colliding = false;
                return;
            }

            if (collidedWith is IShip)
            {
                colliding = false;
                anchored = true;
            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurn += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > GameOptions.millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

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

            int windDir = WeatherState.wind.getWindDirection();
            int windSp = WeatherState.wind.getWindSpeed();

            if (moving)
            {
                // map frame to vector movement
                Tuple<float, float> bonus = SetSailBonusMovement(ShipMovementVectorMapping.ShipDirectionVectorValues, windDir, windSp, shipSail.sailSpeed, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                currentShipSpeedX = (ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item1 + bonus.Item1) * percentNotAnchored;
                currentShipSpeedY = (ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item2 + bonus.Item2) * percentNotAnchored;
                location.X += currentShipSpeedX;
                location.Y += currentShipSpeedY;
                //Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
            else
            {
                currentShipSpeedX = 0;
                currentShipSpeedY = 0;
            }
            // set the sail and cannon offsets here (equal to ship location plus the offset on the texture to hit the mount)
            int sailMountX = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item1;
            int sailMountY = SailMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item2;
            shipSail.location.X = location.X + sailMountX;
            shipSail.location.Y = location.Y + sailMountY;

            // ship sail update
            shipSail.Update(kstate, gameTime, windDir, windSp);

            SpatialBounding.SetQuad(GetBase());
            // sinking
            if (health <= 0)
            {
                sinking = true;
                currRowFrame = 2;
                shipSail.currRowFrame = 2;

                timeSinceStartSinking += gameTime.ElapsedGameTime.Milliseconds;
                sinkingTransparency = 1 - (timeSinceStartSinking / millisecondToSink);
                shipSail.sinkingTransparency = sinkingTransparency;
                if(sinkingTransparency <= 0)
                {
                    remove = true;

                    // drop items
                    foreach (var item in inventory)
                    {
                        if (item == null)
                            continue;

                        // TODO: drop (package up) all items as barrels/chests
                        if (item.bbKey.Equals("baseBarrelItem"))
                        {
                            BaseBarrel b = new BaseBarrel(teamType, regionKey, location, _content, _graphics);
                            // scatter items
                            b.location.X = location.X + RandomEvents.rand.Next(-40, 40);
                            b.location.Y = location.Y + RandomEvents.rand.Next(-40, 40);
                            ItemUtility.ItemsToUpdate.Add(b);
                        }
                        else if (item.bbKey.Equals("baseChestItem"))
                        {
                            BaseChest c = new BaseChest(teamType, regionKey, location, _content, _graphics);
                            // scatter items
                            c.location.X = location.X + RandomEvents.rand.Next(-40, 40);
                            c.location.Y = location.Y + RandomEvents.rand.Next(-40, 40);
                            ItemUtility.ItemsToUpdate.Add(c);
                        }
                    }
                    inventory.Clear();
                }
            }
        }

        private void PlayerUpdate(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            //health = 40; //UNLIMITED HEALTH
            
            // turning
            if (timeSinceLastTurn > millisecondsPerTurn && playerAboard && !playerInInterior)
            {
                if (!kstate.IsKeyDown(Keys.LeftShift))
                {
                    // ship direction
                    if (kstate.IsKeyDown(Keys.A))
                        currRowFrame++;
                    else if (kstate.IsKeyDown(Keys.D))
                        currRowFrame--;
                    Tuple<int, int> frames = BoundFrames(currRowFrame, currColumnFrame);
                    currRowFrame = frames.Item1;
                    currColumnFrame = frames.Item2;
                }
                timeSinceLastTurn -= millisecondsPerTurn;
            }

            // anchoring toggle
            if (kstate.IsKeyDown(Keys.S) && playerAboard)
            {
                if (anchored)
                {
                    percentNotAnchored = (timeSinceStartAnchor / millisecondsToAnchor);
                    timeSinceStartAnchor += gameTime.ElapsedGameTime.Milliseconds;
                    if (percentNotAnchored >= 1)
                    {
                        anchored = false;
                        timeSinceStartAnchor = 0;
                    }
                }
                else
                {
                    percentNotAnchored = 1 - (timeSinceStartAnchor / millisecondsToAnchor);
                    timeSinceStartAnchor += gameTime.ElapsedGameTime.Milliseconds;
                    if (timeSinceStartAnchor >= millisecondsToAnchor)
                    {
                        anchored = true;
                        timeSinceStartAnchor = 0;
                        percentNotAnchored = 0;
                    }
                }
            }
            else if ((percentNotAnchored <= 1) && playerAboard)
            {
                if (anchored)
                    percentNotAnchored = 0;
                else
                    percentNotAnchored = 1;
                timeSinceStartAnchor = 0;
            }

            // repairing
            //TODO - divide repair time by number of crew on board

            if (kstate.IsKeyDown(Keys.R) && playerAboard && percentNotRepaired > 0)
            {

                int? plankIndex = null;
                // find plank 
                for (int i = 0; i < inventory.Count(); i++)
                {
                    var item = inventory[i];
                    if (item != null && item is IPlank && item.amountStacked > 0)
                    {
                        plankIndex = i;
                        break;
                    }
                }

                if (plankIndex != null)
                {
                    percentNotRepaired = 1 - (timeSinceStartRepairing / msToRepair);
                    timeSinceStartRepairing += gameTime.ElapsedGameTime.Milliseconds;

                    if (percentNotRepaired <= 0 && plankIndex != null)
                    {
                        var item = inventory[(int)plankIndex];
                        health += item.restorePoints;
                        timeSinceStartRepairing = 0;

                        item.amountStacked -= 1;
                        if (item.amountStacked <= 0)
                            item = null;
                    }

                }

            }
            else
            {
                if (health >= fullHealth)
                    percentNotRepaired = 0;
                else
                    percentNotRepaired = 1; // for one repair action..

                timeSinceStartRepairing = 0;
            }


            // aiming
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && playerAboard)
            {
                timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
                float percentReloaded = timeSinceLastShot / millisecondsNewShot;

                aiming = true;
                startAimLine = GetBoundingBox().Center.ToVector2();

                Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                Vector2 clickPos = mousePos - new Vector2(GameOptions.PrefferedBackBufferWidth/2, GameOptions.PrefferedBackBufferHeight/2)+ camera.Position;
                Vector2 reloadSpot = new Vector2(((1 - percentReloaded) * startAimLine.X + (percentReloaded * clickPos.X)), ((1 - percentReloaded) * startAimLine.Y + (percentReloaded * clickPos.Y)));

                var lineDistanceFull = PhysicsUtility.VectorMagnitude(clickPos.X, startAimLine.X, clickPos.Y, startAimLine.Y);
                var lineDistanceReload = PhysicsUtility.VectorMagnitude(reloadSpot.X, startAimLine.X, reloadSpot.Y, startAimLine.Y);

                float disRatio = shotRange / lineDistanceFull;
                Vector2 maxPos = new Vector2(((1 - disRatio) * startAimLine.X + (disRatio * clickPos.X)), ((1 - disRatio) * startAimLine.Y + (disRatio * clickPos.Y)));

                // restrict aiming by shotRange
                if (lineDistanceFull > shotRange)
                    endAimLineFull = maxPos;
                else
                    endAimLineFull = clickPos;

                if (lineDistanceReload > lineDistanceFull || lineDistanceReload > shotRange)
                    endAimLineReload = endAimLineFull;
                else
                    endAimLineReload = reloadSpot;
            }
            else { aiming = false; }

            // shooting
            if (aiming && kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot && playerAboard)
            {
                // loading ammo
                if (ammoLoaded == null)
                {
                    for (int i = 0; i < inventory.Count(); i++)
                    {
                        var item = inventory[i];
                        if (item != null && item.GetType() == typeof(Gusto.AnimatedSprite.InventoryItems.CannonBallItem)) // TODO: refactor to support multiple cannon types? Maybe have ship have weaponSelected like inHand
                        {
                            if (item.amountStacked > 0)
                                ammoLoaded = item;
                            break;
                        }
                    }
                }
                else
                {
                    Tuple<int, int> shotDirection = new Tuple<int, int>((int)endAimLineFull.X, (int)endAimLineFull.Y);
                    BaseCannonBall cannonShot = new BaseCannonBall(teamType, regionKey, startAimLine, _content, _graphics);
                    cannonShot.SetFireAtDirection(shotDirection, RandomEvents.rand.Next(10, 25), 0);
                    cannonShot.moving = true;
                    Shots.Add(cannonShot);
                    timeSinceLastShot = 0;
                    ammoLoaded.amountStacked -= 1;
                    if (ammoLoaded.amountStacked <= 0)
                        ammoLoaded = null;  
                }
            }


            if (colliding || anchored || !playerAboard || health <= 0)
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
                if (target != null)
                {
                    roaming = false;
                    var distanceToTarget = PhysicsUtility.VectorMagnitude(target.Item1, location.X, target.Item2, location.Y);
                    if (distanceToTarget <= stopRange || health <= 0)
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
                else
                {
                    // TODO: roaming can get stuck... 

                    if (!roaming)
                        randomRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionOceanTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionOceanTiles.Count)];

                    roaming = true;
                    target = new Tuple<int, int>((int)randomRoamTile.GetBoundingBox().X, (int)randomRoamTile.GetBoundingBox().Y);
                    if (GetBoundingBox().Intersects(randomRoamTile.GetBoundingBox()))
                        roaming = false;
                    
                }

                // TODO: need some sort of timer to unachor ai ship when it is stuck.
                //if (anchored)
                //moving = false;


                // collision avoidance take 2
                bool probesCollides = false;
                int lineOfSightDistance = 2500;
                Vector2 shipCenterPoint = GetBoundingBox().Center.ToVector2();
                int crf = currRowFrame;
                Dictionary<int, float> nonCollidingLOSMap = new Dictionary<int, float>(); // rowFrame and los distance to target
                for (int i = 0; i < nRows; i++)
                {
                    Tuple<int, int> LosFrames = BoundFrames(crf, currColumnFrame);
                    crf = LosFrames.Item1;
                    Vector2 pointOfSight = new Vector2(shipCenterPoint.X + ShipMovementVectorMapping.ShipDirectionVectorValues[crf].Item1 * lineOfSightDistance,
                        shipCenterPoint.Y + ShipMovementVectorMapping.ShipDirectionVectorValues[crf].Item2 * lineOfSightDistance);
                    
                    foreach (var land in BoundingBoxLocations.LandTileLocationList)
                    {
                        int padding = GetBoundingBox().Width / 2; // padd the tile pieces with half of the ships width
                        Rectangle bbPadded = new Rectangle(land.GetBoundingBox().X, land.GetBoundingBox().Y, land.GetBoundingBox().Width + padding, land.GetBoundingBox().Height + padding);

                        if (AIUtility.LineIntersectsRect(shipCenterPoint, pointOfSight, bbPadded))
                        {
                            nonCollidingLOSMap.Remove(crf);
                            probesCollides = true;
                            break;
                        }
                        else
                        {
                            if (!nonCollidingLOSMap.ContainsKey(crf))
                                nonCollidingLOSMap.Add(crf, PhysicsUtility.VectorMagnitude(target.Item1, pointOfSight.X, target.Item2, pointOfSight.Y));
                        }   
                    }
                    crf++;
                }

                if (!probesCollides)
                    currRowFrame = AIUtility.SetAIShipDirection(target, location);
                else
                {
                    if (nonCollidingLOSMap.Keys.Count == 0)
                    {
                        // all of our lines of sight collide... TODO
                    }
                    else
                    {
                        // go towards min nonCollidable path to target
                        int bestRowFrame = 0;
                        float minDistance = float.MaxValue;
                        foreach (var los in nonCollidingLOSMap.Keys)
                        {
                            if (nonCollidingLOSMap[los] < minDistance)
                            {
                                minDistance = nonCollidingLOSMap[los];
                                bestRowFrame = los;
                            }
                        }
                        currRowFrame = bestRowFrame;
                    }
                }
                // end collision avoidance

                shipSail.currRowFrame = currRowFrame;
                timeSinceLastTurn -= millisecondsPerTurn;
            }

            // AI Ship Shooting
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastShot > millisecondsNewShot && health > 0)
            {
                Tuple<int, int> shotDirection = AIUtility.ChooseTarget(teamType, shotRange, GetBoundingBox());
                if (shotDirection != null)
                {
                    Vector2 shipCenter = GetBoundingBox().Center.ToVector2();
                    BaseCannonBall cannonShot = new BaseCannonBall(teamType, regionKey, shipCenter, _content, _graphics);
                    int cannonBallTextureCenterOffsetX = cannonShot.targetRectangle.Width / 2;
                    int cannonBallTextureCenterOffsetY = cannonShot.targetRectangle.Height / 2;
                    cannonShot.location.X -= cannonBallTextureCenterOffsetX;
                    cannonShot.location.Y -= cannonBallTextureCenterOffsetY;
                    cannonShot.SetFireAtDirection(shotDirection, RandomEvents.rand.Next(10, 25), RandomEvents.rand.Next(-100, 100)); // 3rd param is aim offset for cannon ai
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
            Texture2D reloadLineTexture = new Texture2D(_graphics, 1, 1);
            aimLineTexture.SetData<Color>(new Color[] { Color.IndianRed });
            reloadLineTexture.SetData<Color>(new Color[] { Color.DarkSeaGreen });

            edgeFull = endAimLineFull - startAimLine;
            edgeReload =  endAimLineReload - startAimLine;
            float angleFull = (float)Math.Atan2(edgeFull.Y, edgeFull.X);
            float angleReload = (float)Math.Atan2(edgeReload.Y, edgeReload.X);

            var lineFull = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edgeFull.Length(), 4);
            var lineReload = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edgeReload.Length(), 4);

            sb.Begin(camera);
            sb.Draw(aimLineTexture, lineFull, null, Color.IndianRed, angleFull, new Vector2(0, 0), SpriteEffects.None, 0);
            sb.Draw(reloadLineTexture, lineReload, null, Color.DarkSeaGreen, angleReload, new Vector2(0, 0), SpriteEffects.None, 0);
            sb.End();
        }

        public void DrawAnchorMeter(SpriteBatch sb, Vector2 pos, Texture2D anchorIcon)
        {
            if (teamType == TeamType.Player)
            {
                meterFull.SetData<Color>(new Color[] { Color.IndianRed });
                meterProg.SetData<Color>(new Color[] { Color.DarkKhaki });
                float progress = (1f - percentNotAnchored) * 40f;
                Rectangle full = new Rectangle((int)pos.X, (int)pos.Y, 40, 40);
                Rectangle prog = new Rectangle((int)pos.X, (int)pos.Y, 40, (int)progress);
                sb.Begin();
                sb.Draw(meterFull, full, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterProg, prog, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(anchorIcon, full, null, Color.AliceBlue, 0, Vector2.Zero, SpriteEffects.None, 0);
                sb.End();
            }
        }

        public void DrawRepairHammer(SpriteBatch sb, Vector2 pos, Texture2D repairIcon)
        {
            if (teamType == TeamType.Player)
            {
                meterFull.SetData<Color>(new Color[] { Color.IndianRed });
                meterProg.SetData<Color>(new Color[] { Color.DarkKhaki });
                float progress = (1f - percentNotRepaired) * 40f;
                Rectangle full = new Rectangle((int)pos.X, (int)pos.Y, 40, 40);
                Rectangle prog = new Rectangle((int)pos.X, (int)pos.Y, 40, (int)progress);
                sb.Begin();
                sb.Draw(meterFull, full, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterProg, prog, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(repairIcon, full, null, Color.AliceBlue, 0, Vector2.Zero, SpriteEffects.None, 0);
                sb.End();
            }
        }

        public void DrawSinking(SpriteBatch sb, Camera camera)
        {
            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            sb.Begin(camera);
            sb.Draw(_texture, location, targetRectangle, Color.White * sinkingTransparency, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height/ nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }

        public void DrawHealthBar(SpriteBatch sb, Camera camera)
        {
            if (showHealthBar)
            {
                Texture2D meterAlive = new Texture2D(_graphics, 1, 1);
                Texture2D meterDead = new Texture2D(_graphics, 1, 1);
                meterAlive.SetData<Color>(new Color[] { Color.DarkKhaki });
                meterDead.SetData<Color>(new Color[] { Color.IndianRed });
                float healthLeft = (1f - (1f - (health / fullHealth))) * 60f;
                Rectangle dead = new Rectangle((int)shipSail.GetBoundingBox().Center.X - 30, (int)shipSail.GetBoundingBox().Center.Y - 130, 60, 7);
                Rectangle alive = new Rectangle((int)shipSail.GetBoundingBox().Center.X - 30, (int)shipSail.GetBoundingBox().Center.Y - 130, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
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

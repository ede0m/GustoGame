using Comora;
using Gusto.Bounding;
using Gusto.GameMap;
using Gusto.Mappings;
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
using Gusto.Models.Types;
using GustoGame.Utility;

namespace Gusto.Models.Animated
{
    public class Ship : Sprite, IShip, IVulnerable, ICanUpdate, IShadowCaster, IHasInterior, IWakes
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        Guid shipId;

        //public float timeSinceLastShot;
        public float timeSinceStartAnchor;
        public float timeSinceStartRepairing;
        public float timeSinceStartSinking;
        public int timeSinceLastTurn;
        public int timeShowingHealthBar;
        public float millisecondsNewShot;
        public float millisecondsToAnchor;
        public float msToRepair;
        public float millisecondToSink;
        public int millisecondsPerTurn; // turning speed
        public float msBoarding;

        private Texture2D meterFull;
        private Texture2D meterProg;

        public Vector2 currentShipSpeed;
        public float stopRange;
        public float movementSpeed;
        public float percentNotAnchored;
        public float percentNotRepaired;
        float percentBoarded;
        public float sinkingTransparency;
        public int maxShotsMoving;
        public int nSails;
        int shipWindWindowMax;
        int shipWindWindowMin;

        Texture2D meterAlive;
        Texture2D meterDead;
        public float health;
        public float fullHealth;
        private bool showHealthBar;
        public bool hittingLand;
        public bool sinking;
        public bool beingBoarded;
        public bool boarded;
        public bool aiming;
        public bool anchored;
        public bool playerAboard;
        public bool playerInInterior;

        public TeamType teamType;
        bool roaming;
        bool following;

        List<TilePiece> currentPath;
        TilePiece currMapCordTile; // used here and not in npcs because npcs can get this value from land tile collision. Ocean tiles are not run through collision because there are so many.
        public Sprite randomRoamTile;

        public WakeParticleEngine wake;
        public Sail shipSail { get; set; }

        public ShipMount mountedOnShip;
        public List<InventoryItem> actionInventory;
        public Interior shipInterior;
        Guid boardingShipInteriorId;
        public int maxInventorySlots;

        public Ship(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            actionInventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots).ToList();
            teamType = type;
            _content = content;
            _graphics = graphics;

            timeShowingHealthBar = 0;

            // anchor feat init
            percentNotAnchored = 1;
            meterFull = new Texture2D(_graphics, 1, 1);
            meterProg = new Texture2D(_graphics, 1, 1);
            meterFull.SetData<Color>(new Color[] { Color.IndianRed });
            meterProg.SetData<Color>(new Color[] { Color.DarkKhaki });

            meterAlive = new Texture2D(_graphics, 1, 1);
            meterDead = new Texture2D(_graphics, 1, 1);
            meterAlive.SetData<Color>(new Color[] { Color.DarkKhaki });
            meterDead.SetData<Color>(new Color[] { Color.IndianRed });

            wake = new WakeParticleEngine(content, location);

            shipId = Guid.NewGuid();
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
                hittingLand = true;
                if (!anchored)
                {
                    showHealthBar = true;
                    health -= 1;
                }
                anchored = true;
                return;
            }

            if (collidedWith.bbKey.Equals("playerPirate") || collidedWith is INPC)
            {
                colliding = false;
                return;
            }

            if (collidedWith is IShip)
            {
                colliding = false;
                anchored = true;

                Ship sh = (Ship)collidedWith;
                if (sh.teamType != teamType)
                    boardingShipInteriorId = sh.shipInterior.interiorId;
            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurn += gameTime.ElapsedGameTime.Milliseconds;

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > GameOptions.millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

            // set our map cord point (initially and when it changes TODO: fix the "optimization" using currMapCordTile neighbors below. It doesn't properly update mapCordPoint when used)
            if (currMapCordTile == null || !currMapCordTile.GetBoundingBox().Intersects(GetBoundingBox()))
            {
                foreach (var tile in BoundingBoxLocations.RegionMap[regionKey].RegionOceanTiles)
                {
                    if (GetBoundingBox().Intersects(tile.GetBoundingBox()))
                    {
                        TilePiece tp = (TilePiece)tile;
                        currMapCordTile = tp;
                        mapCordPoint = tp.mapCordPoint;
                        break;
                    }
                }
            }
            /*else if (!currMapCordTile.GetBoundingBox().Intersects(GetBoundingBox())) // (when it has changed)
            {
                List<TilePiece> neighbors = new List<TilePiece>();
                neighbors.Add(GameMapTiles.map[currMapCordTile.mapCordPoint.Value.X * GameMapTiles.cols + currMapCordTile.mapCordPoint.Value.Y + 1]); // right neighbor
                neighbors.Add(GameMapTiles.map[currMapCordTile.mapCordPoint.Value.X * GameMapTiles.cols + currMapCordTile.mapCordPoint.Value.Y - 1]); // left neighbor
                neighbors.Add(GameMapTiles.map[(currMapCordTile.mapCordPoint.Value.X + 1) * GameMapTiles.cols + currMapCordTile.mapCordPoint.Value.Y]); // bottom neighbor
                neighbors.Add(GameMapTiles.map[(currMapCordTile.mapCordPoint.Value.X - 1) * GameMapTiles.cols + currMapCordTile.mapCordPoint.Value.Y]); // top neighbor
                foreach (var tile in neighbors)
                {
                    if (GetBoundingBox().Intersects(tile.GetBoundingBox()))
                    {
                        TilePiece tp = (TilePiece)tile;
                        currMapCordTile = tp;
                        mapCordPoint = tp.mapCordPoint;
                        break;
                    }
                }
            }*/


            // AI logic
            if (teamType != TeamType.Player)
                AIUpdate(gameTime);
            // player logic
            else
                PlayerUpdate(kstate, gameTime, camera);
            
            int windDir = WeatherState.wind.getWindDirection();
            int windSp = WeatherState.wind.getWindSpeed();

            if (moving)
            {
                // map frame to vector movement
                Tuple<float, float> bonus = SetSailBonusMovement(ShipMovementVectorMapping.ShipDirectionVectorValues, windDir, windSp, shipSail.sailSpeed, shipSail.sailIsRightColumn, shipSail.sailIsLeftColumn);
                currentShipSpeed = new Vector2((ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item1 + bonus.Item1) * percentNotAnchored,
                    (ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item2 + bonus.Item2) * percentNotAnchored);
                location += currentShipSpeed;

                // wake particles
                wake.EmitterLocation = new Vector2(location.X + ShipMountTextureCoordinates.BackOfShipCords[bbKey][currRowFrame].Item1,
                    location.Y + ShipMountTextureCoordinates.BackOfShipCords[bbKey][currRowFrame].Item2); ;
                wake.Update(currentShipSpeed);
                
                //Trace.WriteLine("X: " + location.X.ToString() + "\nY: " + location.Y.ToString() + "\n");
            }
            else
            {
                currentShipSpeed = Vector2.Zero;
            }
            shipInterior.speed = currentShipSpeed;

            // set the sail and cannon offsets here (equal to ship location plus the offset on the texture to hit the mount)
            int sailMountX = ShipMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item1;
            int sailMountY = ShipMountTextureCoordinates.SailMountCords[bbKey][shipSail.bbKey][shipSail.currRowFrame][shipSail.currColumnFrame].Item2;
            shipSail.location.X = location.X + sailMountX;
            shipSail.location.Y = location.Y + sailMountY;

            // ship sail update
            shipSail.Update(kstate, gameTime, windDir, windSp);
            
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

                    // drop items from interior (can also include actionInv someday)
                    foreach (var obj in shipInterior.interiorObjects)
                    {
                        if (obj is IStorage || obj is IContainer)
                        {
                            obj.location.X = location.X + RandomEvents.rand.Next(-40, 40);
                            obj.location.Y = location.Y + RandomEvents.rand.Next(-40, 40);
                            obj.inInteriorId = Guid.Empty;
                            ItemUtility.ItemsToUpdate.Add(obj);
                        }
                    }
                    shipInterior.interiorObjects.Clear();
                }
            }

            // being boarded
            if (boardingShipInteriorId != Guid.Empty)
            {
                msBoarding += gameTime.ElapsedGameTime.Milliseconds;
                percentBoarded = msBoarding / 8000;
                if (msBoarding > 8000)
                {
                    if (teamType != TeamType.Player) // players will initiate their boarding with controls TODO
                    {
                        NpcsBoardShip();
                    }
                }
            }
            else
                msBoarding = 0;

            boardingShipInteriorId = Guid.Empty;
        }

        private void PlayerUpdate(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            health = 40; //UNLIMITED HEALTH
            
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
            if (kstate.IsKeyDown(Keys.S) && playerAboard && !playerInInterior)
            {
                if (anchored)
                {
                    percentNotAnchored = (timeSinceStartAnchor / millisecondsToAnchor);
                    timeSinceStartAnchor += gameTime.ElapsedGameTime.Milliseconds;
                    if (percentNotAnchored >= 1)
                    {
                        anchored = false;
                        timeSinceStartAnchor = 0;

                        // give the player a break and move them a little bit when they are hitting land (to get out of jams)
                        location.X += ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item1 * 25;
                        location.Y += ShipMovementVectorMapping.ShipDirectionVectorValues[currRowFrame].Item2 * 25;
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
                for (int i = 0; i < actionInventory.Count(); i++)
                {
                    var item = actionInventory[i];
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
                        var item = actionInventory[(int)plankIndex];
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

            mountedOnShip = null;
            if (playerAboard)
            {
                // set ship mount (set as first appearing in action Inv)
                for (int i = 0; i < actionInventory.Count; i++)
                {
                    if (actionInventory[i] is IShipMount)
                    {
                        mountedOnShip = (ShipMount)actionInventory[i];
                        mountedOnShip.teamType = teamType;
                        mountedOnShip.remove = false;
                        //mountedOnShip.location = GetBoundingBox().Center.ToVector2(); // displays in center of ship
                        Vector2 weaponPosOffset = new Vector2(ShipMountTextureCoordinates.WeaponMountCords[bbKey][currRowFrame][i].Item1, ShipMountTextureCoordinates.WeaponMountCords[bbKey][currRowFrame][i].Item2);
                        mountedOnShip.location = GetBoundingBox().Center.ToVector2() + weaponPosOffset;
                    }
                }

                // ship mount (aiming and firing)
                if (mountedOnShip != null)
                    mountedOnShip.Update(kstate, gameTime, camera, actionInventory);
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
            // AI only works if NPC aboard
            if (shipInterior.interiorObjects.OfType<Npc>().Any())
            {

                // AI ship direction and movement
                if (timeSinceLastTurn > millisecondsPerTurn)
                {
                    if (!roaming)
                    {
                        if (regionKey.Equals("GustoMap")) // TEMP see pathFind comment below
                            regionKey = "Usopp";

                        randomRoamTile = BoundingBoxLocations.RegionMap[regionKey].RegionOceanTiles[RandomEvents.rand.Next(BoundingBoxLocations.RegionMap[regionKey].RegionOceanTiles.Count)];
                        roaming = true;
                        TilePiece rtp = (TilePiece)randomRoamTile;
                        Point? gridPointTo = rtp.mapCordPoint;
                        currentPath = AIUtility.Pathfind(mapCordPoint.Value, gridPointTo.Value, PathType.Ocean); // NOTE: This freezes the game when hitting GustoMap region (because it is almost all the tiles at the moment)
                    }
                    else
                    {
                        // move to attack/follow target when in range
                        int shotRange = mountedOnShip == null ? 0 : mountedOnShip.shotRange;
                        if (shotRange > 0)
                        {
                            Tuple<Point?, float> targetInfo = AIUtility.ChooseTargetPoint(teamType, shotRange, GetBoundingBox(), inInteriorId, PathType.Ocean);
                            if (targetInfo != null)
                            {
                                // stop distance
                                var distanceToTarget = targetInfo.Item2;
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

                                Point? targetMapCords = targetInfo.Item1;
                                // compute follow path
                                if (!following)
                                {
                                    currentPath = AIUtility.Pathfind(mapCordPoint.Value, targetMapCords.Value, PathType.Ocean);
                                    following = true;
                                }
                            }
                            else
                                following = false;
                        }


                        // we have found the next tile in path
                        if (currentPath[0].GetBoundingBox().Intersects(GetBoundingBox()))
                        {
                            currentPath.RemoveAt(0);
                            if (currentPath.Count == 0)
                            {
                                roaming = false;
                                following = false;
                            }
                        }

                        if (roaming)
                            currRowFrame = AIUtility.SetAIShipDirection(currentPath[0].GetBoundingBox().Center.ToVector2(), location);

                    }

                    shipSail.currRowFrame = currRowFrame;
                    timeSinceLastTurn -= millisecondsPerTurn;
                }

                // AI shooting
                if (mountedOnShip != null)
                {
                    Vector2? shotDirection = AIUtility.ChooseTargetVector(teamType, mountedOnShip.shotRange, GetBoundingBox(), inInteriorId);
                    mountedOnShip.UpdateAIMountShot(gameTime, shotDirection);
                    // temp setting AI to use weapon slot 0
                    Vector2 weaponPosOffset = new Vector2(ShipMountTextureCoordinates.WeaponMountCords[bbKey][currRowFrame][0].Item1, ShipMountTextureCoordinates.WeaponMountCords[bbKey][currRowFrame][0].Item2);
                    mountedOnShip.location = GetBoundingBox().Center.ToVector2() + weaponPosOffset;
                }

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

                //Trace.WriteLine("\nCATCHING WIND\n ship pos: " + currRowFrame.ToString() + "\n Max: " + shipWindWindowMax.ToString() + " windDir: " + windDirection.ToString() + " Min: " + shipWindWindowMin.ToString() + "\n");
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

        private void NpcsBoardShip()
        {
            List<Sprite> toRemove = new List<Sprite>();

            // add npcs over to boarded ship
            foreach (var npc in shipInterior.interiorObjects)
            {
                if (npc is INPC)
                {
                    BoundingBoxLocations.interiorMap[boardingShipInteriorId].interiorObjects.Add(npc);
                    npc.location = BoundingBoxLocations.interiorMap[boardingShipInteriorId].RandomInteriorTile().location;
                    toRemove.Add(npc);
                }
            }

            foreach (var toRem in toRemove)
                shipInterior.interiorObjects.Remove(toRem);
        }

        public void DrawAnchorMeter(SpriteBatch sb, Vector2 pos, Texture2D anchorIcon)
        {
            if (teamType == TeamType.Player)
            {
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

        public void DrawBeingBoarded(SpriteBatch sb, Vector2 pos, Texture2D repairIcon)
        {
            if (teamType == TeamType.Player)
            {
                float progress = (1f - percentBoarded) * 40f;
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

        public Guid GetInteriorForId()
        {
            return shipId;
        }

        public void SetInteriorForId(Guid id)
        {
            shipId = id;
        }

        public WakeParticleEngine GetWakeEngine()
        {
            return wake;
        }
    }
}

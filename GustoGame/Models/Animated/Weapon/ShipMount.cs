
using Comora;
using global::Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gusto.Models.Animated
{
    public class ShipMount : InventoryItem, IWeapon, IShipMount, ILight
    {
        public int timeSinceLastFrame;
        public float timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int millisecondsPerFrame; // turning speed
        public float millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public float msToggleButtonHit;
        public bool nextFrame;
        bool animateShot;
        float msAnimateShot;

        public float damage;
        public bool inCombat;
        public int shotRange;

        // aim line stuff
        Vector2 edgeFull;
        Vector2 edgeReload;
        Vector2 startAimLine;
        Vector2 endAimLineFull;
        Vector2 endAimLineReload;
        Vector2 shotOffsetPos;

        public List<Ammo> Shots;
        public InventoryItem ammoLoaded;
        public int ammoLoadedIndex;
       string firedAmmoKey;

        public bool aiming;
        public Type ammoItemType;
        public Light emittingLight; // if this handheld emits any light

        GraphicsDevice _graphics;
        ContentManager _content;

        public ShipMount(TeamType type, ContentManager content, GraphicsDevice graphics) : base(type, content, graphics)
        {
            Shots = new List<Ammo>();
            _graphics = graphics;
            _content = content;
            teamType = type;
            stackable = false;
            amountStacked = 1;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera, List<InventoryItem> actionInventory)
        {
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            // clean shots
            foreach (var shot in Shots)
                shot.Update(gameTime);
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

            // lighting items
            if (emittingLight != null)
            {
                // toggle light
                if (kstate.IsKeyDown(Keys.T))
                {
                    msToggleButtonHit += gameTime.ElapsedGameTime.Milliseconds;
                    if (msToggleButtonHit > 500) // toggle time 500ms
                    {
                        emittingLight.lit = !emittingLight.lit;
                        msToggleButtonHit = 0;
                    }
                }

                emittingLight.Update(kstate, gameTime, GetBoundingBox().Center.ToVector2());
            }

            aiming = false;

            // aiming
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
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

                // max range
                float disRatio = shotRange / lineDistanceFull;
                Vector2 maxPos = new Vector2(((1 - disRatio) * startAimLine.X + (disRatio * clickPos.X)), ((1 - disRatio) * startAimLine.Y + (disRatio * clickPos.Y)));

                // shot offset from mount
                float shotOffsetRatio = 30 / lineDistanceFull;
                shotOffsetPos = new Vector2(((1 - shotOffsetRatio) * startAimLine.X + (shotOffsetRatio * clickPos.X)), ((1 - shotOffsetRatio) * startAimLine.Y + (shotOffsetRatio * clickPos.Y)));

                // restrict aiming by shotRange
                if (lineDistanceFull > shotRange)
                    endAimLineFull = maxPos;
                else
                    endAimLineFull = clickPos;

                if (lineDistanceReload > lineDistanceFull || lineDistanceReload > shotRange)
                    endAimLineReload = endAimLineFull;
                else
                    endAimLineReload = reloadSpot;

                // rotate the mount
                float angleFull = (float)Math.Atan2(edgeFull.Y, edgeFull.X);
                rotation = angleFull + ((float)Math.PI / 2);
            }
            else {
                aiming = false;
            }

            // shooting
            if (aiming && kstate.IsKeyDown(Keys.Space) && timeSinceLastShot > millisecondsNewShot)
            {

                animateShot = true;
                // loading ammo
                if (ammoLoaded == null || actionInventory[ammoLoadedIndex] == null || actionInventory[ammoLoadedIndex].bbKey != ammoLoaded.bbKey) // ran out of ammo, or switched ammo type. Reload
                {
                    for (int i = 0; i < actionInventory.Count; i++)
                    {
                        var item = actionInventory[i];
                        if (item != null && item is IShipAmmoItem && ammoItemType == item.GetType()) // TODO: selects the first item in ship action inv to shoot
                        {
                            if (item.amountStacked > 0)
                            {
                                ammoLoaded = item;
                                ammoLoadedIndex = i;
                                IShipAmmoItem sai = (IShipAmmoItem)item;
                                firedAmmoKey = sai.GetFiredAmmoKey();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    Ammo shot = (Ammo)ItemUtility.CreateItem(firedAmmoKey, teamType, regionKey, shotOffsetPos, _content, _graphics);
                    float angleFull = (float)Math.Atan2(edgeFull.Y, edgeFull.X);
                    shot.rotation = angleFull + ((float)Math.PI/2);
                    shot.SetFireAtDirection(endAimLineFull, RandomEvents.rand.Next(10, 25), 0);
                    shot.moving = true;
                    Shots.Add(shot);
                    timeSinceLastShot = 0;
                    ammoLoaded.amountStacked -= 1;
                    if (ammoLoaded.amountStacked <= 0)
                        ammoLoaded = null;  
                }
            }

            if (animateShot)
            {
                msAnimateShot += gameTime.ElapsedGameTime.Milliseconds;
                if (msAnimateShot > 70)
                {
                    currColumnFrame++;
                    msAnimateShot = 0;
                }
                if (currColumnFrame >= nColumns)
                {
                    currColumnFrame = 0;
                    animateShot = false;
                }
            }

        }

        public void UpdateAIMountShot(GameTime gameTime, Vector2 target)
        {
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;
            // clean shots
            foreach (var shot in Shots)
                shot.Update(gameTime);
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

            float percentReloaded = timeSinceLastShot / millisecondsNewShot;
            startAimLine = GetBoundingBox().Center.ToVector2();

            aiming = true;
            Vector2 reloadSpot = new Vector2(((1 - percentReloaded) * startAimLine.X + (percentReloaded * target.X)), ((1 - percentReloaded) * startAimLine.Y + (percentReloaded * target.Y)));

            var lineDistanceFull = PhysicsUtility.VectorMagnitude(target.X, startAimLine.X, target.Y, startAimLine.Y);
            var lineDistanceReload = PhysicsUtility.VectorMagnitude(reloadSpot.X, startAimLine.X, reloadSpot.Y, startAimLine.Y);

            // max range
            float disRatio = shotRange / lineDistanceFull;
            Vector2 maxPos = new Vector2(((1 - disRatio) * startAimLine.X + (disRatio * target.X)), ((1 - disRatio) * startAimLine.Y + (disRatio * target.Y)));

            // shot offset from mount
            float shotOffsetRatio = 30 / lineDistanceFull;
            shotOffsetPos = new Vector2(((1 - shotOffsetRatio) * startAimLine.X + (shotOffsetRatio * target.X)), ((1 - shotOffsetRatio) * startAimLine.Y + (shotOffsetRatio * target.Y)));

            // restrict aiming by shotRange
            if (lineDistanceFull > shotRange)
                endAimLineFull = maxPos;
            else
                endAimLineFull = target;

            if (lineDistanceReload > lineDistanceFull || lineDistanceReload > shotRange)
                endAimLineReload = endAimLineFull;
            else
                endAimLineReload = reloadSpot;

            edgeFull = endAimLineFull - startAimLine;
            edgeReload = endAimLineReload - startAimLine;
            // rotate the mount
            float angleFull = (float)Math.Atan2(edgeFull.Y, edgeFull.X);
            rotation = angleFull + ((float)Math.PI / 2);


            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastShot > millisecondsNewShot)
            {
                // TODO: just cannon balls for now. Cannon is hard coded to BaseShip animatedSprite AI for now
                animateShot = true;
                BaseCannonBall cannonShot = new BaseCannonBall(teamType, regionKey, shotOffsetPos, _content, _graphics);
                cannonShot.SetFireAtDirection(target, RandomEvents.rand.Next(10, 25), RandomEvents.rand.Next(-100, 100)); // 3rd param is aim offset for cannon ai
                cannonShot.moving = true;
                Shots.Add(cannonShot);
                timeSinceLastShot = 0;
            }

            if (animateShot)
            {
                msAnimateShot += gameTime.ElapsedGameTime.Milliseconds;
                if (msAnimateShot > 70)
                {
                    currColumnFrame++;
                    msAnimateShot = 0;
                }
                if (currColumnFrame >= nColumns)
                {
                    currColumnFrame = 0;
                    animateShot = false;
                }
            }

        }

        public void LoadAmmo(InventoryItem item)
        {
            ammoLoaded = item;
        }

        public Light GetEmittingLight()
        {
            return emittingLight;
        }


        public void DrawAimLine(SpriteBatch sb, Camera camera)
        {
            Texture2D aimLineTexture = new Texture2D(_graphics, 1, 1);
            Texture2D reloadLineTexture = new Texture2D(_graphics, 1, 1);
            aimLineTexture.SetData<Color>(new Color[] { Color.IndianRed });
            reloadLineTexture.SetData<Color>(new Color[] { Color.DarkSeaGreen });

            edgeFull = endAimLineFull - startAimLine;
            edgeReload = endAimLineReload - startAimLine;
            float angleFull = (float)Math.Atan2(edgeFull.Y, edgeFull.X);
            float angleReload = (float)Math.Atan2(edgeReload.Y, edgeReload.X);

            var lineFull = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edgeFull.Length(), 4);
            var lineReload = new Rectangle((int)startAimLine.X, (int)startAimLine.Y, (int)edgeReload.Length(), 4);

            sb.Begin(camera);
            sb.Draw(aimLineTexture, lineFull, null, Color.IndianRed, angleFull, new Vector2(0, 0), SpriteEffects.None, 0);
            sb.Draw(reloadLineTexture, lineReload, null, Color.DarkSeaGreen, angleReload, new Vector2(0, 0), SpriteEffects.None, 0);
            sb.End();
        }

    }
}

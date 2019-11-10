
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
using System.Diagnostics;

namespace Gusto.Models.Animated
{
    public class Ammo : Sprite, IWeapon, IAmmo, IShadowCaster
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        private float secondsSinceFired;
        public float baseMovementSpeed;
        public float structureDamage;
        public float groundDamage;
        public bool exploded = false;
        public bool outOfRange = false;
        
        public int shotSpeed;
        public bool arcShot;

        public Vector2 shotDirection;
        public Vector2 distanceTraveled;
        public Vector2 shotLength;
        public float vectorMagnitude;

        public Vector2 firedFromLoc;
        public Vector2 firedAtLoc;
        public TeamType teamType;

        public Ammo (Vector2 firedFrom, TeamType type) : base(null)
        {
            teamType = type;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            currColumnFrame++; // explosion
            exploded = true;
            Tuple<int, int> frames = BoundFrames(currRowFrame, currColumnFrame);
            currColumnFrame = frames.Item2;
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(GameTime gameTime)
        {
            if (colliding)
                moving = false;
            else
                moving = true;

            secondsSinceFired += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                if (moving && !exploded)
                {
                    // TODO arc shot
                    if (arcShot)
                    {
                        location += new Vector2(secondsSinceFired * (float)shotDirection.X,
                            shotDirection.Y - (shotDirection.Y * (float)Math.Sin(secondsSinceFired / shotDirection.X / MathHelper.TwoPi)));
                    }
                    else
                        location += shotDirection;

                    distanceTraveled += shotDirection;
                }

                if (distanceTraveled.X > (shotLength.X * 1.5) || distanceTraveled.Y > (shotLength.Y * 1.5))
                    outOfRange = true;
                timeSinceLastFrame -= millisecondsPerFrame;
            }

            SetBoundingBox();
            SpatialBounding.SetQuad(GetBase());
        }

        public void SetFireAtDirection(Vector2 fireAtDirection, int speed, int aimOffset)
        {
            firedAtLoc = fireAtDirection;
            shotSpeed = speed;
            vectorMagnitude = PhysicsUtility.VectorMagnitude(GetBoundingBox().X, fireAtDirection.X, GetBoundingBox().Y, fireAtDirection.Y);
            float shotLenX = Math.Max((int)fireAtDirection.X, GetBoundingBox().X) - Math.Min((int)fireAtDirection.X, GetBoundingBox().X);
            float shotLenY = Math.Max((int)fireAtDirection.Y, GetBoundingBox().Y) - Math.Min((int)fireAtDirection.Y, GetBoundingBox().Y);
            shotLength = new Vector2(shotLenX, shotLenY);

            float shotDirX = (fireAtDirection.X - GetBoundingBox().X + aimOffset) / vectorMagnitude  * speed;
            float shotDirY = (fireAtDirection.Y - GetBoundingBox().Y + aimOffset) /vectorMagnitude * speed;
            shotDirection = new Vector2(shotDirX, shotDirY);
        }
    }
}

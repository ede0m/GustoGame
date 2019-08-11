
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
        public float baseMovementSpeed;
        public float structureDamage;
        public float groundDamage;
        public bool exploded = false;
        public bool outOfRange = false;
        public float shotDirX;
        public float shotDirY;
        public float distanceTraveledX = 0;
        public float distanceTraveledY = 0;
        public float vectorMagnitude;
        int shotLenX;
        int shotLenY;
        public Vector2 firedFromLoc;
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
        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            if (colliding)
                moving = false;
            else
                moving = true;

            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                if (moving && !exploded)
                {
                    location.X += shotDirX;
                    location.Y += shotDirY;
                    distanceTraveledX += Math.Abs(shotDirX);
                    distanceTraveledY += Math.Abs(shotDirY);
                }
                if (distanceTraveledX > (shotLenX * 1.5) || distanceTraveledY > (shotLenY * 1.5))
                    outOfRange = true;
                timeSinceLastFrame -= millisecondsPerFrame;
            }

            SpatialBounding.SetQuad(GetBase());
        }

        public void SetFireAtDirection(Tuple<int,int> fireAtDirection, int shotSpeed, int aimOffset)
        {
            vectorMagnitude = PhysicsUtility.VectorMagnitude(GetBoundingBox().X, fireAtDirection.Item1, GetBoundingBox().Y, fireAtDirection.Item2);
            shotLenX = Math.Max(fireAtDirection.Item1, GetBoundingBox().X) - Math.Min(fireAtDirection.Item1, GetBoundingBox().X);
            shotLenY = Math.Max(fireAtDirection.Item2, GetBoundingBox().Y) - Math.Min(fireAtDirection.Item2, GetBoundingBox().Y);
            shotDirX = (fireAtDirection.Item1 - GetBoundingBox().X + aimOffset) / vectorMagnitude  * shotSpeed;
            shotDirY = (fireAtDirection.Item2 - GetBoundingBox().Y + aimOffset) /vectorMagnitude * shotSpeed;
        }
    }
}

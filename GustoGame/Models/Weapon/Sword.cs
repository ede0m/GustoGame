
using global::Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Weapon;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Gusto.Models
{
    public class Sword : Sprite, IWeapon, IHandHeld
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public float baseMovementSpeed;
        public TeamType teamType;

        public Sword(TeamType type)
        {
            teamType = type;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {

        }

        public void Update(KeyboardState kstate, GameTime gameTime, int currRFrame)
        {
            currRowFrame = currRFrame;
            currColumnFrame++;
            if (currColumnFrame == nColumns)
                currColumnFrame = 0;
            SpatialBounding.SetQuad(GetBase());
        }

    }
}

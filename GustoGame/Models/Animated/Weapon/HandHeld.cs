
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
    public class HandHeld : InventoryItem, IWeapon, IHandHeld
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed

        public float damage;

        public TeamType teamType;

        public HandHeld(TeamType type, ContentManager content, GraphicsDevice graphics) : base(type, content, graphics)
        {
            teamType = type;
            stackable = false;
            amountStacked = 1;
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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Bounds;


namespace Gusto.AnimatedSprite
{
    public class BaseShip : Ship
    {
        public BaseShip(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 300; // turn speed
            baseMovementSpeed = 0.2f;
            sailUnits = 1;
            health = 100;

            MapModelMovementVectorValues();

            Texture2D textureBaseShip = content.Load<Texture2D>("BaseShip");
            Texture2D textureBaseShipBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseShipBB = new Texture2D(graphics, textureBaseShip.Width, textureBaseShip.Height);
            Asset baseShipAsset = new Asset(textureBaseShip, textureBaseShipBB, 1, 8, 0.6f, "baseShip");
            // TEMPORARY -- hardcode basesail to baseship (later on we want base ship to start without a sail)
            shipSail = new BaseSail(location, content, graphics);
            SetSpriteAsset(baseShipAsset, location);
        }
    }
}

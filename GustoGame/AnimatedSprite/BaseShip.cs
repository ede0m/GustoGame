using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;

namespace Gusto.AnimatedSprite
{
    public class BaseShip : Ship
    {
        public BaseShip(Vector2 location, Asset asset) : base(location, asset)
        {

            // TEMPORARY -- hardcode basesail to baseship (later on we want base ship to start without a sail)
            shipSail = new BaseSail(location, AssetFinder.Sails["baseSail"]);
            
            // TODO: align the saile on the 

            timeSinceLastFrame = 0;
            millisecondsPerFrame = 300; // turn speed
            baseMovementSpeed = 0.3f;
            sailUnits = 1;
            health = 100;
        }
    }
}

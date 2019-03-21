using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite
{
    public class BaseSail : Sail
    {

        public BaseSail(Vector2 location, Asset asset) : base(location, asset)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 300; // turn speed
            sailIsLeftColumn = 2;
            sailIsRightColumn = 0;
            sailSpeed = 1.5f;
            windWindowAdd = 1;
            windWindowSub = 1;
        }

    }
}

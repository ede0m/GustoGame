﻿using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class TilePiece : Sprite
    {
        public Sprite groundObject; // any tree, rock, etc asset we want to set to this tile.


        public TilePiece(Sprite _groundObject)
        {
            groundObject = _groundObject;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            
        }

    }
}

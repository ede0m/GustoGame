using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Models.Animated;
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
        bool shoveled;
        bool fill;

        public TilePiece(Sprite _groundObject) : base(null)
        {
            groundObject = _groundObject;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // land tiles are only run through collision detection


            // shoveling 
            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                PiratePlayer p = (PiratePlayer)collidedWith;
                if (p.inHand != null && p.inHand.bbKey.Equals("shovel") && p.inHand.inCombat)
                {
                    Rectangle bottomOfShovel = new Rectangle(p.inHand.GetBoundingBox().Left, p.inHand.GetBoundingBox().Bottom - (p.inHand.GetBoundingBox().Height / 3), 
                        p.inHand.GetBoundingBox().Width, p.inHand.GetBoundingBox().Height / 3);

                    if (bottomOfShovel.Intersects(GetBoundingBox()))
                        shoveled = true;
                }
                else if (shoveled)
                {
                    
                    if (fill)
                    {
                        currColumnFrame--;
                        if (currColumnFrame <= 0)
                        {
                            currColumnFrame = 0;
                            fill = false;
                        }
                    }
                    else
                    {
                        currColumnFrame++;
                        if (currColumnFrame >= nColumns)
                        {
                            currColumnFrame = nColumns - 1;
                            fill = true;
                        }
                    }

                    shoveled = false;

                }
            }
        }

    }
}

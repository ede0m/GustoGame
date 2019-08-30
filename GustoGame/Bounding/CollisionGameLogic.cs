using Gusto.AnimatedSprite;
using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounding
{
    public class CollisionGameLogic
    {


        public static bool CheckCollidable(Sprite a, Sprite b)
        {

            if (a.inInteriorId != Guid.Empty)
            {
                if (b.inInteriorId == Guid.Empty)
                    return false;

                if (b.inInteriorId != a.inInteriorId)
                    return false;
            }

            if (a.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
            {
                Ship ship = (Ship)a;
                // a ship doesn't collide with its any ship's sails
                if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Sail))
                    return false;

                // ship doesn't collide with interior tiles
                if (b is IInteriorTile)
                    return false;

                // ship doesn't collide with its own cannon balls
                if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Ammo)) {
                    Ammo ball = (Ammo)b;
                    if (ball.teamType == ship.teamType)
                        return false;
                }
            }

            else if (a.GetType().BaseType == typeof(Gusto.Models.Animated.PlayerPirate))
            {
                PlayerPirate pirate = (PlayerPirate)a;
                // a player doesn't collide with its own shots
                if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Ammo))
                {
                    Ammo ball = (Ammo)b;
                    if (ball.teamType == pirate.teamType)
                        return false;
                }
                else if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Tree))
                    return false;
            }

            else if (a.GetType().BaseType == typeof(Gusto.Models.Animated.Tower))
            {
                BaseTower tower = (BaseTower)a;
                // a tower doesn't collide with its own shots
                if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Ammo))
                {
                    Ammo ball = (Ammo)b;
                    if (ball.teamType == tower.teamType)
                        return false;
                }
            }

            // hand items only collide when they are being used
            else if (a is IHandHeld)
            {
                HandHeld inHand = (HandHeld)a;
                if (!inHand.usingItem && !inHand.onGround)
                    return false;
            }

            else if (a is IAmmo)
            {
                if (b.GetType().BaseType == typeof(Gusto.Models.TilePiece))
                    return false;
                if (b.GetType().BaseType == typeof(Gusto.Models.Animated.Grass))
                    return false;
            }

            else if (a is IGroundObject)
            {
                if (b.GetType().BaseType == typeof(Gusto.Models.TilePiece))
                    return false;
            }

            return true;
        }
    }
}

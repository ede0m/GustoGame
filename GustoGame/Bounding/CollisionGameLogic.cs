using Gusto.AnimatedSprite;
using Gusto.Models;
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
            if (a.GetType().BaseType == typeof(Gusto.Models.Ship))
            {
                Ship ship = (Ship)a;
                // a ship doesn't collide with its any ship's sails
                if (b.GetType().BaseType == typeof(Gusto.Models.Sail))
                    return false;
                // ship doesn't collide with its own cannon balls
                if (b.GetType().BaseType == typeof(Gusto.Models.CannonBall)) {
                    CannonBall ball = (CannonBall)b;
                    if (ball.teamType == ship.teamType)
                        return false;
                }
            }

            else if (a.GetType().BaseType == typeof(Gusto.Models.Tower))
            {
                BaseTower tower = (BaseTower)a;
                // a tower doesn't collide with its own shots
                if (b.GetType().BaseType == typeof(Gusto.Models.CannonBall))
                {
                    CannonBall ball = (CannonBall)b;
                    if (ball.teamType == tower.teamType)
                        return false;
                }
            }
            return true;
        }
    }
}

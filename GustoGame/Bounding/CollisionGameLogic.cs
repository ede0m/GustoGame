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
                // a ship doesn't collide with its own sail
                if (ship.shipSail == b)
                    return false;
            } else if (a.GetType() == typeof(Gusto.AnimatedSprite.Tower))
            {
                Tower tower = (Tower)a;
                // a tower doesn't collide with its own shots (iterate backwards because newest (closest) shots will be appended on the end)
                for (int i = tower.Shots.Count()-1; i >= 0; i--)
                {
                    if (tower.Shots[i] == b)
                        return false;
                }


            }
            return true;
        }
    }
}

using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public class AIUtility
    {
        // Returns attack postion of target as tuple
        public static Tuple<int, int> ChooseTarget(TeamType teamType, float range, Rectangle bb)
        {
            foreach (var otherTeam in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
            {
                if (AttackMapping.AttackMappings[teamType][otherTeam])
                {
                    Tuple<int, int> shotCords = BoundingBoxLocations.BoundingBoxLocationMap[otherTeam][0]; // TODO REMOVE HARDCODED random target (pick team member with lowest health)
                    float vmag = PhysicsUtility.VectorMagnitude(shotCords.Item1, bb.X, shotCords.Item2, bb.Y);
                    if (vmag <= range)
                        return shotCords;
                }
            }
            return null;
        }

        public static int SetAIShipDirection(Tuple<int, int> target, Vector2 location)
        {
            int currRowFrame = 0;
            float slope = (target.Item2 - location.Y) / (target.Item1 - location.X);

            if (slope > 0)
            {
                if (slope < 2.5 && slope > 0.4)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 5; // upper right
                    else
                        currRowFrame = 1; // lower left
                }
                else if (slope < 0.4 && slope > 0)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope > 2.5)
                {
                    if ((target.Item2 - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            else
            {
                if (slope > -2.5 && slope < -0.4)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 7; // lower right
                    else
                        currRowFrame = 3; // upper left
                }
                else if (slope > -0.4 && slope < 0)
                {
                    if ((target.Item1 - location.X) > 0)
                        currRowFrame = 6; // right
                    else
                        currRowFrame = 2; // left
                }
                else if (slope < -2.5)
                {
                    if ((target.Item2 - location.Y) > 0)
                        currRowFrame = 4; // down
                    else
                        currRowFrame = 0; // up
                }
            }
            return currRowFrame;
        }
    }
}

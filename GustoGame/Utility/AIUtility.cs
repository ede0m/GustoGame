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
    }
}

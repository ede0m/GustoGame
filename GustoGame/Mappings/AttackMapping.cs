using Gusto.AnimatedSprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Mappings
{
    public class AttackMapping
    {
        public static Dictionary<TeamType, Dictionary<TeamType, Boolean>> AttackMappings = new Dictionary<TeamType, Dictionary<TeamType, Boolean>>()
        {
            { TeamType.A, new Dictionary<TeamType, bool>
                {
                    { TeamType.Player, true },
                    { TeamType.A, false}
                }
            },
            { TeamType.Player, new Dictionary<TeamType, bool>
                {
                    { TeamType.Player, false },
                    { TeamType.A, true}
                }
            },
        };
    }
}

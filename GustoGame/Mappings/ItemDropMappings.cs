using Gusto.AnimatedSprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Mappings
{
    public class ItemDropMappings
    {
        public static Dictionary<TeamType, Dictionary<string, Dictionary<string, float>>> ItemDrops = new Dictionary<TeamType, Dictionary<string, Dictionary<string, float>>>
        {
            { TeamType.B, new Dictionary<string, Dictionary<string, float>>
                {
                    {"tribalTokens", new Dictionary<string, float>
                        {
                            {"maxDrop", 8 },
                            {"percentDrop", 85 }
                        }
                    },

                    {"shortSword", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 50 }
                        }
                    }
                }
            },

        };
    }
}

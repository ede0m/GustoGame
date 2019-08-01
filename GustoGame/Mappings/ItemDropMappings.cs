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
        public static Dictionary<string, Dictionary<string, Dictionary<string, float>>> ItemDrops = new Dictionary<string, Dictionary<string, Dictionary<string, float>>>
        {
            { "baseTribal", new Dictionary<string, Dictionary<string, float>>
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
            { "baseShip", new Dictionary<string, Dictionary<string, float>>
                {
                    {"softWood", new Dictionary<string, float>
                        {
                            {"maxDrop", 4 },
                            {"percentDrop", 50 }
                        }
                    },
                    {"basePlank", new Dictionary<string, float>
                        {
                            {"maxDrop", 2 },
                            {"percentDrop", 75 }
                        }
                    },
                }
            },
            { "tree1", new Dictionary<string, Dictionary<string, float>>
                {
                    {"softWood", new Dictionary<string, float>
                        {
                            {"maxDrop", 4 },
                            {"percentDrop", 100 }
                        }
                    },
                }
            },
            { "grass1", new Dictionary<string, Dictionary<string, float>>
                {
                    {"islandGrass", new Dictionary<string, float>
                        {
                            {"maxDrop", 4 },
                            {"percentDrop", 80}
                        }
                    },
                }
            },

        };
    }
}

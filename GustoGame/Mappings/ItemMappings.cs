using Gusto.AnimatedSprite;
using Gusto.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Mappings
{
    public class ItemMappings
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
            { "rock1", new Dictionary<string, Dictionary<string, float>>
                {
                    {"coal", new Dictionary<string, float>
                        {
                            {"maxDrop", 2 },
                            {"percentDrop", 30}
                        }
                    },
                    {"ironOre", new Dictionary<string, float>
                        {
                            {"maxDrop", 3},
                            {"percentDrop", 80}
                        }
                    },
                }
            },

        };

        // TODO:could make this a narrowing index by ingredients
        public static Dictionary<string, Dictionary<string, int>> CraftingRecipes = new Dictionary<string, Dictionary<string, int>>
        {
            { "anvilItem", new Dictionary<string, int>
                {
                    {"ironBar", 1} // TODO: change back to 5
                }
            },
            { "baseSword", new Dictionary<string, int>
                {
                    {"ironBar", 2},
                    {"softWood", 2}
                }
            },
            { "nails", new Dictionary<string, int>
                {
                    {"ironOre", 2}
                }
            },

        };

    }
}

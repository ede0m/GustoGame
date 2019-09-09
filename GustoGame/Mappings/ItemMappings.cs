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
                            {"percentDrop", 20 }
                        }
                    }
                }
            },
            { "baseCat", new Dictionary<string, Dictionary<string, float>>
                {
                    {"shortSword", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 20 }
                        }
                    },
                    {"chiliPepper", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 40 }
                        }
                    },
                    {"cookedFish", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 30 }
                        }
                    },
                }
            },
            { "chicken", new Dictionary<string, Dictionary<string, float>>
                {
                    {"rawMeat", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 75 }
                        }
                    },
                    {"feather", new Dictionary<string, float>
                        {
                            {"maxDrop", 5 },
                            {"percentDrop", 70 }
                        }
                    },
                }
            },
            { "blueBird", new Dictionary<string, Dictionary<string, float>>
                {
                    {"rawMeat", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 75 }
                        }
                    },
                    {"feather", new Dictionary<string, float>
                        {
                            {"maxDrop", 5 },
                            {"percentDrop", 70 }
                        }
                    },
                }
            },
            { "snake", new Dictionary<string, Dictionary<string, float>>
                {
                    {"rawMeat", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 50 }
                        }
                    },
                    {"scales", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 50 }
                        }
                    },
                }
            },
            { "teePee", new Dictionary<string, Dictionary<string, float>>
                {

                }
            },

            { "baseShip", new Dictionary<string, Dictionary<string, float>>
                {
                    {"baseBarrel", new Dictionary<string, float>
                        {
                            {"maxDrop", 4 },
                            {"percentDrop", 50 }
                        }
                    },
                    {"baseChest", new Dictionary<string, float>
                        {
                            {"maxDrop", 2 },
                            {"percentDrop", 45 }
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
            { "tree2", new Dictionary<string, Dictionary<string, float>>
                {
                    {"softWood", new Dictionary<string, float>
                        {
                            {"maxDrop", 4 },
                            {"percentDrop", 100 }
                        }
                    },
                }
            },
            { "tree3", new Dictionary<string, Dictionary<string, float>>
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
            { "baseBarrel", new Dictionary<string, Dictionary<string, float>>
                {
                    {"coal", new Dictionary<string, float>
                        {
                            {"maxDrop", 2 },
                            {"percentDrop", 50}
                        }
                    },
                    {"ironBar", new Dictionary<string, float>
                        {
                            {"maxDrop", 2},
                            {"percentDrop", 15}
                        }
                    },
                    {"nails", new Dictionary<string, float>
                        {
                            {"maxDrop", 10},
                            {"percentDrop", 50}
                        }
                    },
                    {"pistolShotItem", new Dictionary<string, float>
                        {
                            {"maxDrop", 6},
                            {"percentDrop", 30}
                        }
                    },
                    {"cannonBallItem", new Dictionary<string, float>
                        {
                            {"maxDrop", 3},
                            {"percentDrop", 25}
                        }
                    },
                    {"goldCoins", new Dictionary<string, float>
                        {
                            {"maxDrop", 15 },
                            {"percentDrop", 20 }
                        }
                    },
                    {"fishOil", new Dictionary<string, float>
                        {
                            {"maxDrop", 1 },
                            {"percentDrop", 30 }
                        }
                    },
                }
            },
             { "baseChest", new Dictionary<string, Dictionary<string, float>>
                {
                    {"tribalTokens", new Dictionary<string, float>
                        {
                            {"maxDrop", 16 },
                            {"percentDrop", 85 }
                        }
                    },
                    {"ironBar", new Dictionary<string, float>
                        {
                            {"maxDrop", 4},
                            {"percentDrop", 55}
                        }
                    },
                    {"treasureMapItem", new Dictionary<string, float>
                        {
                            {"maxDrop", 1},
                            {"percentDrop", 100}
                        }
                    },
                    {"goldCoins", new Dictionary<string, float>
                        {
                            {"maxDrop", 30 },
                            {"percentDrop", 40 }
                        }
                    },
                }
            },

        };

        // TODO:could make this a narrowing index by ingredients
        public static Dictionary<string, Dictionary<string, Dictionary<string, int>>> CraftingRecipes = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>
        {
            {"craftT1", new Dictionary<string, Dictionary<string, int>>
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
                }
            },
            {"cookT1", new Dictionary<string, Dictionary<string, int>>
                {
                    { "cookedFish", new Dictionary<string, int>
                        {
                            {"rawFish", 1} 
                        }
                    },
                    { "cookedMeat", new Dictionary<string, int>
                        {
                            {"rawMeat", 1}
                        }
                    },
                    { "chiliFish", new Dictionary<string, int>
                        {
                            {"rawFish", 1},
                            {"chiliPepper", 2}
                        }
                    },
                }
            },
            { "furnaceT1", new Dictionary<string, Dictionary<string, int>>
                {
                    { "ironBar", new Dictionary<string, int>
                        {
                            {"ironOre", 2},
                            { "coal", 1 }
                        }
                    },
                }
            },
        };

        public static Dictionary<string, Tuple<int, string>> SpoilMappings = new Dictionary<string, Tuple<int, string>>
        {
            { "rawMeat", new Tuple<int, string>(20000, "spoiledMeat") },
            { "rawFish", new Tuple<int, string>(20000, "spoiledFish") },
            { "cookedMeat", new Tuple<int, string>(50000, "spoiledMeat") },
            { "cookedFish", new Tuple<int, string>(50000, "spoiledFish") },
        };


    }
}

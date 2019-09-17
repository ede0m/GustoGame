using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Mappings
{
    public class ShipMountTextureCoordinates
    {
        public static Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Tuple<int, int>>>>> SailMountCords = new Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Tuple<int, int>>>>>
        {
            {"baseShip", new Dictionary<string, Dictionary<int, Dictionary<int, Tuple<int, int>>>>
                {
                    {"baseSail", new Dictionary<int, Dictionary<int, Tuple<int, int>>>
                        {
                            {0, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(6, -83)},
                                    { 1, new Tuple<int, int>(0, -75)},
                                    { 2, new Tuple<int, int>(-5, -82)}
                                } 
                            },
                            {1, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(-23, -100)},
                                    { 1, new Tuple<int, int>(-29, -110)},
                                    { 2, new Tuple<int, int>(-32, -92)}
                                }
                            },
                            {2, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(-30, -70)},
                                    { 1, new Tuple<int, int>(-55, -65)},
                                    { 2, new Tuple<int, int>(-27, -82)}
                                }
                            },
                            {3, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(-18, -35)},
                                    { 1, new Tuple<int, int>(-19, -20)},
                                    { 2, new Tuple<int, int>(-25, -21)}
                                }
                            },
                            {4, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(4, -20)},
                                    { 1, new Tuple<int, int>(-2, -20)},
                                    { 2, new Tuple<int, int>(-6, -20)}
                                }
                            },
                            {5, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(25, -21)},
                                    { 1, new Tuple<int, int>(20, -20)},
                                    { 2, new Tuple<int, int>(18, -30)}
                                }
                            },
                            {6, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(27, -84)},
                                    { 1, new Tuple<int, int>(52, -65)},
                                    { 2, new Tuple<int, int>(29, -70)}
                                }
                            },
                            {7, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(32, -90)},
                                    { 1, new Tuple<int, int>(29, -110)},
                                    { 2, new Tuple<int, int>(25, -100)}
                                }
                            },


                        }
                    }
                }
            },
            {"shortShip", new Dictionary<string, Dictionary<int, Dictionary<int, Tuple<int, int>>>>
                {
                    {"shortSail", new Dictionary<int, Dictionary<int, Tuple<int, int>>>
                        {
                            {0, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {1, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {2, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {3, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {4, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {5, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },
                            {6, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(09, 0)}
                                }
                            },
                            {7, new Dictionary<int, Tuple<int, int>>
                                {
                                    { 0, new Tuple<int, int>(0, 0)},
                                    { 1, new Tuple<int, int>(0, 0)},
                                    { 2, new Tuple<int, int>(0, 0)}
                                }
                            },


                        }
                    }
                }
            }

        };

        public static Dictionary<string, Dictionary<int, Dictionary<int,Tuple<int, int>>>> WeaponMountCords = new Dictionary<string, Dictionary<int, Dictionary<int, Tuple<int, int>>>>
        {
            {"baseShip", new Dictionary<int, Dictionary<int, Tuple<int, int>>>
                {
                    {0, new Dictionary<int, Tuple<int, int>> // this int key is curr row index
                        {
                            {0, new Tuple<int, int>(0, 0) }, // this int key is the current weapon slot pos
                            {1, new Tuple<int, int>(-30, -30) },
                            {2, new Tuple<int, int>(30, -30) },
                        }
                    },
                    {1, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(-45, -30) },
                            {2, new Tuple<int, int>(-10, -50) },
                        }
                    },
                    {2, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(-50, 20) },
                            {2, new Tuple<int, int>(-50, -20) },
                        }
                    },
                    {3, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(-15, 65) },
                            {2, new Tuple<int, int>(-45, 30) },
                        }
                    },
                    {4, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(25, 50) },
                            {2, new Tuple<int, int>(-25, 50) },
                        }
                    },
                    {5, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(50, 45) },
                            {2, new Tuple<int, int>(12, 62) },
                        }
                    },
                    {6, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(40, -20) },
                            {2, new Tuple<int, int>(40, 20) },
                        }
                    },
                    {7, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 0) },
                            {1, new Tuple<int, int>(5, -45) },
                            {2, new Tuple<int, int>(35, -15) },
                        }
                    },
                }
            },
            {"shortShip", new Dictionary<int, Dictionary<int, Tuple<int, int>>>
                {
                    {0, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, 48) },
                            {1, new Tuple<int, int>(0, 48) },
                        }
                    },
                    {1, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(46, 26) },
                            {1, new Tuple<int, int>(46, 26) },
                        }
                    },
                    {2, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(60, -5) },
                            {1, new Tuple<int, int>(60, -5) },
                        }
                    },
                    {3, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(50, -30) },
                            {1, new Tuple<int, int>(50, -30) },
                        }
                    },
                    {4, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(0, -50) },
                            {1, new Tuple<int, int>(0, -50) },
                        }
                    },
                    {5, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(-45, -30) },
                            {1, new Tuple<int, int>(-45, -30) },
                        }
                    },
                    {6, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(-60, -5) },
                            {1, new Tuple<int, int>(-60, -5) },
                        }
                    },
                    {7, new Dictionary<int, Tuple<int, int>>
                        {
                            {0, new Tuple<int, int>(-45, 25) },
                            {1, new Tuple<int, int>(-45, 25) },
                        }
                    },
                }
            },
        };

    }
}

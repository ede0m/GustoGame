using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounds
{
    public class SailMountTextureCoordinates
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
                                    { 0, new Tuple<int, int>(-23, -50)},
                                    { 1, new Tuple<int, int>(-28, -58)},
                                    { 2, new Tuple<int, int>(-32, -41)}
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
                                    { 0, new Tuple<int, int>(32, -42)},
                                    { 1, new Tuple<int, int>(30, -60)},
                                    { 2, new Tuple<int, int>(26, -55)}
                                }
                            },


                        }
                    }
                }
            } 
        };

    }
}

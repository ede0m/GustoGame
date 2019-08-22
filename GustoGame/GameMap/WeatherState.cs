using Gusto.GameMap.lightning;
using Gusto.Models.Animated.Weather;
using Gusto.Models.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public static class WeatherState
    {
        public static float weatherDuration { get; set; }
        public static float msThroughWeather { get; set; }

        public static List<RainDrop> rain { get; set; }
        public static RainState rainState { get; set; }
        public static WindArrows wind { get; set; } // this is a static sprite
        public static int rainIntensity { get; set; }
        public static bool lightning { get; set; }
        public static BranchLightning lightningBolt { get; set; }

    }
}

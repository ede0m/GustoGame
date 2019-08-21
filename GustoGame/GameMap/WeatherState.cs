using Gusto.GameMap.lightning;
using Gusto.Models.Animated.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public class WeatherState
    {
        public float weatherDuration { get; set; }
        public float msThroughWeather { get; set; }

        public List<RainDrop> rain { get; set; }
        public RainState rainState { get; set; }
        public int rainIntensity { get; set; }
        public bool lightning { get; set; }
        public BranchLightning lightningBolt { get; set; }

        public WeatherState()
        {
            rain = new List<RainDrop>();
            rainState = RainState.NOT;
            lightning = false;

        }

    }
}

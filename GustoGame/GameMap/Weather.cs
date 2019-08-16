using Comora;
using Gusto.Models.Animated.Weather;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public class Weather
    {
        float msOfWeather;
        float msCurrWeather;

        List<RainDrop> rain;
        bool raining;

        // List of different weather sprites??

        ContentManager _content;
        GraphicsDevice _graphics;

        // Handles timing and combination of weather events
        public Weather(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            rain = new List<RainDrop>();
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            msCurrWeather += gameTime.ElapsedGameTime.Milliseconds;
            if (msCurrWeather >= msOfWeather)
            {
                // new weather pattern time
                msOfWeather = RandomEvents.rand.Next(GameOptions.GameDayLengthMs / 10, GameOptions.GameDayLengthMs * 2); // weather can last between 1/10th of a day and 3 days
                msCurrWeather = 0;
                
                // new weather pattern TODO: extend to other weathers. 
                int randRain = RandomEvents.rand.Next(0, 100);
                if (randRain <= 50) // 20% chance of rain
                {
                    raining = true;
                    rain.Clear();
                    int randRainIntensity = RandomEvents.rand.Next(100, 500);
                    for (int i = 0; i < randRainIntensity; i++)
                        rain.Add(new RainDrop(_content, _graphics));
                }
                else
                {
                    raining = false;
                }
            } 
            else
            {
                // continue with current weather
                if (raining)
                {
                    foreach (var drop in rain)
                        drop.Update(kstate, gameTime);
                }
            }
        }

        public void DrawWeather(SpriteBatch sb)
        {


            if (raining)
            {
                // more than just rain here..
                foreach (var drop in rain)
                    drop.Draw(sb, null);
            }
        }

        public bool IsRaining()
        {
            return raining;
        }
    }
}

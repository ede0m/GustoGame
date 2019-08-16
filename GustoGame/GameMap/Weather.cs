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
        int rainIntensity;
        float msToAddRainDrop;
        float msSinceDropAdded;
        float msToRemoveRainDrop;
        float msSinceDropRemoved;

        // List of different weather sprites??

        ContentManager _content;
        GraphicsDevice _graphics;

        // Handles timing and combination of weather events
        public Weather(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            rain = new List<RainDrop>();
            msToAddRainDrop = 25;
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            msCurrWeather += gameTime.ElapsedGameTime.Milliseconds;

            if (msCurrWeather >= msOfWeather && !raining) // and other weather patterns?
            {
                msOfWeather = RandomEvents.rand.Next(GameOptions.GameDayLengthMs / 10, GameOptions.GameDayLengthMs * 2); // weather can last between 1/10th of a day and 3 days
                msOfWeather = 30000; 
                msCurrWeather = 0;
               
                // RAIN chance
                int randRain = RandomEvents.rand.Next(0, 100);
                if (randRain <= 50) // 20% chance of rain
                {
                    raining = true;
                    rain.Clear();
                    rainIntensity = RandomEvents.rand.Next(100, 500);
                }

                // TODO: more weather patterns

            } 
            else
            {
                // continue with current weather

                // RAIN
                if (raining)
                {
                    // rain starts gradually
                    msSinceDropAdded += gameTime.ElapsedGameTime.Milliseconds;
                    msSinceDropRemoved += gameTime.ElapsedGameTime.Milliseconds;
                    if (rain.Count < rainIntensity && msSinceDropAdded > msToAddRainDrop)
                    {
                        rain.Add(new RainDrop(_content, _graphics));
                        msSinceDropAdded = 0;
                    }

                    foreach (var drop in rain)
                        drop.Update(kstate, gameTime);

                    if (msCurrWeather + (msOfWeather / 10) > msOfWeather && rain.Count > 0 && msSinceDropRemoved > msToRemoveRainDrop)
                    {
                        rain.RemoveAt(rain.Count-1);
                        msSinceDropRemoved = 0; // really means removed here
                    }
                    else if (msCurrWeather >= msOfWeather && rain.Count == 0)
                        raining = false;
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

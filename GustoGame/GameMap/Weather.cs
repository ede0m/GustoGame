using Comora;
using Gusto.GameMap.lightning;
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
        float msCurrWeather;
        //WeatherState state;

        float msToAddRainDrop;
        float msSinceDropAdded;
        float msToRemoveRainDrop;
        float msSinceDropRemoved;
        float msDrawingLightning;
        float msLightningTime;

        // List of different weather sprites??

        ContentManager _content;
        GraphicsDevice _graphics;

        // Handles timing and combination of weather events
        public Weather(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;


            WeatherState.rain = new List<RainDrop>();
            WeatherState.rainState = RainState.NOT;
            WeatherState.lightning = false;

            msToAddRainDrop = 25;
            msToRemoveRainDrop = 15;
            msLightningTime = 200;
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            msCurrWeather += gameTime.ElapsedGameTime.Milliseconds;

            if (msCurrWeather >= WeatherState.weatherDuration && WeatherState.rainState == RainState.NOT) // and other weather patterns?
            {
                WeatherState.weatherDuration = RandomEvents.rand.Next(GameOptions.GameDayLengthMs / 10, GameOptions.GameDayLengthMs * 2); // weather can last between 1/10th of a day and 3 days
                msCurrWeather = 0;
               
                // RAIN chance
                int randRain = RandomEvents.rand.Next(0, 100);
                if (randRain <= 20) // 20% chance of rain
                {
                    WeatherState.rainState = RainState.STARTING;
                    WeatherState.rain.Clear();
                    WeatherState.rainIntensity = RandomEvents.rand.Next(100, 500);

                    // Lightning with rain?
                    int randLightning = RandomEvents.rand.Next(1, 100);
                    if (randLightning <= 50)
                        WeatherState.lightning = true;

                }

                // TODO: more weather patterns

            } 
            else
            {
                // continue with current weather

                // RAIN
                if (WeatherState.rainState != RainState.NOT)
                {
                    // rain starts gradually
                    msSinceDropAdded += gameTime.ElapsedGameTime.Milliseconds;
                    msSinceDropRemoved += gameTime.ElapsedGameTime.Milliseconds;
                    if (WeatherState.rain.Count < WeatherState.rainIntensity && msSinceDropAdded > msToAddRainDrop && WeatherState.rainState == RainState.STARTING)
                    {
                        WeatherState.rain.Add(new RainDrop(_content, _graphics));
                        msSinceDropAdded = 0;
                    }
                    else if (WeatherState.rain.Count() == WeatherState.rainIntensity)
                    {
                        WeatherState.rainState = RainState.RAINING;

                        // random lightning strike
                        if (WeatherState.lightning)
                        {
                            if (WeatherState.lightningBolt == null || WeatherState.lightningBolt.IsComplete)
                            {
                                int randomLightning = RandomEvents.rand.Next(1, 1000);
                                if (randomLightning < 15) // this value could scale intensity
                                    WeatherState.lightningBolt = new BranchLightning(_content);
                                else
                                    WeatherState.lightningBolt = null;
                            }
                            else
                                WeatherState.lightningBolt.Update();
                        }
                    }

                    // rain starts receding gradually in the back 10th of the weatherEvent
                    if (msCurrWeather + (WeatherState.weatherDuration / 10) > WeatherState.weatherDuration && WeatherState.rain.Count > 0 && msSinceDropRemoved > msToRemoveRainDrop) 
                    {
                        WeatherState.rainState = RainState.ENDING;
                        WeatherState.lightning = false;
                        WeatherState.lightningBolt = null;
                        WeatherState.rain.RemoveAt(WeatherState.rain.Count - 1);
                        msSinceDropRemoved = 0;
                    }
                    else if (msCurrWeather >= WeatherState.weatherDuration && WeatherState.rain.Count == 0)
                        WeatherState.rainState = RainState.NOT;

                    foreach (var drop in WeatherState.rain)
                        drop.Update(kstate, gameTime);

                }
            }

            WeatherState.msThroughWeather = msCurrWeather;
        }

        public void DrawWeather(SpriteBatch sb)
        {


            if (WeatherState.rainState != RainState.NOT)
            {
                // more than just rain here..
                foreach (var drop in WeatherState.rain)
                    drop.Draw(sb, null);
            }
        }

        // sepearated lightning becase we want it to be drawn after ambient light shader, when the rest is drawn before it
        public void DrawLightning(SpriteBatch sb)
        {
            if (WeatherState.lightningBolt != null)
            {
                sb.Begin(SpriteSortMode.Texture, BlendState.Additive);
                WeatherState.lightningBolt.Draw(sb);
                sb.End();
            }
        }

    }
}

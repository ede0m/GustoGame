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
        WeatherState state;

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

            state = new WeatherState();

            msToAddRainDrop = 25;
            msToRemoveRainDrop = 15;
            msLightningTime = 200;
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            msCurrWeather += gameTime.ElapsedGameTime.Milliseconds;

            if (msCurrWeather >= state.weatherDuration && state.rainState == RainState.NOT) // and other weather patterns?
            {
                state.weatherDuration = RandomEvents.rand.Next(GameOptions.GameDayLengthMs / 10, GameOptions.GameDayLengthMs * 2); // weather can last between 1/10th of a day and 3 days
                msCurrWeather = 0;
               
                // RAIN chance
                int randRain = RandomEvents.rand.Next(0, 100);
                if (randRain <= 20) // 20% chance of rain
                {
                    state.rainState = RainState.STARTING;
                    state.rain.Clear();
                    state.rainIntensity = RandomEvents.rand.Next(100, 500);

                    // Lightning with rain?
                    int randLightning = RandomEvents.rand.Next(1, 100);
                    if (randLightning <= 50)
                        state.lightning = true;

                }

                // TODO: more weather patterns

            } 
            else
            {
                // continue with current weather

                // RAIN
                if (state.rainState != RainState.NOT)
                {
                    // rain starts gradually
                    msSinceDropAdded += gameTime.ElapsedGameTime.Milliseconds;
                    msSinceDropRemoved += gameTime.ElapsedGameTime.Milliseconds;
                    if (state.rain.Count < state.rainIntensity && msSinceDropAdded > msToAddRainDrop && state.rainState == RainState.STARTING)
                    {
                        state.rain.Add(new RainDrop(_content, _graphics));
                        msSinceDropAdded = 0;
                    }
                    else if (state.rain.Count() == state.rainIntensity)
                    {
                        state.rainState = RainState.RAINING;

                        // random lightning strike
                        if (state.lightning)
                        {
                            if (state.lightningBolt == null || state.lightningBolt.IsComplete)
                            {
                                int randomLightning = RandomEvents.rand.Next(1, 1000);
                                if (randomLightning < 15) // this value could scale intensity
                                    state.lightningBolt = new BranchLightning(_content);
                                else
                                    state.lightningBolt = null;
                            }
                            else
                                state.lightningBolt.Update();
                        }
                    }

                    // rain starts receding gradually in the back 10th of the weatherEvent
                    if (msCurrWeather + (state.weatherDuration / 10) > state.weatherDuration && state.rain.Count > 0 && msSinceDropRemoved > msToRemoveRainDrop) 
                    {
                        state.rainState = RainState.ENDING;
                        state.lightning = false;
                        state.lightningBolt = null;
                        state.rain.RemoveAt(state.rain.Count - 1);
                        msSinceDropRemoved = 0;
                    }
                    else if (msCurrWeather >= state.weatherDuration && state.rain.Count == 0)
                        state.rainState = RainState.NOT;

                    foreach (var drop in state.rain)
                        drop.Update(kstate, gameTime);

                }
            }

            state.msThroughWeather = msCurrWeather;
        }

        public void DrawWeather(SpriteBatch sb)
        {


            if (state.rainState != RainState.NOT)
            {
                // more than just rain here..
                foreach (var drop in state.rain)
                    drop.Draw(sb, null);
            }
        }

        // sepearated lightning becase we want it to be drawn after ambient light shader, when the rest is drawn before it
        public void DrawLightning(SpriteBatch sb)
        {
            if (state.lightningBolt != null)
            {
                sb.Begin(SpriteSortMode.Texture, BlendState.Additive);
                state.lightningBolt.Draw(sb);
                sb.End();
            }
        }

        public WeatherState GetWeatherState()
        {
            return state;
        }
    }
}

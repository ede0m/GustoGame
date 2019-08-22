using Comora;
using Gusto.Models.Interfaces;
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
    public class DayLight
    {
        float maxShadowTransparency;
        public float shadowTransparency;
        float sunAngleXStart; 
        public float sunAngleX; 

        float currentMsOfDay;
        float dayLengthMs;

        float percentDayComplete;
        float sunRisePercent;
        float sunSetPercent;
        float halfDayPercent;
        int sunRiseSetIntensity;

        float maxBlackoutIntensity;
        float overcastIntensity;
        float overcastAdder; // used to overexpose green in the shader during overcast
        float minIntensity;
        float currentIntensity;
        float tempCurrentIntensity;

        ContentManager _content;
        GraphicsDevice _graphics;
        RenderTarget2D sunRiseSetRange;
        Effect ambientLightEff;

        public DayLight(ContentManager content, GraphicsDevice graphics)
        {
            currentMsOfDay = 0;
            dayLengthMs = GameOptions.GameDayLengthMs; // how long the day takes

            // Note - when intensity is higher, the screen is darker. MinIntensity of 1 happens at peak day. We move currentIntensity down to one by half day and back up to maxBlackout through the end of the day. 

            maxBlackoutIntensity = 50;
            overcastIntensity = 7.5f;
            overcastAdder = 1f;
            minIntensity = 1;
            sunRiseSetIntensity = 2; // intensity of ambient light at sunrise and sunset
            currentIntensity = maxBlackoutIntensity;
            tempCurrentIntensity = maxBlackoutIntensity;

            sunRisePercent = 0.165f; // sunrise happens at 16.5% of day
            sunSetPercent = 0.835f; // sunset happens at 83.5% of day
            halfDayPercent = 0.5f;

            sunAngleXStart = 40; // angle for casting shadows. (-60 to 60) should take up sun time
            sunAngleX = sunAngleXStart;
            maxShadowTransparency = 0.15f;

            _content = content;
            _graphics = graphics;

            sunRiseSetRange = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height / 2);
            ambientLightEff = _content.Load<Effect>("ambientLight");
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {

            float elapsedMs = gameTime.ElapsedGameTime.Milliseconds;
            currentMsOfDay += elapsedMs;
            percentDayComplete = currentMsOfDay / dayLengthMs;

            // fade in/out shadows

            if (percentDayComplete > 0.05f && percentDayComplete < 0.85f)
                shadowTransparency += (gameTime.ElapsedGameTime.Milliseconds / (dayLengthMs * 0.1f)) * maxShadowTransparency;
            if (shadowTransparency > maxShadowTransparency)
                shadowTransparency = maxShadowTransparency;
            if (percentDayComplete > 0.83)
               shadowTransparency -= (gameTime.ElapsedGameTime.Milliseconds / (dayLengthMs * 0.1f)) * maxShadowTransparency;

            // don't surpass max angle for sun
            sunAngleX -= (gameTime.ElapsedGameTime.Milliseconds / dayLengthMs) * sunAngleXStart * 2; 
            if (sunAngleX < -1 * sunAngleXStart)
                sunAngleX = -1 * sunAngleXStart;

            float ambientIntensityChange = 0;
            float intensityDelta = 0;
            float msUntilChange = 0;
            bool incrementIntensity = false;

            if (percentDayComplete < sunRisePercent)
            {
                msUntilChange = dayLengthMs * sunRisePercent;
                intensityDelta = (maxBlackoutIntensity - sunRiseSetIntensity);
            }
            else if (percentDayComplete < halfDayPercent)
            {
                msUntilChange = dayLengthMs * (halfDayPercent - sunRisePercent);
                intensityDelta = sunRiseSetIntensity - minIntensity;
            }
            else if (percentDayComplete < sunSetPercent)
            {
                msUntilChange = dayLengthMs * (sunSetPercent - halfDayPercent);
                intensityDelta = sunRiseSetIntensity - minIntensity;
                incrementIntensity = true;
            }
            else if (percentDayComplete < 1.0f)
            {
                msUntilChange = dayLengthMs * (1.0f - sunSetPercent);
                intensityDelta = (maxBlackoutIntensity - sunRiseSetIntensity);
                incrementIntensity = true;
            }

            float sign = incrementIntensity ? 1 : -1;
            ambientIntensityChange = sign * intensityDelta / msUntilChange * elapsedMs;
            tempCurrentIntensity += ambientIntensityChange;

            // rain overcast - we always want to converge to tempCurrentIntensity in 1/10th of the storm time (the time be begin ending the storm)
            if (WeatherState.rainState == RainState.STARTING || WeatherState.rainState == RainState.RAINING)
            {
                if (tempCurrentIntensity < overcastIntensity)
                {
                    currentIntensity += 0.01f;

                    if (currentIntensity > overcastIntensity)
                        currentIntensity = overcastIntensity;
                }
                else
                    currentIntensity = tempCurrentIntensity;
                // used to expose more green during overcast
                overcastAdder += 0.01f;
                if (overcastAdder > 2)
                    overcastAdder = 2;

            }
            else if (WeatherState.rainState == RainState.ENDING)
            {
                if (tempCurrentIntensity < currentIntensity)
                    currentIntensity -= 0.05f;
                else
                    currentIntensity = tempCurrentIntensity;

                overcastAdder -= 0.01f;
                if (overcastAdder < 1)
                    overcastAdder = 1.0f;
            }
            else
            {
                currentIntensity = tempCurrentIntensity;
                overcastAdder = 1.0f;
            }


            // reset day
            if (percentDayComplete > 1.0f)
            {
                sunAngleX = sunAngleXStart;
                shadowTransparency = 0;
                percentDayComplete = 0.0f;
                currentMsOfDay = 0;
                currentIntensity = maxBlackoutIntensity;
                tempCurrentIntensity = maxBlackoutIntensity;
            }

        }

        // draws the game scene with post processing ambient light
        public void Draw(SpriteBatch sb, RenderTarget2D ambientLight, RenderTarget2D lightMaskTarget)
        {
            // ambient light
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ambientLightEff.Parameters["ambient"].SetValue(currentIntensity);
            ambientLightEff.Parameters["overcast"].SetValue(overcastAdder);
            ambientLightEff.Parameters["percentThroughDay"].SetValue(percentDayComplete);
            ambientLightEff.Parameters["lightMask"].SetValue(lightMaskTarget);
            ExecuteTechnique("ambientLightDayNight");
            sb.Draw(ambientLight, Vector2.Zero, Color.White);
            sb.End();
        }

        public void ExecuteTechnique(string techniqueName)
        {
            ambientLightEff.CurrentTechnique = ambientLightEff.Techniques[techniqueName];
            foreach (EffectPass pass in ambientLightEff.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }
    }
}

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
    public class DayLight : ICanUpdate
    {
        float currentMsOfDay;
        float dayLengthMs;

        float percentDayComplete;
        float sunRisePercent;
        float sunSetPercent;
        float halfDayPercent;
        int sunRiseSetIntensity;

        float maxBlackoutIntensity;
        float minIntensity;
        float currentIntensity;

        ContentManager _content;
        GraphicsDevice _graphics;
        RenderTarget2D sunRiseSetRange;
        Effect ambientLightEff;

        public DayLight(ContentManager content, GraphicsDevice graphics)
        {
            currentMsOfDay = 0;
            dayLengthMs = 60000; // how long the day takes

            maxBlackoutIntensity = 60;
            minIntensity = 1;
            sunRiseSetIntensity = 2; // intensity of ambient light at sunrise and sunset
            currentIntensity = maxBlackoutIntensity;

            sunRisePercent = 0.165f; // sunrise happens at 16.5% of day
            sunSetPercent = 0.835f; // sunset happens at 83.5% of day
            halfDayPercent = 0.5f;

            _content = content;
            _graphics = graphics;

            sunRiseSetRange = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height / 2);
            ambientLightEff = _content.Load<Effect>("ambientLight");
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            float elapsedMs = gameTime.ElapsedGameTime.Milliseconds;
            currentMsOfDay += elapsedMs;
            percentDayComplete = currentMsOfDay / dayLengthMs;

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
            currentIntensity += ambientIntensityChange;

            // reset day
            if (percentDayComplete > 1.0f)
            {
                percentDayComplete = 0.0f;
                currentMsOfDay = 0;
                currentIntensity = maxBlackoutIntensity;
            }

        }

        // draws the game scene with post processing ambient light
        public void Draw(SpriteBatch sb, RenderTarget2D ambientLight, RenderTarget2D lightMaskTarget)
        {
            // ambient light
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ambientLightEff.Parameters["ambient"].SetValue(currentIntensity);
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

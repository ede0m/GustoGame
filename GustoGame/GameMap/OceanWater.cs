using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GustoGame.GameMap
{
    public class OceanWater
    {
        ContentManager _content;
        GraphicsDevice _graphics;
        Effect oceanRippleEffect;
        //Texture2D oceanWaterTexture;
        Texture2D waterBumpMap;

        public OceanWater(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            oceanRippleEffect = _content.Load<Effect>("oceanRippleEffect");
            //oceanWaterTexture = _content.Load<Texture2D>("OceanWaterTexture");
            waterBumpMap = _content.Load<Texture2D>("waterbump");
        }

        public void Draw(SpriteBatch sb, RenderTarget2D waterScene)
        {
            // ocean ripple
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            oceanRippleEffect.Parameters["bumpMap"].SetValue(waterBumpMap);
            oceanRippleEffect.Parameters["water"].SetValue(waterScene);
            //oceanRippleEffect.Parameters["xWaveLength"].SetValue(3f);
            oceanRippleEffect.Parameters["xWaveHeight"].SetValue(0.3f);
            ExecuteTechnique("oceanRipple");
            sb.Draw(waterScene, Vector2.Zero, Color.White);
            sb.End();
        }

        public void ExecuteTechnique(string techniqueName)
        {
            oceanRippleEffect.CurrentTechnique = oceanRippleEffect.Techniques[techniqueName];
            foreach (EffectPass pass in oceanRippleEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }
    }
}

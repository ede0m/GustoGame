using Gusto.Utility;
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
        Texture2D noiseMap;
        float noiseOffset = 0.0f;

        public OceanWater(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            oceanRippleEffect = _content.Load<Effect>("oceanRippleEffect");
            noiseMap = _content.Load<Texture2D>("noise2");
        }

        public void Draw(SpriteBatch sb, RenderTarget2D waterScene, Vector2 camMove)
        {

            //noiseMap = GenerateNoiseMap(256);

            // ocean ripple
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            oceanRippleEffect.Parameters["noiseTexture"].SetValue(noiseMap);
            oceanRippleEffect.Parameters["water"].SetValue(waterScene);

            float noisePow = 0.3f;
            noiseOffset += 0.0001f;

            oceanRippleEffect.Parameters["noisePower"].SetValue(noisePow);
            oceanRippleEffect.Parameters["noiseOffset"].SetValue(noiseOffset);
            oceanRippleEffect.Parameters["noiseFrequency"].SetValue(noisePow * 3.0f);

            oceanRippleEffect.Parameters["camMoveX"].SetValue(camMove.X);
            oceanRippleEffect.Parameters["camMoveY"].SetValue(camMove.Y);
            //oceanRippleEffect.Parameters["camMove"].SetValue(camMove);

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

        private Texture2D GenerateNoiseMap(int resolution)
        {
            Color[] noisyColors = new Color[resolution * resolution];
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                    noisyColors[x + y * resolution] = new Color(new Vector3((float)RandomEvents.rand.Next(1000) / 1000.0f, 0, 0));

            Texture2D noiseImage = new Texture2D(_graphics, resolution, resolution);
            noiseImage.SetData(noisyColors);
            return noiseImage;
        }
    }
}

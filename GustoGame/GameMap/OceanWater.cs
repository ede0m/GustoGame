using Gusto;
using Gusto.Utility;
using GustoGame.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace GustoGame.GameMap
{
    public class OceanWater
    {
        ContentManager _content;
        GraphicsDevice _graphics;
        QuadRenderer _quadRenderer;

        Effect oceanRippleEffect;
        Texture2D noiseMap;
        RenderTarget2D oceanEffectRT;

        float noiseOffset = 0.0f;
        float noiseFreq = 1.0f; // This has to stay here.. (0.9F) why this value??!
        Vector2 noisePow = Vector2.Zero; // artistic param? 0.017f, 0.031f

        public OceanWater(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;
            _quadRenderer = new QuadRenderer(graphics);

            oceanEffectRT = new RenderTarget2D(_graphics, GameOptions.PrefferedBackBufferWidth, GameOptions.PrefferedBackBufferHeight);
            oceanRippleEffect = _content.Load<Effect>("oceanRippleEffect");
            noiseMap = _content.Load<Texture2D>("fractal-tiled");
        }

        public RenderTarget2D RenderOcean(RenderTarget2D waterScene, Vector2 camMove, Matrix wvm)
        {
            noisePow = new Vector2(0.01724f, 0.03125f) * 3; // 3 tile sample radius looks good
            noiseFreq = 1.0f; 
            noiseOffset += 0.0002f;

            oceanRippleEffect.Parameters["WorldViewProjection"].SetValue(wvm);
            oceanRippleEffect.Parameters["noiseOffset"].SetValue(noiseOffset);
            oceanRippleEffect.Parameters["noiseFrequency"].SetValue(noiseFreq);
            oceanRippleEffect.Parameters["camMove"].SetValue(camMove);
            oceanRippleEffect.Parameters["noisePower"].SetValue(noisePow);
            oceanRippleEffect.Parameters["noiseTexture"].SetValue(noiseMap);
            oceanRippleEffect.Parameters["water"].SetValue(waterScene);

            _graphics.SetRenderTarget(oceanEffectRT);
            _graphics.Clear(Color.PeachPuff);
            oceanRippleEffect.CurrentTechnique.Passes[0].Apply();
            _quadRenderer.Render(Vector2.One * -1, Vector2.One);

            return oceanEffectRT;
        }

        private void DebugNoiseDist()
        {
            var noiseData = new Color[noiseMap.Width * noiseMap.Height];
            noiseMap.GetData<Color>(noiseData);
            Dictionary<Color, int> dist = new Dictionary<Color, int>();
            foreach (var color in noiseData)
            {
                if (!dist.ContainsKey(color))
                    dist.Add(color, 1);
                else 
                    dist[color] += 1;
            }

            foreach (KeyValuePair<Color, int> kvp in dist)
                Trace.WriteLine(kvp.Key + ": " + ((double)kvp.Value/noiseData.Length));
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

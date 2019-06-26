using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public class LightArea
    {
        private GraphicsDevice graphicsDevice;

        public RenderTarget2D RenderTargetSmall { get; private set; }
        public RenderTarget2D RenderTargetLarge { get; private set; }
        public RenderTarget2D RenderTarget { get; private set; }
        public Vector2 LightPosition { get; set; }
        public Vector2 LightAreaSize { get; set; }

        public LightArea(GraphicsDevice graphicsDevice, ShadowMapSize size)
        {
            int baseSize = 2 << (int)size;
            LightAreaSize = new Vector2(baseSize);
            RenderTargetSmall = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            RenderTargetLarge = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            RenderTarget = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            this.graphicsDevice = graphicsDevice;
        }

        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - (LightPosition - LightAreaSize * 0.5f);
        }

        public void BeginDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }

        public void EndDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(null);
        }
    }
}

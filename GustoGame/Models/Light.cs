using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Light
    {
        GraphicsDevice _graphics;

        Texture2D lightMask;

        public Light(ContentManager content, GraphicsDevice graphics)
        {
            _graphics = graphics;
            lightMask = content.Load<Texture2D>("lightmask3");
        }

        public void Draw(SpriteBatch sb, Camera cam, Vector2 location)
        {
            sb.Begin(cam, SpriteSortMode.Immediate, BlendState.Additive);
            //sb.Draw(lightMask, location, null, Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            sb.Draw(lightMask, location, Color.Red);
            sb.End();

        }
    }
}

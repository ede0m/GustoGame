using Comora;
using Gusto.Bounding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        Color color;
        public float baseSize;
        public float scaleSize;
        Texture2D lightMask;
        Vector2 location;

        public bool lit;

        float toggleMs = 500;
        float msButtonHit;

        public Light(ContentManager content, GraphicsDevice graphics, float size, Color c)
        {
            _graphics = graphics;
            lightMask = content.Load<Texture2D>("lightmask3");
            baseSize = size;
            scaleSize = baseSize;

            color = c;
        }

        public void Draw(SpriteBatch sb, Camera cam)
        {
            Vector2 minCorner = new Vector2(cam.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(cam.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), cam.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            // if we are in the viewport
            if ((location.X >= minCorner.X && location.X <= maxCorner.X) && (location.Y >= minCorner.Y && location.Y <= maxCorner.Y))
            {
                sb.Begin(cam, SpriteSortMode.Immediate, BlendState.Additive);
                sb.Draw(lightMask, location, null, color, 0f, new Vector2(lightMask.Width / 2, lightMask.Height / 2), scaleSize, SpriteEffects.None, 0f);
                sb.End();
            }

        }

        public void Update(KeyboardState kstate, GameTime gametime, Vector2 loc)
        {
            location = loc;

            // toggle light -- Maybe move this to handheld
            if (kstate.IsKeyDown(Keys.T))
            {
                msButtonHit += gametime.ElapsedGameTime.Milliseconds;
                if (msButtonHit > toggleMs)
                {
                    lit = !lit;
                    msButtonHit = 0;
                }
            }

            if (lit)
                BoundingBoxLocations.LightLocationList.Add(this);
        }
    }
}

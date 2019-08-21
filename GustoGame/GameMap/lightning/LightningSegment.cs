using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap.lightning
{
    public class LightningSegment
    {
        public Vector2 A;
        public Vector2 B;
        public float Thickness;

        Texture2D halfCircle;
        Texture2D textLightningSegment;

        public LightningSegment() { }
        public LightningSegment(Vector2 a, Vector2 b, ContentManager content, float thickness = 1)
        {
            A = a;
            B = b;
            Thickness = thickness;

            halfCircle = content.Load<Texture2D>("halfCircleLightningSegment");
            textLightningSegment = content.Load<Texture2D>("lightningSegment");

        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 tangent = B - A;
            float rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            const float ImageThickness = 8;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(halfCircle.Width, halfCircle.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, textLightningSegment.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            spriteBatch.Draw(textLightningSegment, A, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(halfCircle, A, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(halfCircle, B, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }
    }
}

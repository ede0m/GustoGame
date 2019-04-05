using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Cannon : Sprite
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame;

        public Cannon() { }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Tuple<float, float> shipMovement)
        {
            // sail direction
            if (!kstate.IsKeyDown(Keys.LeftShift))
            {
                // ship direction
                if (kstate.IsKeyDown(Keys.A))
                    currRowFrame++;
                else if (kstate.IsKeyDown(Keys.D))
                    currRowFrame--;
            }
            BoundFrames();
            if (moving)
            {
                // map frame to vector movement
                location.X += shipMovement.Item1;
                location.Y += shipMovement.Item2;
            }
        }
    }
}

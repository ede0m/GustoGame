using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models.Animated
{
    public class Grass : Sprite, ICanUpdate, IGroundObject
    {

        int timeSinceLastFrame;

        TeamType team;
        ContentManager _content;
        GraphicsDevice _graphics;

        Vector2 startingLoc;

        bool animate;
        float msAnimate;
        float msNow;


        public Grass(TeamType t, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;
            msAnimate = 500;
            msNow = msAnimate;

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // grass walk through animation
            if (collidedWith is IWalks && collidedWith.moving && !animate)
                animate = true;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            if (animate && msNow > 0)
            {
                if (msNow > msAnimate - (msAnimate / 4))
                    rotation += 0.01f;
                else if (msNow > msAnimate - ((msAnimate / 4) * 3))
                    rotation -= 0.01f;
                else
                    rotation += 0.01f;

                msNow -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                animate = false;
                msNow = 500;
                rotation = 0;
            }

        }
    }
}

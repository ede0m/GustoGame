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
    public class Tree : Sprite, ICanUpdate, IGroundObject
    {
        TeamType team;
        ContentManager _content;
        GraphicsDevice _graphics;

        public Tree (TeamType t, ContentManager content, GraphicsDevice graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {

        }
    }
}

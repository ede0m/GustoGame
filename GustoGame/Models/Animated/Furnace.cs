using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gusto.Models.Animated
{
    public class Furnace : Sprite, ICanUpdate, IPlaceable
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;


        Random rand;
        public TeamType teamType;

        public Furnace(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            rand = new Random();

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            
        }
    }
}

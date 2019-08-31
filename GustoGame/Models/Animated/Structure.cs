using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Structure : Sprite, IStructure, IHasInterior, ICanUpdate
    {
        ContentManager _content;
        GraphicsDevice _graphics;

        bool nearStructure;

        Guid structureId;
        public Interior structureInterior;

        public Structure(TeamType t, ContentManager c, GraphicsDevice g) : base(g)
        {
            _content = c;
            _graphics = g;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IPlayer)
            {
                nearStructure = true;
            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            //throw new NotImplementedException();
            nearStructure = false;
        }


        public Guid GetInteriorForId()
        {
            return structureId;
        }

        public void SetInteriorForId(Guid id)
        {
            structureId = id;
        }

        public void DrawNearInterior(SpriteBatch sb, Camera cam)
        {
            if (nearStructure)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(cam);
                sb.DrawString(font, "i", new Vector2(GetBoundingBox().X + 20, GetBoundingBox().Y - 30), Color.Black);
                sb.End();
            }
        }


        // TODO: used for non moving interiors (so everything but ships lol)
    }
}

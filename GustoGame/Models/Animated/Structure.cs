using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Structure : Sprite, IStructure , IHasInterior
    {
        Guid structureId;

        public Structure(GraphicsDevice g) : base(g)
        {

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public Guid GetInteriorForId()
        {
            return structureId;
        }

        public void SetInteriorForId(Guid id)
        {
            structureId = id;
        }

        // TODO: used for non moving interiors (so everything but ships lol)
    }
}

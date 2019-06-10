using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Interfaces
{
    public interface IVulnerable
    {
        void DrawHealthBar(SpriteBatch sb, Camera camera);
    }
}

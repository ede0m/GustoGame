using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Interfaces
{
    public interface ICanUpdate
    {
        void Update(KeyboardState kstate, GameTime gameTime, Camera cam);
    }
}

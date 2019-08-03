using Comora;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.Models.Interfaces
{
    public interface IPlaceable
    {
        void DrawCanPickUp(SpriteBatch sb, Camera camera);
    }
}
